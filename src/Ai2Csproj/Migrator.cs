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

using System.Xml.Linq;
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
            this.csProjFile = this.config.CsProjPath;
            this.assemblyInfoFile = this.config.AssmblyInfoPath;
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
            var model = new AssemblyInfoModel();

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText( assemblyInfoContents );
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

            var attributesToDelete = new HashSet<AttributeSyntax>();

            foreach( AttributeListSyntax attributeList in root.AttributeLists )
            {
                foreach( AttributeSyntax attributes in attributeList.Attributes )
                {
                    string name = attributes.Name.ToString();
                    Type? type = AssemblyAttributeMapping.TryGetTypeFromName( name );
                    if( type is null )
                    {
                        continue;
                    }

                    var attributeParameters = new List<string>();
                    AttributeArgumentListSyntax? attributeArgSyntax = attributes.ArgumentList;
                    if( attributeArgSyntax is not null )
                    {
                        foreach( AttributeArgumentSyntax argumentSyntax in attributeArgSyntax.Arguments )
                        {
                            string text;
                            ExpressionSyntax expression = argumentSyntax.Expression;
                            if( expression is InterpolatedStringExpressionSyntax interString )
                            {
                                text = expression.ToString();
                            }
                            else
                            {
                                text = expression.ToString().Trim( '"' );
                            }
                            attributeParameters.Add( text );
                        }
                    }

                    SupportedAssemblyAttributes? supportedAssembly = AssemblyAttributeMapping.TryGetAssemblyAttributeFromType( type );
                    if( supportedAssembly is null )
                    {
                        throw new InvalidOperationException(
                            $"Could not find a supported assembly attribute from type: {type}"
                        );
                    }

                    MigrationBehavior behavior = this.config.GetMigrationBehavior( supportedAssembly.Value );
                    if( behavior == MigrationBehavior.migrate )
                    {
                        model.AddType( type, attributeParameters );
                    }

                    if(
                        ( behavior == MigrationBehavior.migrate ) ||
                        ( behavior == MigrationBehavior.delete )
                    )
                    {
                        attributesToDelete.Add( attributes );
                    }
                }
            }

            // If we contain anything other than using statements
            // we want to keep the file as-is.
            bool safeToDelete = true;
            foreach( SyntaxNode node in root.DescendantNodes() )
            {
                if( node is UsingDirectiveSyntax usingStatement )
                {
                    // Do not delete if there are global using statements,
                    // as those impact the entire assembly.
                    // We'll need to keep the file instead.
                    if( usingStatement.GlobalKeyword.IsMissing == false )
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
                newAssemblyInfo = "?";
            }
            return new MigrationResult( newCsProj, newAssemblyInfo );
        }

        public void WriteFiles( MigrationResult result )
        {
            BackupFile( csProjFile );
            BackupFile( assemblyInfoFile );

            WriteFile( csProjFile, result.CsprojContents );
            WriteFile( assemblyInfoFile, result.AssemblyInfoContents );
        }

        private string BuildCsProj( string originalCsProjContents, AssemblyInfoModel model )
        {
            XElement? propertyGroup = model.GetPropertyGroupXml();
            XElement? itemGroup = model.GetItemGroupXml();

            if( ( propertyGroup is null ) && ( itemGroup is null ) )
            {
                return originalCsProjContents;
            }

            XDocument csproj = XDocument.Load( originalCsProjContents );
            XElement? root = csproj.Root;
            if( root is null )
            {
                throw new InvalidOperationException( "Somehow, the csproj root node is null" );
            }
            if( root.HasElements == false )
            {
                if( propertyGroup is not null )
                {
                    root.Add( propertyGroup );
                }

                if( itemGroup is not null )
                {
                    root.Add( itemGroup );
                }
            }
            else
            {
                if( propertyGroup is not null )
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
                        root.AddFirst( propertyGroup );
                    }
                    else
                    {
                        lastPropertyGroup.AddAfterSelf( propertyGroup );
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
            }

            return csproj.ToString( SaveOptions.DisableFormatting );
        }

        private void BackupFile( FileInfo fileInfo )
        {
            FileInfo original = fileInfo;
            FileInfo destination = new FileInfo( $"{this.csProjFile.FullName}.old" );

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
                if(
                    ( directory is not null ) &&
                    ( directory.EnumerateFiles().Any() == false )
                )
                {
                    Console.WriteLine( $"Directory, '{directory.FullName}' is empty, deleting." );
                    if( config.DryRun == false )
                    {
                        Directory.Delete( directory.FullName );
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
