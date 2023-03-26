# AssemblyInfo to csproj

AssemblyInfo to csproj (ai2csproj) is a tool that can be used to migrate all AssemblyAttributes defined in an AssemblyInfo.cs file and migrates them into a csproj.  The main use case is migrating a C# codebase from .NET Framework to .NET Core or newer.

## Installation

This can be installed as a dotnet tool.  To install globally for your user, run ```dotnet tool install -g ai2csproj```.

## Usage

To migrate Assembly Attributes to the .csproj, simply run ```ai2csproj``` in the same directory as the .csproj.  By default, it will try to find a .csproj whose name is the same as the directory name, and search the Properties folder of the working directory for an AssemblyInfo.cs file.  If your project does not have this layout, you'll have to specify the appropriate ```--project_path``` and ```--assembly_info_path``` command line arguments.

### Supported vs unsupported types

There are AssemblyAttributes ai2csproj knows how to work with right out of the box.  These are considered "supported" types.  To get a list of supported types and exit, pass in the ```--list_supported_types``` command line argument.

Supported types have the benefit of you being able to specify how ai2csproj handles each attribute type.  This can be controlled with "behavior" flags (pass in ```--help``` to see a list of behavior flags).  Three values can be passed in to modify how ai2csproj handles each supported attribute.

* ```migrate``` - Move this attribute type from the AssemblyInfo file and into the csproj.  This is the default behavior if a behavior argument is not specified.
* ```delete``` - Delete this attribute type from the AssemblyInfo file and _do not_ move it to the csproj (in other words, delete the attribute completely from the assembly).
* ```leave``` - Do not migrate the attribute to the csproj file, and leave it inside of AssemblyInfo.cs.

Unsupported types can still be migrated just fine; but by default they are left alone.  Simply pass in ```--migrate_unsupported_types``` on the command line to migrate _all_ unsupported types (its all or nothing).

### Version Sources

The new SDK-Style .csproj format includes a ```<Version>``` property.  According to [Microsoft's Documentation](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props), if this is specified, but ```<AssemblyVersion>```, ```<FileVersion>```, and/or ```<InformationalVersion>``` are not, then those three values are defaulted to whatever is in the ```<Version>``` tag (AssemblyVersion and FileVersion have any suffixes specified in ```<Version>``` removed).

The ```<Version>``` property does not tie directly to an Assembly Attribute.  Therefore, the tool needs to know where to pull the version information from.  This is done via the ```--version_source``` argument.  By default, no ```<Version>``` property is generated.  However, if the argument is specified, these are the possible options:

* ```exclude_version``` - Do not add a ```<Version>``` property to the .csproj file.  This is the default behavior if the ```--version_source``` argument is not specified.
* ```use_assembly_version``` - Copy the AssemblyVersion value into the csproj's Version element.
* ```use_file_version``` - Copy the AssemblyFileVersion value into the csproj's Version element.
* ```use_informational_version``` - Copy the AssemblyInformationalVersion value into the csproj's Version element.

Note that if a ```--version_source``` is specified, the attribute that it copied the information from is _still_ copied to the csproj by default.  So, for example, if ```--version_source=use_assembly_version``` is passed in, the csproj will contain a ```<Version>``` and a ```<AssemblyVersion>``` tag.  To have it completely migrate away from ```<AssemblyVersion>``` (or any of the other version attributes) and have the csproj only contain ```<Version>```, append the appropriate behavior argument set to delete, such as ```--assembly_version_behavior=delete```.

### Other flags

* ```--dry_run``` - Prints to the STDOUT what ai2csproj will do without actually doing it.  The final contents of the csproj and AssemblyInfo file will be printed to STDOUT as well.
* ```--no_backup``` - By default, backups are made of the csproj and AssemblyInfo.cs file by copying them into the same directory they are in and appending ".old" to the end of the file name.
* ```--delete_old_assembly_info``` - Attempts to delete the old AssemblyInfo file.  The file will only be delete if the only thing left in the file are non-global namespaces or comments.  If the AssemblyInfo file contains anything else, it will be left alone.
* ```--define``` - Add a preprocessor define.  Useful if you have attributes set to different values with a #if statement, and want to specify which one to migrate over.  Note, any attributes that are excluded from the preprocessor definitions are **deleted** (see limitations section).

## Limitations

* When writing out the modified csproj or AssemblyInfo file, code formatting and whitespace is not guaranteed.  While the contents of the file should stay the same, the whitespace may look different post-migration.
* When parsing an AssemblyInfo, if you have your own assembly attribute that just so happens to match the same name as one of the supported assembly attributes, that will be removed and migrated to the csproj.  The way to have the tool not do this is by passing in the appropriate "leave" argument for that attribute so the tool leaves that assembly attribute alone.
* Comments within the AssemblyInfo are not migrated to the .csproj.
* If your AssemblyInfo.cs file has a syntax error, ai2csproj probably won't work.  Make sure your project compiles before running ai2csproj.
* When using a version source that copies from AssemblyVersion or FileVersion, the attribute's values are copied _exactly_, even if the value is invalid for the ```<Version>``` property.
* This tool doesn't work great with preprocessor defines.  Whatever attribute is excluded from the preprocessor will be removed completely when the tool is run.  Someday, it may be nice to somehow reconstruct the preprocessor logic in the .csproj, but that's not supported right now.
