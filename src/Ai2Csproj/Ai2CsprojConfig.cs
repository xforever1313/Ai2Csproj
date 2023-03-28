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

using Ai2Csproj.Shared;
using SethCS.Exceptions;

namespace Ai2Csproj
{
    internal record class Ai2CsprojConfig
    {
        // ---------------- Properties ----------------

        public bool DryRun { get; init; } = false;

        public bool DeleteBackup { get; init; } = false;

        public string CsProjPath { get; init; } =
            $"{Path.GetFileName( Environment.CurrentDirectory )}.csproj";

        public string AssmblyInfoPath { get; init; } = Path.Combine(
            Environment.CurrentDirectory,
            "Properties",
            "AssemblyInfo.cs"
        );

        public bool DeleteOldAssemblyInfo { get; init; } = false;

        public bool MigrateUnsupportedTypes { get; init; } = false;

        public VersionSource VersionSourceStrategy { get; init; } = VersionSource.exclude_version;

        public SupportedAssemblyAttributes TypesToDelete { get; init; }

        public SupportedAssemblyAttributes TypesToMigrate { get; init; }

        public SupportedAssemblyAttributes TypesToLeave { get; init; }

        public PreprocessorDefinesCollection PreprocessorDefines { get; init; }

        // ---------------- Functions ----------------

        public FileInfo GetCsProjPathAsFileInfo()
        {
            return new FileInfo( this.CsProjPath );
        }

        public FileInfo GetAsemblyInfoAsFileInfo()
        {
            return new FileInfo( this.AssmblyInfoPath );
        }

        public void Validate()
        {
            var errors = new List<string>();

            FileInfo csProjPath = GetCsProjPathAsFileInfo();
            FileInfo assemblyInfoPath = GetAsemblyInfoAsFileInfo();

            if( csProjPath.Exists == false )
            {
                errors.Add( $"{csProjPath.FullName} does not exist!" );
            }

            if( assemblyInfoPath.Exists == false )
            {
                errors.Add( $"{assemblyInfoPath.FullName} does not exist!" );
            }

            if( errors.Any() )
            {
                throw new ListedValidationException(
                    "Errors when validating configuration",
                    errors
                );
            }
        }

        public MigrationBehavior GetMigrationBehavior( SupportedAssemblyAttributes attribute )
        {
            if( ( attribute & this.TypesToMigrate ) == attribute )
            {
                return MigrationBehavior.migrate;
            }
            else if( ( attribute & this.TypesToLeave ) == attribute )
            {
                return MigrationBehavior.leave;
            }
            else if( ( attribute & this.TypesToDelete ) == attribute )
            {
                return MigrationBehavior.delete;
            }
            else
            {
                // Assume migrate is the default.
                return MigrationBehavior.migrate;
            }
        }
    }
}
