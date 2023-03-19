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

namespace Ai2Csproj
{
    internal class AssemblyInfoModel
    {
        // ---------------- Fields ----------------

        private readonly HashSet<Type> existingTypes;
        private readonly List<Tuple<Type, ImmutableArray<string>>> assemblyAttributes;

        private readonly List<string> errors;

        // ---------------- Constructor ----------------

        public AssemblyInfoModel()
        {
            this.existingTypes = new HashSet<Type>();
            this.assemblyAttributes = new List<Tuple<Type, ImmutableArray<string>>>();
            this.errors = new List<string>();
        }

        // ---------------- Functions ----------------

        public void AddType( Type type, IEnumerable<string> parameters )
        {
            if( AssemblyAttributeMapping.IsMultiplePerAssemblyAllowed( type ) == false )
            {
                if( this.existingTypes.Contains( type ) )
                {
                    errors.Add( $"{type.FullName} was already added! Only 1 can exist." );
                    return;
                }
            }
            else if(
                AssemblyAttributeMapping.IsOnlyOneParameterAllowed( type ) &&
                ( parameters.Count() != 1 )
            )
            {
                errors.Add(
                    $"{type.FullName} can only have 1 and only 1 parameter, got: {parameters.Count()}."
                );
            }

            var tuple = new Tuple<Type, ImmutableArray<string>>(
                type,
                parameters.ToImmutableArray()
            );

            this.assemblyAttributes.Add( tuple );
        }

        public void ThrowIfErrors()
        {
            if( this.errors.Any() )
            {
                throw new SyntaxTreeParseException( this.errors );
            }
        }

        public XElement? GetPropertyGroupXml()
        {
            var element = new XElement( "PropertyGroup" );

            foreach( var attribute in this.assemblyAttributes )
            {
                string? propertyGroupName = AssemblyAttributeMapping.TryGetPropertyGroupName( attribute.Item1 );
                if( propertyGroupName is null )
                {
                    continue;
                }

                element.Add(
                    new XElement( propertyGroupName, attribute.Item2.First() )
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
            var element = new XElement( "ItemGroup" );

            foreach( var attribute in this.assemblyAttributes )
            {
                string? propertyGroupName = AssemblyAttributeMapping.TryGetPropertyGroupName( attribute.Item1 );
                if( propertyGroupName is not null )
                {
                    continue;
                }

                var attributeElement = new XElement( "AssemblyAttribute" );
                int i = 1;
                foreach( string parameter in attribute.Item2 )
                {
                    var parameterElement = new XElement(
                        $"_Parameter{i}",
                        parameter
                    );

                    attributeElement.Add( parameterElement );
                }

                string? typeName = attribute.Item1.FullName;
                if( typeName is not null )
                {
                    attributeElement.Add(
                        new XAttribute( "Include", typeName )
                    );
                }

                element.Add( attributeElement );
            }

            if( element.HasElements == false )
            {
                return null;
            }

            return element;
        }
    }
}
