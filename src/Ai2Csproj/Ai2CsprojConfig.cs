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

using SethCS.Exceptions;

namespace Ai2Csproj
{
    internal record class Ai2CsprojConfig
    {
        // ---------------- Constructor ----------------

        public FileInfo CsProjPath { get; init; } = new FileInfo(
            $"{Environment.CurrentDirectory}.csproj"
        );

        public FileInfo AssmblyInfoPath { get; init; } = new FileInfo(
            Path.Combine( ".", "Properties", "AssemblyInfo.cs" )
        );

        public bool DeleteOldAssemblyInfo { get; init; } = false;
    }

    internal static class Ai2CsprojConfigExtensions
    {
        public static void Validate( this Ai2CsprojConfig config )
        {
            var errors = new List<string>();

            if( config.CsProjPath.Exists == false )
            {
                errors.Add( $"{config.CsProjPath.FullName} does not exist!" );
            }

            if( config.AssmblyInfoPath.Exists == false )
            {
                errors.Add( $"{config.AssmblyInfoPath.FullName} does not exist!" );
            }

            if( errors.Any() )
            {
                throw new ListedValidationException(
                    "Errors when validating arguments",
                    errors
                );
            }
        }
    }
}
