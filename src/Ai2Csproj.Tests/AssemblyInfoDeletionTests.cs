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

using System.Runtime.InteropServices;

namespace Ai2Csproj.Tests
{
    [TestClass]
    public sealed class AssemblyInfoDeletionTests
    {
        // ---------------- Fields ----------------

        private const string defaultAssemblyVersion = "1.2.3";

        private const string defaultOriginalCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
";

        private const string defaultExcpectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <PropertyGroup>
    <AssemblyVersion>{defaultAssemblyVersion}</AssemblyVersion>
  </PropertyGroup>

</Project>
";

        // ---------------- Tests ----------------

        [TestMethod]
        public void DontDeleteWithGlobalNamespaceTest()
        {
            // Setup
            const string originalAssemblyInfo =
$@"global using System.Reflection;

[assembly: AssemblyVersion( ""{defaultAssemblyVersion}"" )]
";
            const string expectedAssemblyInfo =
$@"global using System.Reflection;
";

            var config = new Ai2CsprojConfig
            {
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                defaultOriginalCsProj,
                originalAssemblyInfo,
                defaultExcpectedCsProj,
                expectedAssemblyInfo
            );
        }

        [TestMethod]
        public void DontDeleteWithClassDefinedTest()
        {
            // Setup
            const string originalAssemblyInfo =
$@"using System.Reflection;

[assembly: AssemblyVersion( ""{defaultAssemblyVersion}"" )]

public class MyClass
{{
}}
";
            const string expectedAssemblyInfo =
$@"using System.Reflection;

public class MyClass
{{
}}
";

            var config = new Ai2CsprojConfig
            {
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                defaultOriginalCsProj,
                originalAssemblyInfo,
                defaultExcpectedCsProj,
                expectedAssemblyInfo
            );
        }

        [TestMethod]
        public void DontDeleteWithStructDefinedTest()
        {
            // Setup
            const string originalAssemblyInfo =
$@"using System.Reflection;

[assembly: AssemblyVersion( ""{defaultAssemblyVersion}"" )]

public struct MyStruct
{{
}}
";
            const string expectedAssemblyInfo =
$@"using System.Reflection;

public struct MyStruct
{{
}}
";

            var config = new Ai2CsprojConfig
            {
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                defaultOriginalCsProj,
                originalAssemblyInfo,
                defaultExcpectedCsProj,
                expectedAssemblyInfo
            );
        }

        [TestMethod]
        public void DontDeleteWithEnumDefinedTest()
        {
            // Setup
            const string originalAssemblyInfo =
$@"using System.Reflection;

[assembly: AssemblyVersion( ""{defaultAssemblyVersion}"" )]

public enum MyEnum
{{
    SomeValue,
    SomeOtherValue
}}
";
            const string expectedAssemblyInfo =
$@"using System.Reflection;

public enum MyEnum
{{
    SomeValue,
    SomeOtherValue
}}
";

            var config = new Ai2CsprojConfig
            {
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                defaultOriginalCsProj,
                originalAssemblyInfo,
                defaultExcpectedCsProj,
                expectedAssemblyInfo
            );
        }

        [TestMethod]
        public void DontDeleteWithRecordDefinedTest()
        {
            // Setup
            const string originalAssemblyInfo =
$@"using System.Reflection;

[assembly: AssemblyVersion( ""{defaultAssemblyVersion}"" )]

public record MyRecord
{{
}}
";
            const string expectedAssemblyInfo =
$@"using System.Reflection;

public record MyRecord
{{
}}
";

            var config = new Ai2CsprojConfig
            {
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                defaultOriginalCsProj,
                originalAssemblyInfo,
                defaultExcpectedCsProj,
                expectedAssemblyInfo
            );
        }

        [TestMethod]
        public void DontDeleteWithExternDefinedTest()
        {
            // Setup
            const string originalAssemblyInfo =
$@"using System.Reflection;

[assembly: AssemblyVersion( ""{defaultAssemblyVersion}"" )]

static extern void DoSomething();
";
            const string expectedAssemblyInfo =
$@"using System.Reflection;

static extern void DoSomething();
";

            var config = new Ai2CsprojConfig
            {
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                defaultOriginalCsProj,
                originalAssemblyInfo,
                defaultExcpectedCsProj,
                expectedAssemblyInfo
            );
        }
    }
}
