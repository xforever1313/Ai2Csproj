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

using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps;

public class BuildContext : FrostingContext
{
    // ---------------- Constructor ----------------

    public BuildContext( ICakeContext context ) :
        base( context )
    {
        this.RepoRoot = context.Environment.WorkingDirectory;
        this.SrcDir = this.RepoRoot.Combine( "src" );
        this.Solution = this.SrcDir.CombineWithFilePath( "Ai2Csproj.sln" );
        this.ProjectPath = this.SrcDir.CombineWithFilePath( "Ai2Csproj/Ai2Csproj.csproj" );
        this.DistFolder = this.RepoRoot.Combine( "dist" );
        this.TestResultsFolder = this.RepoRoot.Combine( "TestResults" );
        this.TestCsProj = this.SrcDir.CombineWithFilePath( "Ai2Csproj.Tests/Ai2Csproj.Tests.csproj" );
    }

    // ---------------- Properties ----------------

    public DirectoryPath RepoRoot { get; private set; }

    public DirectoryPath SrcDir { get; private set; }

    public FilePath Solution { get; private set; }

    public FilePath ProjectPath { get; private set; }

    public DirectoryPath DistFolder { get; private set; }

    public DirectoryPath TestResultsFolder { get; private set; }

    public FilePath TestCsProj { get; private set; }
}