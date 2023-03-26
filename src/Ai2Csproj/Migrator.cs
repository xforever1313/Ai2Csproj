// Ai2Csproj - A program that migrates AssemblyInfo files to csproj.
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Reflection;
using System.Xml.Linq;
using Ai2Csproj.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ai2Csproj
{
    internal class Migrator
    {
        // ---------------- Fields ----------------

        private readonly Ai2CsprojConfig config;

        private readonly FileInfo csProjFile;

        private readonly FileInfo assemblyInfoFile;

        // ---------------- Constructor ----------------

        public Migrator( Ai2CsprojConfig config )
        {
            this.config = config;
            this.csProjFile = this.config.GetCsProjPathAsFileInfo();
            this.assemblyInfoFile = this.config.GetAsemblyInfoAsFileInfo();
        }

        // ---------------- Functions ----------------

        public MigrationResult Migrate()
        {
            string csProjContents = File.ReadAllText( this.csProjFile.FullName );
            string assemblyInfoContents = File.ReadAllText( this.assemblyInfoFile.FullName );

            return Migrate( csProjContents, assemblyInfoContents );
        }

        /// <remarks>
        /// Internal so unit tests can snag this.
        /// </remarks>
        internal MigrationResult Migrate( string csProjContents, string assemblyInfoContents )
        {
            var model = new AssemblyInfoModel( this.config );

            var parseOptions = new CSharpParseOptions(
                preprocessorSymbols: this.config.PreprocessorDefines.Defines
            );

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText( assemblyInfoContents, parseOptions );
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

            var attributeListsToKeep = new SyntaxList<AttributeListSyntax>();

            foreach( AttributeListSyntax attributeList in root.AttributeLists )
            {
                var attributesToKeep = new SeparatedSyntaxList<AttributeSyntax>();
                foreach( AttributeSyntax attributes in attributeList.Attributes )
                {
                    string name = attributes.Name.ToString();
                    Type? type = AssemblyAttributeMapping.TryGetTypeFromName( name );

                    var attributeParameters = new List<string>();
                    AttributeArgumentListSyntax? attributeArgSyntax = attributes.ArgumentList;
                    if( attributeArgSyntax is not null )
                    {
                        foreach( AttributeArgumentSyntax argumentSyntax in attributeArgSyntax.Arguments )
                        {
                            string? text = null;
                            ExpressionSyntax expression = argumentSyntax.Expression;
                            if( expression is InterpolatedStringExpressionSyntax interString )
                            {
                                text = interString.Contents.ToString();
                            }
                            else if( expression is LiteralExpressionSyntax litExpression )
                            {
                                if( litExpression.Kind() == SyntaxKind.StringLiteralExpression )
                                {
                                    text = litExpression.Token.ValueText;
                                }
                            }
                            else
                            {
                                text = expression.ToString();
                            }
                            if( text is not null )
                            {
                                attributeParameters.Add( text );
                            }
                        }
                    }

                    if( type is null )
                    {
                        if( this.config.MigrateUnsupportedTypes )
                        {
                            model.AddUnsupportedType( name, attributeParameters );
                        }
                        else
                        {
                            attributesToKeep = attributesToKeep.Add( attributes );
                        }
                    }
                    else
                    {
                        SupportedAssemblyAttributes? supportedAssembly = AssemblyAttributeMapping.TryGetAssemblyAttributeFromType( type );
                        if( supportedAssembly is null )
                        {
                            throw new InvalidOperationException(
                                $"Could not find a supported assembly attribute from type: {type}"
                            );
                        }

                        MigrationBehavior behavior = this.config.GetMigrationBehavior( supportedAssembly.Value );
                        model.AddSupportedType( type, attributeParameters, behavior );

                        if(
                            ( behavior != MigrationBehavior.migrate ) &&
                            ( behavior != MigrationBehavior.delete )
                        )
                        {
                            attributesToKeep = attributesToKeep.Add( attributes );
                        }
                    }
                }

                if( attributesToKeep.Any() )
                {
                    var newAttributeList = attributeList.WithAttributes( attributesToKeep );
                    attributeListsToKeep = attributeListsToKeep.Add( newAttributeList );
                }
            }

            model.Verify();

            root = root.WithAttributeLists( attributeListsToKeep );

            // If we contain anything other than using statements
            // we want to keep the file as-is.
            bool safeToDelete = this.config.DeleteOldAssemblyInfo;
            if( safeToDelete )
            {
                if( root.AttributeLists.Any() )
                {
                    safeToDelete = false;
                }
                else if( root.Externs.Any() )
                {
                    safeToDelete = false;
                }
                else if( root.Members.Any() )
                {
                    safeToDelete = false;
                }
                foreach( UsingDirectiveSyntax usingStatement in root.Usings )
                {
                    // Do not delete if there are global using statements,
                    // as those impact the entire assembly.
                    // We'll need to keep the file instead.
                    if( usingStatement.GlobalKeyword.ValueText == "global" )
                    {
                        safeToDelete = false;
                    }
                }
            }

            string newCsProj = BuildCsProj( csProjContents, model );
            string newAssemblyInfo;
            if( safeToDelete )
            {
                newAssemblyInfo = "";
            }
            else
            {
                newAssemblyInfo = root.ToFullString();
            }
            return new MigrationResult( newAssemblyInfo, newCsProj );
        }

        public void WriteFiles( MigrationResult result )
        {
            if( this.config.DeleteBackup == false )
            {
                BackupFile( csProjFile );
                BackupFile( assemblyInfoFile );
            }

            WriteFile( csProjFile, result.CsprojContents );
            WriteFile( assemblyInfoFile, result.AssemblyInfoContents );
        }

        private string BuildCsProj( string originalCsProjContents, AssemblyInfoModel model )
        {
            XElement? propertyGroup = model.GetAssemblyAttributesPropertyGroupXml();
            XElement? generatePropertyGroup = model.GetGeneratePropertyGroupXml();
            XElement? itemGroup = model.GetItemGroupXml();

            if( ( propertyGroup is null ) && ( generatePropertyGroup is null ) && ( itemGroup is null ) )
            {
                return originalCsProjContents;
            }

            XDocument csproj = XDocument.Parse( originalCsProjContents );
            XElement? root = csproj.Root;
            if( root is null )
            {
                throw new InvalidOperationException( "Somehow, the csproj root node is null" );
            }

            ResolveGenerateAssemblyInfo( root );

            if( ( propertyGroup is not null ) || ( generatePropertyGroup is not null ) )
            {
                XElement? lastPropertyGroup = null;
                foreach( XElement element in root.Elements() )
                {
                    if( "PropertyGroup".Equals( element.Name.LocalName ) )
                    {
                        lastPropertyGroup = element;
                    }
                }

                if( lastPropertyGroup is null )
                {
                    if( generatePropertyGroup is not null )
                    {
                        root.AddFirst( generatePropertyGroup );
                    }

                    if( propertyGroup is not null )
                    {
                        root.AddFirst( propertyGroup );
                    }
                }
                else
                {
                    if( generatePropertyGroup is not null )
                    {
                        lastPropertyGroup.AddAfterSelf( generatePropertyGroup );
                    }

                    if( propertyGroup is not null )
                    {
                        lastPropertyGroup.AddAfterSelf( propertyGroup );
                    }
                }
            }

            if( itemGroup is not null )
            {
                XElement? lastItemGroup = null;
                foreach( XElement element in root.Elements() )
                {
                    if( "ItemGroup".Equals( element.Name.LocalName ) )
                    {
                        lastItemGroup = element;
                    }
                }

                if( lastItemGroup is null )
                {
                    root.Add( itemGroup );
                }
                else
                {
                    lastItemGroup.AddAfterSelf( itemGroup );
                }
            }

            return csproj.ToString( SaveOptions.None );
        }

        private void ResolveGenerateAssemblyInfo( XElement root )
        {
            var elementsToRemove = new List<XElement>();
            foreach( XElement element in root.Elements() )
            {
                if( "PropertyGroup".Equals( element.Name.LocalName ) )
                {
                    var propertiesToRemove = new List<XElement>();
                    foreach( XElement propertyGroupElement in element.Elements() )
                    {
                        if( "GenerateAssemblyInfo".Equals( propertyGroupElement.Name.LocalName ) )
                        {
                            propertiesToRemove.Add( propertyGroupElement );
                        }
                    }
                    foreach( XElement toRemove in propertiesToRemove )
                    {
                        toRemove.Remove();
                    }

                    if( element.HasElements == false )
                    {
                        elementsToRemove.Add( element );
                    }
                }
            }

            foreach( XElement toRemove in elementsToRemove )
            {
                toRemove.Remove();
            }
        }

        private void BackupFile( FileInfo fileInfo )
        {
            FileInfo original = fileInfo;
            FileInfo destination = new FileInfo( $"{original.FullName}.old" );

            Console.WriteLine( $"Backing up '{original.FullName}' to '{destination.FullName}'." );

            if( config.DryRun == false )
            {
                File.Copy(
                    original.FullName,
                    destination.FullName
                );
            }
        }

        private void WriteFile( FileInfo file, string newContents )
        {
            if( string.IsNullOrEmpty( newContents ) )
            {
                Console.WriteLine( $"Deleteing file: {file.FullName}." );
                if( config.DryRun == false )
                {
                    File.Delete( file.FullName );
                }

                DirectoryInfo? directory = file.Directory;
                if( directory is not null )
                {
                    directory.Refresh();
                    if( directory.EnumerateFiles().Any() == false )
                    {
                        Console.WriteLine( $"Directory, '{directory.FullName}' is empty, deleting." );
                        if( config.DryRun == false )
                        {
                            Directory.Delete( directory.FullName );
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine( $"Writing the following contents to {file.FullName}:" );
                Console.WriteLine( newContents );
                Console.WriteLine();

                if( config.DryRun == false )
                {
                    File.WriteAllText( file.FullName, newContents );
                }
            }
        }
    }
}
