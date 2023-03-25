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

using Cake.Frosting;
using Seth.CakeLib.TestRunner;

namespace DevOps.Tasks;

[TaskName( "run_tests" )]
public sealed class RunTestsTask : DevopsTask
{
    public override void Run( BuildContext context )
    {
        var testConfig = new TestConfig
        {
            ResultsFolder = context.TestResultsFolder,
            TestCsProject = context.TestCsProj
        };

        var runner = new BaseTestRunner( context, testConfig, "Ai2Csproj.Tests" );
        runner.RunTests();
    }
}
