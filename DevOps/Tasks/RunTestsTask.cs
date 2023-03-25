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

using Cake.ArgumentBinder;
using Cake.Frosting;
using Seth.CakeLib.TestRunner;

namespace DevOps.Tasks;
public class TestArguments
{
    // ---------------- Fields ----------------

    public static readonly string CoverageFilter = "+[Ai2Csproj]* -[Ai2CsProj.Tests]* -[Ai2CsProj.TestGenerator]*";

    // ---------------- Constructor ----------------

    public TestArguments()
    {
        this.RunWithCodeCoverage = false;
    }

    // ---------------- Properties ----------------

    [BooleanArgument(
        "code_coverage",
        Description = "Should we run this with code coverage?",
        DefaultValue = false
    )]
    public bool RunWithCodeCoverage { get; set; }
}

[TaskName( "run_tests" )]
[TaskDescription( "Runs all tests.  Pass in --code_coverage=true to run code coverage." )]
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

        TestArguments args = context.CreateFromArguments<TestArguments>();
        if( args.RunWithCodeCoverage )
        {
            runner.RunCodeCoverage( TestArguments.CoverageFilter );
        }
        else
        {
            runner.RunTests();
        }
    }
}
