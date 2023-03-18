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

using Mono.Options;

namespace Ai2Csproj
{
    internal class ArgumentParser
    {
        // ---------------- Fields ----------------

        private readonly OptionSet optionSet;

        // ---------------- Constructor ----------------

        public ArgumentParser()
        {
            this.ShowHelp = false;
            this.ShowVersion = false;
            this.ShowLicense = false;
            this.ShowCredits = false;
            this.Config = new Ai2CsprojConfig();

            this.optionSet = new OptionSet
            {
                {
                    "h|help",
                    "Shows this mesage and exits.",
                    v => this.ShowHelp = ( v is not null )
                },
                {
                    "version",
                    "Shows the version and exits.",
                    v => this.ShowVersion = ( v is not null )
                },
                {
                    "print_license",
                    "Prints the software license and exits.",
                    v => this.ShowLicense = ( v is not null )
                },
                {
                    "print_credits",
                    "Prints the third-party notices and credits.",
                    v => this.ShowCredits = ( v is not null )
                },

                {
                    "project_path=",
                    "The path to the csproj to process.  If not specified, tries to search for a csproj with the same name as the current directory.",
                    v => this.Config = this.Config with { CsProjPath = new FileInfo( v ) }
                },
                {
                    "assembly_info_path=",
                    "The path to the AssemblyInfo.cs file.  If not specified, tries to search in the Properties directory in the same directory as csproj file.",
                    v => this.Config = this.Config  with{ AssmblyInfoPath = new FileInfo( v ) }
                },
                {
                    "delete_old_assembly_info",
                    "If specified, the old AssemblyInfo.cs file will be deleted if there is nothing else remaining inside of it.  Defaulted to false.",
                    v => this.Config = this.Config with { DeleteOldAssemblyInfo = v is not null }
                }
            };
        }

        // ---------------- Properties ----------------

        public bool ShowHelp { get; private set; }

        public bool ShowVersion { get; private set; }

        public bool ShowLicense { get; private set; }

        public bool ShowCredits { get; private set; }

        public Ai2CsprojConfig Config { get; private set; }

        // ---------------- Functions ----------------

        public void Parse( string[] args )
        {
            this.optionSet.Parse( args );
        }

        public void WriteHelp( TextWriter writer )
        {
            this.optionSet.WriteOptionDescriptions( writer );
        }
    }
}
