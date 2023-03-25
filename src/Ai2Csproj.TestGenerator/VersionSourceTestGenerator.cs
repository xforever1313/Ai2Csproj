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
    public sealed class VersionSourceTestGenerator : ISourceGenerator
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

    // -------- Versioning Source Tests --------
"
            );

            foreach( VersionSource versionSource in Enum.GetValues( typeof( VersionSource ) ) )
            {
                sourceBuilder.AppendLine(
$@"
    [TestMethod]
    public void TestVersionSource_{versionSource}()
    {{
        string[] args = new string[] {{ ""--version_source={versionSource}"" }};

        var expectedConfig = new Ai2CsprojConfig
        {{
            VersionSourceStrategy = {nameof(VersionSource)}.{versionSource}
        }};

        DoArgumentParsingToConfigTest( args, expectedConfig );
    }}
"
                );
            }

            sourceBuilder.AppendLine( "}" );
            context.AddSource( "ArgumentParsingTests.VersionSourceTests", sourceBuilder.ToString() );
        }
    }
}
