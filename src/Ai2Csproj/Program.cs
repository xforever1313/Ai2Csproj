﻿// Ai2Csproj - A program that migrates AssemblyInfo files to csproj.
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

internal class Program
{
    private static int Main( string[] args )
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
                    return 0;
                }
                else if( argParser.ShowCredits )
                {
                    PrintCredits();
                    return 0;
                }
                else if( argParser.ShowLicense )
                {
                    PrintLicense();
                    return 0;
                }
                else if( argParser.ShowVersion )
                {
                    PrintVersion();
                    return 0;
                }

                config = argParser.Config;
            }

            Run( config );

            return 0;
        }
        catch( OptionException e )
        {
            Console.WriteLine( "Error when parsing options:" );
            Console.WriteLine( e );
            return 1;
        }
        catch( ListedValidationException e )
        {
            Console.WriteLine( "Error when validating options:" );
            Console.WriteLine( e );
            return 2;
        }
        catch( Exception e )
        {
            Console.WriteLine( "FATAL ERROR:" );
            Console.WriteLine( e );
            return -1;
        }
    }

    private static void Run( Ai2CsprojConfig config )
    {
        config.Validate();
    }

    private static void PrintVersion()
    {
        Console.WriteLine(
            typeof( Program ).Assembly.GetName().Version?.ToString( 3 ) ?? "Unknown Version"
        );
    }

    private static void PrintLicense()
    {
    }

    private static void PrintCredits()
    {
    }
}