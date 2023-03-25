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
using Cake.Frosting;
using Seth.CakeLib;

namespace DevOps;

internal class Program
{
    private static int Main( string[] args )
    {
        string exeDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) ?? string.Empty;
        string repoRoot = Path.Combine(
            exeDir, // app
            "..", // Debug
            "..", // Bin
            "..", // DevOps
            ".." // Src
        );

        return new CakeHost()
            .UseContext<BuildContext>()
            .SetToolPath( ".cake" )
            .InstallTool( new Uri( "nuget:?package=OpenCover&version=4.7.1221" ) )
            .InstallTool( new Uri( "nuget:?package=ReportGenerator&version=5.1.19" ) )
            .UseWorkingDirectory( repoRoot )
            .AddAssembly( SethCakeLib.GetAssembly() )
            .Run( args );
    }
}
