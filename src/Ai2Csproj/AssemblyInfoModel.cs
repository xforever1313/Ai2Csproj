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

using System.Collections.Immutable;
using System.Xml.Linq;
using Ai2Csproj.Exceptions;
using Ai2Csproj.Shared;

namespace Ai2Csproj
{
    internal class AssemblyInfoModel
    {
        // ---------------- Fields ----------------

        private readonly Ai2CsprojConfig config;

        private readonly HashSet<Type> existingTypes;
        private readonly List<SupportedAttributeInfo> assemblyAttributes;
        private readonly List<Tuple<string, ImmutableArray<string>>> unsupportedAssemblyAttributes;

        private readonly List<string> errors;

        private string? versionString;

        // ---------------- Constructor ----------------

        public AssemblyInfoModel( Ai2CsprojConfig config )
        {
            this.config = config;

            this.existingTypes = new HashSet<Type>();
            this.assemblyAttributes = new List<SupportedAttributeInfo>();
            this.unsupportedAssemblyAttributes = new List<Tuple<string, ImmutableArray<string>>>();
            this.errors = new List<string>();

            this.versionString = null;
        }

        // ---------------- Functions ----------------

        public void AddSupportedType( Type type, IEnumerable<string> parameters, MigrationBehavior behavior )
        {
            if( AssemblyAttributeMapping.IsMultiplePerAssemblyAllowed( type ) == false )
            {
                if( this.existingTypes.Contains( type ) )
                {
                    errors.Add( $"{type.FullName} was already added! Only 1 can exist." );
                    return;
                }
            }

            int? expectedArgs = AssemblyAttributeMapping.TryGetExectedNumberOfParameters( type );
            if( expectedArgs is not null )
            {
                if( parameters.Count() != expectedArgs )
                {
                    errors.Add(
                        $"{type.FullName} can only have {expectedArgs} and only {expectedArgs} parameter(s), got: {parameters.Count()}."
                    );
                    return;
                }
            }

            var info = new SupportedAttributeInfo( type, parameters.ToImmutableArray(), behavior );

            this.existingTypes.Add( type );
            this.assemblyAttributes.Add( info );
        }

        public void AddUnsupportedType( string type, IEnumerable<string> parameters )
        {
            var tuple = new Tuple<string, ImmutableArray<string>>(
                type,
                parameters.ToImmutableArray()
            );

            this.unsupportedAssemblyAttributes.Add( tuple );
        }

        public void Verify()
        {
            if( this.errors.Any() )
            {
                throw new SyntaxTreeParseException( this.errors );
            }

            if( this.config.VersionSourceStrategy != VersionSource.exclude_version )
            {
                Type expectedType = AssemblyAttributeMapping.GetVersionSourceType( this.config.VersionSourceStrategy );

                var foundAttribute = this.assemblyAttributes.FirstOrDefault( t => t.Type.Equals( expectedType ) );
                if( foundAttribute == default )
                {
                    throw new MissingVersionSourceException(
                        this.config.VersionSourceStrategy
                    );
                }

                this.versionString = foundAttribute.Parameters.FirstOrDefault() ?? string.Empty;
            }
        }

        public XElement? GetAssemblyAttributesPropertyGroupXml()
        {
            var element = new XElement( "PropertyGroup" );

            foreach( var attribute in this.assemblyAttributes )
            {
                string? propertyGroupName = AssemblyAttributeMapping.TryGetPropertyGroupName( attribute.Type );
                if( propertyGroupName is null )
                {
                    continue;
                }
                else if( attribute.MigrationBehavior != MigrationBehavior.migrate )
                {
                    continue;
                }

                element.Add(
                    new XElement( propertyGroupName, attribute.Parameters.First() )
                );
            }

            if( this.versionString is not null )
            {
                element.Add(
                    new XElement( "Version", this.versionString )
                );
            }

            if( element.HasElements == false )
            {
                return null;
            }

            return element;
        }

        public XElement? GetGeneratePropertyGroupXml()
        {
            var element = new XElement( "PropertyGroup" );

            foreach( var attribute in this.assemblyAttributes )
            {
                string? propertyGroupName = AssemblyAttributeMapping.TryGetDisableGenerationXmlName( attribute.Type );
                if( propertyGroupName is null )
                {
                    continue;
                }
                else if( attribute.MigrationBehavior != MigrationBehavior.leave )
                {
                    continue;
                }

                element.Add(
                    new XElement( propertyGroupName, "false" )
                );
            }

            if( element.HasElements == false )
            {
                return null;
            }

            return element;
        }

        public XElement? GetItemGroupXml()
        {
            XElement element = new XElement( "ItemGroup" );

            void AddElement( string? typeName, IEnumerable<string> parameters )
            {
                var attributeElement = new XElement( "AssemblyAttribute" );
                int i = 1;
                foreach( string parameter in parameters )
                {
                    var parameterElement = new XElement(
                        $"_Parameter{i}",
                        parameter
                    );
                    ++i;

                    attributeElement.Add( parameterElement );
                }

                if( typeName is not null )
                {
                    attributeElement.Add(
                        new XAttribute( "Include", typeName )
                    );
                }

                element.Add( attributeElement );
            }

            foreach( var attribute in this.assemblyAttributes )
            {
                string? propertyGroupName = AssemblyAttributeMapping.TryGetPropertyGroupName( attribute.Type );
                if( propertyGroupName is not null )
                {
                    continue;
                }
                else if( attribute.MigrationBehavior != MigrationBehavior.migrate )
                {
                    continue;
                }

                AddElement( attribute.Type.FullName, attribute.Parameters );
            }

            foreach( var attribute in this.unsupportedAssemblyAttributes )
            {
                AddElement( attribute.Item1, attribute.Item2 );
            }

            if( element.HasElements == false )
            {
                return null;
            }

            return element;
        }

        private class SupportedAttributeInfo
        {
            // ---------------- Constructor ----------------

            public SupportedAttributeInfo(
                Type type,
                ImmutableArray<string> parameters,
                MigrationBehavior migrationBehavior
            )
            {
                this.Type = type;
                this.Parameters = parameters;
                this.MigrationBehavior = migrationBehavior;
            }

            // ---------------- Properties ----------------

            public Type Type { get; private set; }

            public ImmutableArray<string> Parameters { get; private set; }

            public MigrationBehavior MigrationBehavior { get; private set; }
        }
    }
}
