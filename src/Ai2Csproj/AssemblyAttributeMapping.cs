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

namespace Ai2Csproj
{
    /// <summary>
    /// Maps assembly attributes to their equivalent csproj values.
    /// </summary>
    public static class AssemblyAttributeMapping
    {
        // ---------------- Fields ----------------

        private static readonly IReadOnlyDictionary<Type, string> attributeToXmlMapping;

        // ---------------- Constructor ----------------

        static AssemblyAttributeMapping()
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

        // ---------------- Functions ----------------
    }
}
