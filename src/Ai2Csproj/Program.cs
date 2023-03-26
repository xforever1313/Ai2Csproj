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

using Ai2Csproj;
using Mono.Options;
using SethCS.Exceptions;
using SethCS.IO;

internal class Program
{
    // ---------------- Fields ----------------

    internal const int SuccessExitCode = 0;

    internal const int CliArgsErrorExitCode = 1;

    internal const int SyntaxParsingErrorExitCode = 5;

    internal const int OptionsValidationErrorExitCode = 2;

    internal const int FatalErrorExitCode = 0;

    // ---------------- Functions ----------------

    internal static int Main( string[] args )
    {
        try
        {
            Ai2CsprojConfig config;
            {
                var argParser = new ArgumentParser();
                argParser.Parse( args );

                if( argParser.ShowHelp )
                {
                    argParser.WriteHelp( Console.Out );
                    return SuccessExitCode;
                }
                else if( argParser.ShowCredits )
                {
                    PrintCredits();
                    return SuccessExitCode;
                }
                else if( argParser.ShowLicense )
                {
                    PrintLicense();
                    return SuccessExitCode;
                }
                else if( argParser.ShowVersion )
                {
                    PrintVersion();
                    return SuccessExitCode;
                }
                else if( argParser.ShowSupportedTypes )
                {
                    PrintSupportedTypes();
                    return SuccessExitCode;
                }

                config = argParser.Config;
            }

            Run( config );

            return SuccessExitCode;
        }
        catch( OptionException e )
        {
            Console.WriteLine( "Error when parsing options:" );
            Console.WriteLine( e.Message );
            return CliArgsErrorExitCode;
        }
        catch( SyntaxTreeParseException e )
        {
            Console.WriteLine( e.Message );
            return SyntaxParsingErrorExitCode;
        }
        catch( ListedValidationException e )
        {
            Console.WriteLine( e.Message );
            return OptionsValidationErrorExitCode;
        }
        catch( Exception e )
        {
            Console.WriteLine( "FATAL ERROR:" );
            Console.WriteLine( e );
            return FatalErrorExitCode;
        }
    }

    private static void Run( Ai2CsprojConfig config )
    {
        config.Validate();

        var migrator = new Migrator( config );
        MigrationResult result = migrator.Migrate();
        migrator.WriteFiles( result );
    }

    private static void PrintVersion()
    {
        Console.WriteLine(
            typeof( Program ).Assembly.GetName().Version?.ToString( 3 ) ?? "Unknown Version"
        );
    }

    private static void PrintSupportedTypes()
    {
        Console.WriteLine( "Supported Types:" );
        foreach( Type type in AssemblyAttributeMapping.GetSupportedTypes() )
        {
            Console.WriteLine( $"\t- {type.FullName}" );
        }
        Console.WriteLine();
        Console.WriteLine( "Note: This doesn't mean unsupported assembly attributes can't be migrated." );
        Console.WriteLine( "By specifying '--migrate_unsupported_types', an *attempt* can be made to migrate them and add to the csproj." );
    }

    private static void PrintLicense()
    {
        string text = AssemblyResourceReader.ReadStringResource(
            typeof( Program ).Assembly, $"{nameof( Ai2Csproj )}.License.md"
        );

        Console.WriteLine( text );
    }

    private static void PrintCredits()
    {
        string text = AssemblyResourceReader.ReadStringResource(
            typeof( Program ).Assembly, $"{nameof( Ai2Csproj )}.Credits.md"
        );

        Console.WriteLine( text );
    }
}
