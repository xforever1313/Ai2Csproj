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
using Ai2Csproj.Exceptions;

namespace Ai2Csproj.Tests
{
    [TestClass]
    public sealed class VersionSourceTests
    {
        // ----------------- Fields ----------------

        private const SupportedAssemblyAttributes allVersionFlag =
            SupportedAssemblyAttributes.assembly_version |
            SupportedAssemblyAttributes.assembly_file_version |
            SupportedAssemblyAttributes.assembly_informational_version;

        private const string assemblyVersion = "1.2.3";

        private const string fileVersion = "4.5.6";

        private const string informationVersion = "7.8.9";

        private const string assemblyInfoWithAllAttributes =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyVersion( ""{assemblyVersion}"" )]
[assembly: AssemblyFileVersion( ""{fileVersion}"" )]
[assembly: AssemblyInformationalVersion( ""{informationVersion}"" )]
";

        private const string originalCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
";

        // ----------------- Tests ----------------

        // -------- Replace Tests --------

        [TestMethod]
        public void ReplaceWithNoVersionTest()
        {
            // Setup
            var config = new Ai2CsprojConfig
            {
                TypesToDelete = allVersionFlag,
                VersionSourceStrategy = VersionSource.exclude_version
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                originalCsProj,
                assemblyInfoWithAllAttributes,
                // Should be original, all versions are flagged to be deleted,
                // and no version tag should be added.
                originalCsProj,
                ""
            );
        }

        [TestMethod]
        public void ReplaceUsingAssemblyVersionTest()
        {
            DoReplaceTest( VersionSource.use_assembly_version, assemblyVersion );
        }

        [TestMethod]
        public void ReplaceUsingFileVersionTest()
        {
            DoReplaceTest( VersionSource.use_file_version, fileVersion );
        }

        [TestMethod]
        public void ReplaceUsingInformationVersionTest()
        {
            DoReplaceTest( VersionSource.use_informational_version, informationVersion );
        }

        // -------- Missing Attribute --------

        [TestMethod]
        public void MissingAssemblyVersionTest()
        {
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyFileVersion( ""{fileVersion}"" )]
[assembly: AssemblyInformationalVersion( ""{informationVersion}"" )]
";
            DoMissingAttributeTest(
                VersionSource.use_assembly_version,
                typeof( AssemblyVersionAttribute ),
                originalCsProj,
                originalAssemblyInfo
            );
        }

        [TestMethod]
        public void MissingFileVersionTest()
        {
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyVersion( ""{assemblyVersion}"" )]
[assembly: AssemblyInformationalVersion( ""{informationVersion}"" )]
";
            DoMissingAttributeTest(
                VersionSource.use_file_version,
                typeof( AssemblyFileVersionAttribute ),
                originalCsProj,
                originalAssemblyInfo
            );
        }

        [TestMethod]
        public void MissingInformationVersionTest()
        {
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyVersion( ""{assemblyVersion}"" )]
[assembly: AssemblyFileVersion( ""{fileVersion}"" )]
";
            DoMissingAttributeTest(
                VersionSource.use_informational_version,
                typeof( AssemblyInformationalVersionAttribute ),
                originalCsProj,
                originalAssemblyInfo
            );
        }

        // ---------------- Test Helpers ----------------

        private static void DoReplaceTest( VersionSource versionSource, string versionString )
        {
            // Setup
            string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <PropertyGroup>
    <Version>{versionString}</Version>
  </PropertyGroup>
</Project>
";

            var config = new Ai2CsprojConfig
            {
                TypesToDelete = allVersionFlag,
                VersionSourceStrategy = versionSource
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                originalCsProj,
                assemblyInfoWithAllAttributes,
                expectedCsProj,
                ""
            );
        }

        private static void DoMissingAttributeTest(
            VersionSource versionSource,
            Type expectedType,
            string originalCsProj,
            string originalAssemblyInfo
        )
        {
            var config = new Ai2CsprojConfig
            {
                TypesToDelete = allVersionFlag,
                VersionSourceStrategy = versionSource
            };

            var uut = new Migrator( config );
            var e = Assert.ThrowsException<MissingVersionSourceException>(
                () => uut.Migrate( originalCsProj, originalAssemblyInfo )
            );

            Assert.AreEqual( versionSource, e.ExpectedVersionSource );
            Assert.AreEqual( expectedType, e.MissingAttributeType );
        }
    }
}
