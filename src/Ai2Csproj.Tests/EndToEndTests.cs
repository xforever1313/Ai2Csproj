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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ai2Csproj.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class EndToEndTests
    {
        // ---------------- Fields ----------------

        private const string defaultCopyRight = "Copyright (C) Seth Hendrick 2023";

        private const string defaultAssemblyVersion = "1.2.3";

        private const string defaultTrademark = "My Trademark";

        private const string defaultOriginalCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
";

        private const string defaultOriginalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCopyright( ""{defaultCopyRight}"" )]
[assembly: AssemblyVersion( ""{defaultAssemblyVersion}"" )]
[assembly: AssemblyTrademark( ""{defaultTrademark}"" )]
";

        // ---------------- Tests ----------------

        /// <summary>
        /// Default settings include:
        /// - Everything is migrated.
        /// - Version source is "do nothing".
        /// - Backups are enabled.
        /// - Dry run is disabled.
        /// - AssemblyInfo is not deleted.
        /// </summary>
        [TestMethod]
        public void TestWithDefaultSettings()
        {
            const string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <PropertyGroup>
    <Copyright>{defaultCopyRight}</Copyright>
    <AssemblyVersion>{defaultAssemblyVersion}</AssemblyVersion>
  </PropertyGroup>

  <ItemGroup>
    <AssemblyAttribute Include=""System.Reflection.AssemblyTrademarkAttribute"">
        <_Parameter1>{defaultTrademark}</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>
";

            const string expectedAssemblyInfo =
@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
";

            var testConfig = new TestConfig( nameof( TestWithDefaultSettings ) )
            {
                Arguments = new string[]
                {
                },
                ExpectBackups = true,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = defaultOriginalAssemblyInfo,
                ExpectedCsProj = expectedCsProj,
                ExpectedAssemblyInfo = expectedAssemblyInfo,
                ExpectedExitCode = 0
            };

            RunTest( testConfig );
        }

        /// <summary>
        /// Settings include:
        /// - Everything is migrated.
        /// - Version source is "do nothing".
        /// - Backups are enabled.
        /// - Dry run is disabled.
        /// - AssemblyInfo is not deleted.
        /// </summary>
        [TestMethod]
        public void TestWithBackupsDisabled()
        {
            const string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <PropertyGroup>
    <Copyright>{defaultCopyRight}</Copyright>
    <AssemblyVersion>{defaultAssemblyVersion}</AssemblyVersion>
  </PropertyGroup>

  <ItemGroup>
    <AssemblyAttribute Include=""System.Reflection.AssemblyTrademarkAttribute"">
        <_Parameter1>{defaultTrademark}</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>
";

            const string expectedAssemblyInfo =
@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
";

            var testConfig = new TestConfig( nameof( TestWithDefaultSettings ) )
            {
                Arguments = new string[]
                {
                    "--no_backup"
                },
                ExpectBackups = false,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = defaultOriginalAssemblyInfo,
                ExpectedCsProj = expectedCsProj,
                ExpectedAssemblyInfo = expectedAssemblyInfo,
                ExpectedExitCode = 0
            };

            RunTest( testConfig );
        }

        /// <summary>
        /// Settings include:
        /// - Dry run is enabled.
        /// </summary>
        [TestMethod]
        public void TestWithDryRunEnabled()
        {
            var args = new string[]
            {
                "--dry_run"
            };

            DoArgumentsDoNotChangeStateTest( args );
        }

        [TestMethod]
        public void TestWithHelpArgument()
        {
            var args = new string[]
            {
                "--help"
            };

            DoArgumentsDoNotChangeStateTest( args );
        }

        [TestMethod]
        public void TestWithVersionArgument()
        {
            var args = new string[]
            {
                "--version"
            };

            DoArgumentsDoNotChangeStateTest( args );
        }

        [TestMethod]
        public void TestWithPrintCreditsArgument()
        {
            var args = new string[]
            {
                "--print_credits"
            };

            DoArgumentsDoNotChangeStateTest( args );
        }

        [TestMethod]
        public void TestWithPrintLicenseArgument()
        {
            var args = new string[]
            {
                "--print_license"
            };

            DoArgumentsDoNotChangeStateTest( args );
        }

        [TestMethod]
        public void TestWithListSupportedTypesArgument()
        {
            var args = new string[]
            {
                "--list_supported_types"
            };

            DoArgumentsDoNotChangeStateTest( args );
        }

        // ---------------- Test Helpers ----------------

        private static void DoArgumentsDoNotChangeStateTest( string[] args )
        {
            var testConfig = new TestConfig( nameof( TestWithDefaultSettings ) )
            {
                Arguments = args,
                // State should not change, backups should not be created.
                ExpectBackups = false,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = defaultOriginalAssemblyInfo,

                // State should not change, expected should match original.
                ExpectedCsProj = defaultOriginalCsProj,
                ExpectedAssemblyInfo = defaultOriginalAssemblyInfo,
                ExpectedExitCode = 0
            };

            RunTest( testConfig );
        }

        private static void RunTest( TestConfig config )
        {
            // Setup
            string? currentDirectory = Path.GetDirectoryName( typeof( EndToEndTests ).Assembly.Location );
            Assert.IsNotNull( currentDirectory );
            var directory = new DirectoryInfo(
                Path.Combine( currentDirectory, config.TestName )
            );

            try
            {
                directory.Refresh();
                if( directory.Exists )
                {
                    Directory.Delete( directory.FullName, true );
                }

                Directory.CreateDirectory( directory.FullName );
                Directory.CreateDirectory( Path.Combine( directory.FullName, "Properties" ) );
                FileInfo csProj = new FileInfo( Path.Combine( directory.FullName, $"{config.TestName}.csproj" ) );
                FileInfo assemblyInfo = new FileInfo( Path.Combine( directory.FullName, "Properties", "AssemblyInfo.cs" ) );

                File.WriteAllText( csProj.FullName, config.OriginalCsProj );
                File.WriteAllText( assemblyInfo.FullName, config.OriginalAssemblyInfo );

                var arguments = new List<string>( config.Arguments )
                {
                    $@"--project_path={csProj.FullName}",
                    $@"--assembly_info_path={assemblyInfo.FullName}"
                };

                // Act
                int exitCode = Program.Main( arguments.ToArray() );

                // Check
                Assert.AreEqual( config.ExpectedExitCode, exitCode );

                FileInfo backupCsProj = new FileInfo( $"{csProj.FullName}.old" );
                FileInfo backupAssemblyInfo = new FileInfo( $"{assemblyInfo.FullName}.old" );
                if( config.ExpectBackups )
                {
                    Assert.IsTrue( backupCsProj.Exists );
                    Assert.AreEqual( config.OriginalCsProj, File.ReadAllText( backupCsProj.FullName ) );

                    Assert.IsTrue( backupAssemblyInfo.Exists );
                    Assert.AreEqual( config.OriginalAssemblyInfo, File.ReadAllText( backupAssemblyInfo.FullName ) );
                }
                else
                {
                    Assert.IsFalse( backupCsProj.Exists );
                    Assert.IsFalse( backupAssemblyInfo.Exists );
                }

                XDocument expectedCsProjXml = XDocument.Parse( config.ExpectedCsProj );
                XDocument actualCsProjXml = XDocument.Load( csProj.FullName );
                Assert.AreEqual( expectedCsProjXml.ToString(), actualCsProjXml.ToString() );

                if( config.ExpectedAssemblyInfo is null )
                {
                    Assert.IsFalse( assemblyInfo.Exists );
                }
                else
                {
                    Assert.AreEqual( config.ExpectedAssemblyInfo, File.ReadAllText( assemblyInfo.FullName ) );
                }
            }
            finally
            {
                try
                {
                    directory.Refresh();
                    if( directory.Exists )
                    {
                        Directory.Delete( directory.FullName, true );
                    }
                }
                catch( IOException )
                {
                }
            }
        }

        // ---------------- Helper Classes ----------------

        private class TestConfig
        {
            // ---------------- Constructor ----------------

            public TestConfig( string testName )
            {
                this.TestName = testName;
            }

            // ---------------- Properties ----------------

            public string TestName { get; private set; }

            public bool ExpectBackups { get; init; } = true;

            public string OriginalCsProj { get; init; } = "";

            public string OriginalAssemblyInfo { get; init; } = "";

            public string ExpectedCsProj { get; init; } = "";

            /// <summary>
            /// Set to null to expected a deleted assembly info file.
            /// </summary>
            public string? ExpectedAssemblyInfo { get; init; } = null;

            /// <remarks>
            /// Do not include project path or assembly info path,
            /// those are created automatically when
            /// passed into the test.
            /// </remarks>
            public string[] Arguments { get; init; } = Array.Empty<string>();

            public int ExpectedExitCode { get; init; } = 0;
        }
    }
}
