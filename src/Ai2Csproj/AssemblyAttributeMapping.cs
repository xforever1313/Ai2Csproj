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

using System.Collections.ObjectModel;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ai2Csproj
{
    /// <summary>
    /// Maps assembly attributes to their equivalent csproj values.
    /// </summary>
    internal static class AssemblyAttributeMapping
    {
        // ---------------- Fields ----------------

        private static readonly IReadOnlyDictionary<Type, string> attributeToXmlMapping;

        private static readonly IReadOnlyDictionary<SupportedAssemblyAttributes, Type> supportedAssembliesMapping;

        // ---------------- Constructor ----------------

        static AssemblyAttributeMapping()
        {
            {
                var dict = new Dictionary<Type, string>
                {
                    // Taken from here:
                    // https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#assembly-attribute-properties
                    [typeof( AssemblyCompanyAttribute )] = "Company",
                    [typeof( AssemblyConfigurationAttribute )] = "Configuration",
                    [typeof( AssemblyCopyrightAttribute )] = "Copyright",
                    [typeof( AssemblyDescriptionAttribute )] = "Description",
                    [typeof( AssemblyFileVersionAttribute )] = "FileVersion",
                    [typeof( AssemblyInformationalVersionAttribute )] = "InformationalVersion",
                    [typeof( AssemblyProductAttribute )] = "Product",
                    [typeof( AssemblyTitleAttribute )] = "AssemblyTitle",
                    [typeof( AssemblyVersionAttribute )] = "AssemblyVersion",
                    [typeof( NeutralResourcesLanguageAttribute )] = "NeutralLanguage"
                };

                attributeToXmlMapping = new ReadOnlyDictionary<Type, string>( dict );
            }

            {
                var dict = new Dictionary<SupportedAssemblyAttributes, Type>
                {
                    [SupportedAssemblyAttributes.assembly_company] = typeof( AssemblyCompanyAttribute ),
                    [SupportedAssemblyAttributes.assembly_configuration] = typeof( AssemblyConfigurationAttribute ),
                    [SupportedAssemblyAttributes.assembly_copyright] = typeof( AssemblyCopyrightAttribute ),
                    [SupportedAssemblyAttributes.assembly_description] = typeof( AssemblyDescriptionAttribute ),
                    [SupportedAssemblyAttributes.assembly_file_version] = typeof( AssemblyFileVersionAttribute ),
                    [SupportedAssemblyAttributes.assembly_informational_version] = typeof( AssemblyInformationalVersionAttribute ),
                    [SupportedAssemblyAttributes.assembly_product] = typeof( AssemblyProductAttribute ),
                    [SupportedAssemblyAttributes.assembly_Title] = typeof( AssemblyTitleAttribute ),
                    [SupportedAssemblyAttributes.assembly_version] = typeof( AssemblyVersionAttribute ),
                    [SupportedAssemblyAttributes.neutral_resources_language] = typeof( NeutralResourcesLanguageAttribute ),

                    [SupportedAssemblyAttributes.assembly_trademark] = typeof( AssemblyTrademarkAttribute ),
                    [SupportedAssemblyAttributes.internals_visible_to] = typeof( InternalsVisibleToAttribute ),
                    [SupportedAssemblyAttributes.com_visible] = typeof( ComVisibleAttribute ),
                    [SupportedAssemblyAttributes.cls_compliant] = typeof( CLSCompliantAttribute ),
                };

                supportedAssembliesMapping = new ReadOnlyDictionary<SupportedAssemblyAttributes, Type>( dict );
            }
        }

        // ---------------- Functions ----------------

        /// <summary>
        /// Tries to get the PropertyGroupName of the given attribute,
        /// if one exists.  If this returns not-null,
        /// then the type should go in the PropertyGroup element
        /// whose name is the returned type.
        /// 
        /// Otherwise, this goes in an ItemGroup.
        /// </summary>
        public static string? TryGetPropertyGroupName( Type type )
        {
            if( attributeToXmlMapping.ContainsKey( type ) )
            {
                return attributeToXmlMapping[type];
            }

            return null;
        }

        public static Type? TryGetTypeFromName( string typeName )
        {
            if( typeName.EndsWith( "Attribute" ) == false )
            {
                typeName = $"{typeName}Attribute";
            }

            foreach( Type type in supportedAssembliesMapping.Values )
            {
                if(
                    typeName.Equals( type.FullName ) ||
                    typeName.Equals( type.Name )
                )
                {
                    return type;
                }
            }

            return null;
        }

        public static SupportedAssemblyAttributes? TryGetAssemblyAttributeFromType( Type type )
        {
            foreach( var kvp in supportedAssembliesMapping )
            {
                if( type.Equals( kvp.Value ) )
                {
                    return kvp.Key;
                }
            }

            return null;
        }

        public static Type GetType( SupportedAssemblyAttributes attribute )
        {
            return supportedAssembliesMapping[attribute];
        }

        public static IEnumerable<Type> GetSupportedTypes()
        {
            return supportedAssembliesMapping.Values;
        }

        public static bool IsMultiplePerAssemblyAllowed( Type type )
        {
            if( type.Equals( typeof( InternalsVisibleToAttribute ) ) )
            {
                return true;
            }
            else if( supportedAssembliesMapping.Values.Contains( type ) )
            {
                // All other supported assemblies (to our knowledge)
                // allow just one declaration per assembly.
                return false;
            }

            // Assume its a user type, and we just don't know.
            // Assume its allowed.
            return true;
        }

        public static bool IsOnlyOneParameterAllowed( Type type )
        {
            // All of our known attributes only allow one paramter.
            // *Technically*, NeutralResourcesLanguageAttribute has a Constructor
            // with 2 parameters, but it probably won't work at compile time
            // since its an interface.
            //
            // All other types, we can't make assumptions, assume they're allowed
            // any number of parameters.
            return supportedAssembliesMapping.Values.Contains( type );
        }
    }
}
