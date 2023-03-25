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

using System;
using System.Text;
using Ai2Csproj.Shared;
using Microsoft.CodeAnalysis;

namespace Ai2Csproj.TestGenerator
{
    [Generator]
    public class BehaviorArgumentParsingTestGenerator : ISourceGenerator
    {
        public void Initialize( GeneratorInitializationContext context )
        {
        }

        public void Execute( GeneratorExecutionContext context )
        {
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine(
@"
namespace Ai2Csproj.Tests;

public sealed partial class ArgumentParsingTests
{
    // ---------------- Tests ----------------

    // -------- Behavior Tests --------
"
            );

            foreach( SupportedAssemblyAttributes supportedAttribute in Enum.GetValues( typeof( SupportedAssemblyAttributes ) ) )
            {
                sourceBuilder.AppendLine(
$@"
    [TestMethod]
    public void TestMigrateFlag_{supportedAttribute}()
    {{
        // Setup
        string[] args = new string[] {{ ""--{supportedAttribute}_behavior=migrate"" }};

        var expectedConfig = new Ai2CsprojConfig
        {{
            TypesToMigrate = {nameof( SupportedAssemblyAttributes )}.{supportedAttribute}
        }};

        DoArgumentParsingToConfigTest( args, expectedConfig );
    }}

    [TestMethod]
    public void TestDeleteFlag_{supportedAttribute}()
    {{
        string[] args = new string[] {{ ""--{supportedAttribute}_behavior=delete"" }};

        var expectedConfig = new Ai2CsprojConfig
        {{
            TypesToDelete = {nameof( SupportedAssemblyAttributes )}.{supportedAttribute}
        }};

        DoArgumentParsingToConfigTest( args, expectedConfig );
    }}

    [TestMethod]
    public void TestLeaveFlag_{supportedAttribute}()
    {{
        string[] args = new string[] {{ ""--{supportedAttribute}_behavior=leave"" }};

        var expectedConfig = new Ai2CsprojConfig
        {{
            TypesToLeave = {nameof( SupportedAssemblyAttributes )}.{supportedAttribute}
        }};

        DoArgumentParsingToConfigTest( args, expectedConfig );
    }}
"
                );
            }

            sourceBuilder.AppendLine( "}" );
            context.AddSource( "ArgumentParsingTests.BehaviorParsingTests", sourceBuilder.ToString() );
        }
    }
}
