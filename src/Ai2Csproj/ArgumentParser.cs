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

using System.Reflection;
using Ai2Csproj.Shared;
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
                    "list_supported_types",
                    "Prints a list of the supported AssemblyAttributes that can be migrated",
                    v => this.ShowSupportedTypes = ( v is not null )
                },

                {
                    "project_path=",
                    "The path to the csproj to process.  If not specified, tries to search for a csproj with the same name as the current directory.",
                    v => this.Config = this.Config with { CsProjPath = v }
                },
                {
                    "assembly_info_path=",
                    "The path to the AssemblyInfo.cs file.  If not specified, tries to search in the Properties directory in the same directory as csproj file.",
                    v => this.Config = this.Config  with{ AssmblyInfoPath = v }
                },
                {
                    "delete_old_assembly_info",
                    "If specified, the old AssemblyInfo.cs file will be deleted if there is nothing else remaining inside of it.",
                    v => this.Config = this.Config with { DeleteOldAssemblyInfo = v is not null }
                },
                {
                    "migrate_unsupported_types",
                    "If specified, unsupported types will be attempted to migrated by using the 'AssemblyAttribute' XML element.",
                    v => this.Config = this.Config with { MigrateUnsupportedTypes = v is not null }
                },
                {
                    "dry_run",
                    "If specified, no files will be changed, but instead what will happen will be printed to STDOUT.",
                    v => this.Config = this.Config with { DryRun = v is not null }
                },
                {
                    "no_backup",
                    "If specified, backups of the files that will be changed will not be created.",
                    v => this.Config = this.Config with { DeleteBackup = v is not null }
                },
                {
                    "version_source=",
                    "If specified, this creates a Version tag inside of the csproj.  " +
                    "However, the version number can come from a number of attriutes.  " +
                    "This selects which assembly attribute to grab the version from.  " +
                    $"{VersionSource.exclude_version} to not include a Version tag in the csproj.  " +
                    $"{VersionSource.use_assembly_version} to use the {nameof( AssemblyVersionAttribute )} for the Version tag in the csproj.  " +
                    $"{VersionSource.use_file_version} to use the {nameof( AssemblyFileVersionAttribute)} for the Version tag in the csproj.  " +
                    $"{VersionSource.use_informational_version} to use the {nameof( AssemblyInformationalVersionAttribute )} for the Version tag in the csproj.",
                    v =>
                    {
                        if( Enum.TryParse( v, out VersionSource versionSource ) == false )
                        {
                            throw new ArgumentException(
                                $"Unknown value for version_source: {v}",
                                "version_source"
                            );
                        }

                        this.Config = this.Config with { VersionSourceStrategy = versionSource };
                    }
                }
            };
            
            foreach( SupportedAssemblyAttributes supportedAttribute in Enum.GetValues<SupportedAssemblyAttributes>() )
            {
                optionSet.Add(
                    $"{supportedAttribute}_behavior=",
                    $"How to handle '{AssemblyAttributeMapping.GetType( supportedAttribute ).FullName}'.  " +
                    $"{MigrationBehavior.migrate} (default) - migrate to csproj.  {MigrationBehavior.delete} - remove from AssemblyInfo do not put in csproj.  " +
                    $"{MigrationBehavior.leave} - leave in the AssemblyInfo file",
                    ( string value ) =>
                    {
                        if( Enum.TryParse( value, out MigrationBehavior migrateBehavior ) == false )
                        {
                            throw new ArgumentException(
                                $"Unknown value for behavior attribute: {value}"
                            );
                        }
                        else if( migrateBehavior == MigrationBehavior.migrate )
                        {
                            this.Config = this.Config with { TypesToMigrate = Config.TypesToMigrate | supportedAttribute };
                        }
                        else if( migrateBehavior == MigrationBehavior.delete )
                        {
                            this.Config = this.Config with { TypesToDelete = Config.TypesToDelete | supportedAttribute };
                        }
                        else if( migrateBehavior == MigrationBehavior.leave )
                        {
                            this.Config = this.Config with { TypesToLeave = Config.TypesToLeave | supportedAttribute };
                        }
                    }
                );
            }
        }

        // ---------------- Properties ----------------

        public bool ShowHelp { get; private set; }

        public bool ShowVersion { get; private set; }

        public bool ShowLicense { get; private set; }

        public bool ShowCredits { get; private set; }

        public bool ShowSupportedTypes { get; private set; }

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
