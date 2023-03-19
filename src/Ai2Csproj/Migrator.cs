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
            using var csProjStream = new FileStream( this.csProjFile.FullName, FileMode.Open, FileAccess.Read );
            using var assemblyInfoStream = new FileStream( this.csProjFile.FullName, FileMode.Open, FileAccess.Read );

            return Migrate( csProjStream, assemblyInfoStream );
        }

        /// <remarks>
        /// Internal so unit tests can snag this.
        /// </remarks>
        internal static MigrationResult Migrate( Stream csProjStream, Stream assemblyInfoStream )
        {
            return new MigrationResult( "", "" );
        }

        public void WriteFiles( MigrationResult result )
        {
            BackupFile( csProjFile );
            BackupFile( assemblyInfoFile );

            WriteFile( csProjFile, result.CsprojContents );
            WriteFile( assemblyInfoFile, result.AssemblyInfoContents );
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
