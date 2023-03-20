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

namespace Ai2Csproj
{
    internal enum MigrationBehavior
    {
        /// <summary>
        /// Migrate the assembly attribute over from the AssemblyInfo
        /// file to the csproj.
        /// </summary>
        migrate,

        /// <summary>
        /// Deletes the given assembly attribute from the AssemblyInfo
        /// file and does not add it to the csproj.
        /// </summary>
        delete,

        /// <summary>
        /// Leaves the given assembly attribute alone, and it remains
        /// inside of the AssemblyInfo file.
        /// </summary>
        leave
    }

    [Flags]
    internal enum SupportedAssemblyAttributes : long
    {
        assembly_company = ( 2L << 1 ),

        assembly_configuration = ( 2L << 2 ),

        assembly_copyright = ( 2L << 3 ),

        assembly_description = ( 2L << 4 ),

        assembly_file_version = ( 2L << 5 ),

        assembly_informational_version = ( 2L << 6 ),

        assembly_product = ( 2L << 7 ),

        assembly_Title = ( 2L << 8 ),

        assembly_version = ( 2L << 9 ),

        neutral_resources_language = ( 2L << 10 ),

        internals_visible_to = ( 2L << 11 ),

        com_visible = ( 2L << 12 ),

        assembly_trademark = ( 2L << 13 ),

        cls_compliant = ( 2L << 14 ),

        assembly_guid = ( 2L << 15 )
    }
}
