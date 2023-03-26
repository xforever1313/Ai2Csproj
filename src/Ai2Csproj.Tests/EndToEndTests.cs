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
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

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

        private const string defaultCustom = "My Custom Value";

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

        private static readonly string defaultOriginalAssemblyInfoWithUnsupportedType =
            defaultOriginalAssemblyInfo +
            $@"[assembly: NameSpace.CustomAttribute( ""{defaultCustom}"" )]" +
            Environment.NewLine;

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
        /// Default settings include:
        /// - Everything is migrated.
        /// - Version source is "do nothing".
        /// - Backups are enabled.
        /// - Dry run is disabled.
        /// - AssemblyInfo is not deleted.
        /// </summary>
        [TestMethod]
        public void TestWithDefaultSettingsWithUnsupportedType()
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
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
[assembly: NameSpace.CustomAttribute( ""{defaultCustom}"" )]
";

            var testConfig = new TestConfig( nameof( TestWithDefaultSettings ) )
            {
                Arguments = new string[]
                {
                },
                ExpectBackups = true,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = defaultOriginalAssemblyInfoWithUnsupportedType,
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
        /// - AssemblyInfo should be attempted to be deleted, and should
        ///   be since nothing is left.
        /// </summary>
        [TestMethod]
        public void TestWithUnsupportedAssemblyEnabled()
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
    <AssemblyAttribute Include=""NameSpace.CustomAttribute"">
        <_Parameter1>{defaultCustom}</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>
";

            const string? expectedAssemblyInfo = null;

            var testConfig = new TestConfig( nameof( TestWithUnsupportedAssemblyEnabled ) )
            {
                Arguments = new string[]
                {
                    "--migrate_unsupported_types",
                    "--delete_old_assembly_info"
                },
                ExpectBackups = true,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = defaultOriginalAssemblyInfoWithUnsupportedType,
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
        /// - AssemblyInfo should be attempted to be deleted, and should
        ///   not be since unsupported types are not bein migrated.
        /// </summary>
        [TestMethod]
        public void TestWithUnsupportedTypesDisabled()
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
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
[assembly: NameSpace.CustomAttribute( ""{defaultCustom}"" )]
";

            var testConfig = new TestConfig( nameof( TestWithUnsupportedTypesDisabled ) )
            {
                Arguments = new string[]
                {
                    "--delete_old_assembly_info"
                },
                ExpectBackups = true,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = defaultOriginalAssemblyInfoWithUnsupportedType,
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

            var testConfig = new TestConfig( nameof( TestWithBackupsDisabled ) )
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
        /// If the properties folder is empty,
        /// it should no longer exist.
        /// </summary>
        [TestMethod]
        public void TestDeletePropertiesFolder()
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

            const string? expectedAssemblyInfo = null;

            var testConfig = new TestConfig( nameof( TestDeletePropertiesFolder ) )
            {
                Arguments = new string[]
                {
                    "--no_backup",
                    "--delete_old_assembly_info"
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
        /// If the properties folder is empty,
        /// it should no longer exist.
        /// </summary>
        [TestMethod]
        public void DontDeletePropertiesFolderIfNotEmptyTest()
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

            const string? expectedAssemblyInfo = null;

            var testConfig = new TestConfig( nameof( DontDeletePropertiesFolderIfNotEmptyTest ) )
            {
                Arguments = new string[]
                {
                    "--no_backup",
                    "--delete_old_assembly_info"
                },
                ExpectBackups = false,
                WriteExtraFileInPropertiesFolder = true,
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
        /// - Backups are disabled.
        /// - Dry run is disabled.
        /// - AssemblyInfo should be deleted if its safe.
        /// </summary>
        [TestMethod]
        public void TestWithAssemblyInfoDeleteOn()
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

            const string? expectedAssemblyInfo = null;

            var testConfig = new TestConfig( nameof( TestWithAssemblyInfoDeleteOn ) )
            {
                Arguments = new string[]
                {
                    "--delete_old_assembly_info"
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

        [TestMethod]
        public void TestWithPreprocessorDefined()
        {
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyCopyright( ""{defaultCopyRight}"" )]
#else
[assembly: AssemblyCopyright( ""WRONG COPYRIGHT"" )]
#endif

#if NET5_0
[assembly: AssemblyVersion( ""WRONG VERSION"" )]
#elseif NET6_0_OR_GREATER
[assembly: AssemblyVersion( ""{defaultAssemblyVersion}"" )]
#else
[assembly: AssemblyVersion( ""STILL WRONG"" )]
#endif

#if NOT_DEFINED
[assembly: AssemblyTrademark( ""WRONG TRADEMARK"" )]
#else
[assembly: AssemblyTrademark( ""{defaultTrademark}"" )]
#endif
";

            const string expectedCsProj =
$@" <Project Sdk=""Microsoft.NET.Sdk"">

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

            string? expectedAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG
#else
[assembly: AssemblyCopyright( ""WRONG COPYRIGHT"" )]
#endif

#if NET5_0
[assembly: AssemblyVersion( ""WRONG VERSION"" )]
#elseif NET6_0_OR_GREATER
#else
[assembly: AssemblyVersion( ""STILL WRONG"" )]
#endif

#if NOT_DEFINED
[assembly: AssemblyTrademark( ""WRONG TRADEMARK"" )]
#else
#endif
";
            // ^That is what we would expect in the ideal situation,
            // but the parser currently isn't smart enough
            // to handle that due to preprocessor stuff.
            // The end result is the assembly info being deleted completely.
            // Just something to mark in the limitation section.
            // Even something better we can do is add preprocessor stuff
            // to the csproj... somehow...
            expectedAssemblyInfo = null;
            var testConfig = new TestConfig( nameof( TestWithPreprocessorDefined ) )
            {
                Arguments = new string[]
                {
                    "--delete_old_assembly_info",
                    "--define=NET6_0_OR_GREATER",
                    "--define=DEBUG"
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

        [TestMethod]
        public void DoMultipleAttributesTest()
        {
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCopyright( ""{defaultCopyRight}"" )]
[assembly: AssemblyCopyright( ""ANOTHER COPYRIGHT!"" )]
";

            var testConfig = new TestConfig( nameof( DoMultipleAttributesTest ) )
            {
                // State should not change, backups should not be created.
                ExpectBackups = false,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = originalAssemblyInfo,

                // State should not change, expected should match original.
                ExpectedCsProj = defaultOriginalCsProj,
                ExpectedAssemblyInfo = originalAssemblyInfo,
                ExpectedExitCode = Program.SyntaxParsingErrorExitCode
            };

            RunTest( testConfig );
        }

        [TestMethod]
        public void DoMultipleArgumentsTest()
        {
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCopyright( ""{defaultCopyRight}"", ""Incorrect Second Argument"" )]
";

            var testConfig = new TestConfig( nameof( DoMultipleArgumentsTest ) )
            {
                // State should not change, backups should not be created.
                ExpectBackups = false,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = originalAssemblyInfo,

                // State should not change, expected should match original.
                ExpectedCsProj = defaultOriginalCsProj,
                ExpectedAssemblyInfo = originalAssemblyInfo,
                ExpectedExitCode = Program.SyntaxParsingErrorExitCode
            };

            RunTest( testConfig );
        }

        [TestMethod]
        public void DoMissingArgumentsTest()
        {
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCopyright()]
";

            var testConfig = new TestConfig( nameof( DoMissingArgumentsTest ) )
            {
                // State should not change, backups should not be created.
                ExpectBackups = false,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = originalAssemblyInfo,

                // State should not change, expected should match original.
                ExpectedCsProj = defaultOriginalCsProj,
                ExpectedAssemblyInfo = originalAssemblyInfo,
                ExpectedExitCode = Program.SyntaxParsingErrorExitCode
            };

            RunTest( testConfig );
        }

        [TestMethod]
        public void DoMissingArgumentsForAssemblyKeySignatureTest()
        {
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblySignatureKeyAttribute( ""value1"" )]
";

            var testConfig = new TestConfig( nameof( DoMissingArgumentsTest ) )
            {
                // State should not change, backups should not be created.
                ExpectBackups = false,
                OriginalCsProj = defaultOriginalCsProj,
                OriginalAssemblyInfo = originalAssemblyInfo,

                // State should not change, expected should match original.
                ExpectedCsProj = defaultOriginalCsProj,
                ExpectedAssemblyInfo = originalAssemblyInfo,
                ExpectedExitCode = Program.SyntaxParsingErrorExitCode
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

            DoArgumentsDoNotChangeStateTest( args, nameof( TestWithDryRunEnabled ) );
        }

        /// <summary>
        /// Settings include:
        /// - Dry run is enabled.
        /// </summary>
        [TestMethod]
        public void TestWithDryRunAndDeleteAssemblyInfoEnabled()
        {
            var args = new string[]
            {
                "--dry_run",
                "--delete_old_assembly_info"
            };

            DoArgumentsDoNotChangeStateTest( args, nameof( TestWithDryRunAndDeleteAssemblyInfoEnabled ) );
        }

        [TestMethod]
        public void TestWithHelpArgument()
        {
            var args = new string[]
            {
                "--help"
            };

            DoArgumentsDoNotChangeStateTest( args, nameof( TestWithHelpArgument ) );
        }

        [TestMethod]
        public void TestWithVersionArgument()
        {
            var args = new string[]
            {
                "--version"
            };

            DoArgumentsDoNotChangeStateTest( args, nameof( TestWithVersionArgument ) );
        }

        [TestMethod]
        public void TestWithPrintCreditsArgument()
        {
            var args = new string[]
            {
                "--show_credits"
            };

            DoArgumentsDoNotChangeStateTest( args, nameof( TestWithPrintCreditsArgument ) );
        }

        [TestMethod]
        public void TestWithPrintLicenseArgument()
        {
            var args = new string[]
            {
                "--show_license"
            };

            DoArgumentsDoNotChangeStateTest( args, nameof( TestWithPrintLicenseArgument ) );
        }

        [TestMethod]
        public void TestWithPrintReadmeArgument()
        {
            var args = new string[]
            {
                "--show_readme"
            };

            DoArgumentsDoNotChangeStateTest( args, nameof( TestWithPrintReadmeArgument ) );
        }

        [TestMethod]
        public void TestWithPrintSourceArgument()
        {
            var args = new string[]
            {
                "--show_source"
            };

            DoArgumentsDoNotChangeStateTest( args, nameof( TestWithPrintSourceArgument ) );
        }

        [TestMethod]
        public void TestWithListSupportedTypesArgument()
        {
            var args = new string[]
            {
                "--show_supported_types"
            };

            DoArgumentsDoNotChangeStateTest( args, nameof( TestWithListSupportedTypesArgument ) );
        }

        [TestMethod]
        public void MissingCsProjTest()
        {
            // Setup
            var args = new string[]
            {
                $"--project_path=does_not_exist.csproj",
                // Cheat a little bit, the validation logic doesn't look
                // inside, it just checks to see if it exists.
                $"--assembly_info_path={typeof( EndToEndTests ).Assembly.Location}"
            };

            // Act
            int exitCode = Program.Main( args );

            // Check
            Assert.AreEqual( Program.OptionsValidationErrorExitCode, exitCode );
        }

        [TestMethod]
        public void MissingAssemblyInfoTest()
        {
            // Setup
            var args = new string[]
            {
                // Cheat a little bit, the validation logic doesn't look
                // inside, it just checks to see if it exists.
                $"--project_path={typeof( EndToEndTests ).Assembly.Location}",
                $"--assembly_info_path=does_not_exist.cs"
            };

            // Act
            int exitCode = Program.Main( args );

            // Check
            Assert.AreEqual( Program.OptionsValidationErrorExitCode, exitCode );
        }

        [TestMethod]
        public void InvalidBehaviorOption()
        {
            // Setup
            var args = new string[]
            {
                "--assembly_company_behavior=invalid"
            };

            // Act
            int exitCode = Program.Main( args );

            // Check
            Assert.AreEqual( Program.CliArgsErrorExitCode, exitCode );
        }

        [TestMethod]
        public void InvalidVersionSourceOption()
        {
            // Setup
            var args = new string[]
            {
                "--version_source=invalid"
            };

            // Act
            int exitCode = Program.Main( args );

            // Check
            Assert.AreEqual( Program.CliArgsErrorExitCode, exitCode );
        }

        // ---------------- Test Helpers ----------------

        private static void DoArgumentsDoNotChangeStateTest( string[] args, string testName )
        {
            var testConfig = new TestConfig( testName )
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
            var propertiesDirectory = new DirectoryInfo( Path.Combine( directory.FullName, "Properties" ) );

            try
            {
                directory.Refresh();
                if( directory.Exists )
                {
                    Directory.Delete( directory.FullName, true );
                }

                Directory.CreateDirectory( directory.FullName );
                Directory.CreateDirectory( propertiesDirectory.FullName );
                FileInfo csProj = new FileInfo( Path.Combine( directory.FullName, $"{config.TestName}.csproj" ) );
                FileInfo assemblyInfo = new FileInfo( Path.Combine( propertiesDirectory.FullName, "AssemblyInfo.cs" ) );

                if( config.WriteExtraFileInPropertiesFolder )
                {
                    File.WriteAllText(
                        Path.Combine( propertiesDirectory.FullName, "Hello.txt" ),
                        "Hello"
                    );
                }

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

                    // We should delete the Properties folder if it is empty.
                    if(
                        ( config.WriteExtraFileInPropertiesFolder == false ) &&
                        ( config.ExpectBackups == false )
                    )
                    {
                        Assert.IsFalse( propertiesDirectory.Exists );
                    }
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

            public bool WriteExtraFileInPropertiesFolder { get; init; } = false;

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
