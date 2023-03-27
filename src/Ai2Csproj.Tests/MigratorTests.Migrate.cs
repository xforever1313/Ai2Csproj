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

using System.Collections.Immutable;

namespace Ai2Csproj.Tests
{
    public sealed partial class MigratorTests
    {
        // ---------------- Fields ----------------

        private static readonly string everythingMigratedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <PropertyGroup>
    <Company>{defaultCompany}</Company>
    <Configuration>{defaultConfiguration}</Configuration>
    <Copyright>{defaultCopyRight}</Copyright>
    <Description>{defaultDescription}</Description>
    <FileVersion>{defaultFileVersion}</FileVersion>
    <InformationalVersion>{defaultInfoVersion}</InformationalVersion>
    <Product>{defaultProduct}</Product>
    <AssemblyTitle>{defaultTitle}</AssemblyTitle>
    <AssemblyVersion>{defaultVersion}</AssemblyVersion>
    <NeutralLanguage>{defaultLanguage}</NeutralLanguage>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Microsoft.CodeAnalysis.CSharp"" />
    <PackageReference Include=""Mono.Options"" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include=""..\..\SethCS\LibSethCS\LibSethCS.csproj"" />
  </ItemGroup>
  <ItemGroup>
    <AssemblyAttribute Include=""System.Reflection.AssemblyTrademarkAttribute"">
        <_Parameter1>{defaultTrademark}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Runtime.CompilerServices.InternalsVisibleToAttribute"">
        <_Parameter1>{defaultInternals1}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Runtime.CompilerServices.InternalsVisibleToAttribute"">
        <_Parameter1>{defaultInternals2}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Runtime.InteropServices.ComVisibleAttribute"">
        <_Parameter1>{defaultComVisible}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.CLSCompliantAttribute"">
        <_Parameter1>{defaultCls}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Runtime.InteropServices.GuidAttribute"">
        <_Parameter1>{defaultGuid}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Reflection.AssemblyKeyFileAttribute"">
        <_Parameter1>{defaultKeyFile}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Reflection.AssemblyKeyNameAttribute"">
        <_Parameter1>{defaultKeyName}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Reflection.AssemblySignatureKeyAttribute"">
        <_Parameter1>{defaultPublicKey}</_Parameter1>
        <_Parameter2>{defaultCounterSignature}</_Parameter2>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Reflection.AssemblyCultureAttribute"">
        <_Parameter1>{defaultCulture}</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>";

        // ---------------- Tests  ----------------

       [TestMethod]
        public void MigrateAllAttributesTest()
        {
            // Setup
            string expectedCsProj = everythingMigratedCsProj;

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                defaultStartingCsProj,
                defaultStartingAssemblyInfo,
                expectedCsProj,
                ""
            );
        }
    
        [TestMethod]
        public void MigrateAllWithNoNamespaceWithSuffixInAssemlbyInfoTest()
        {
            // Setup
            string expectedCsProj = everythingMigratedCsProj;

            var config = GetConfigWithAllMigrated( null );
            config = config with
            {
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                defaultStartingCsProj,
                GetDefaultStartingAssemblyInfo( true, false ),
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void MigrateAllWithNamespaceWithSuffixInAssemlbyInfoTest()
        {
            // Setup
            string expectedCsProj = everythingMigratedCsProj;

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                defaultStartingCsProj,
                GetDefaultStartingAssemblyInfo( true, true ),
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void MigrateAllWithNamespaceWithNoSuffixInAssemlbyInfoTest()
        {
            // Setup
            string expectedCsProj = everythingMigratedCsProj;

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                defaultStartingCsProj,
                GetDefaultStartingAssemblyInfo( false, true ),
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void MigratePropertyGroupToEmptyCsProjTest()
        {
            // Setup
            const string originalCsProj = 
@"<Project Sdk=""Microsoft.NET.Sdk"">
</Project>
";
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany( ""{defaultCompany}"" )]
[assembly: AssemblyConfiguration( ""{defaultConfiguration}"" )]
[assembly: AssemblyCopyright( ""{defaultCopyRight}"" )]
";

            const string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <Company>{defaultCompany}</Company>
    <Configuration>{defaultConfiguration}</Configuration>
    <Copyright>{defaultCopyRight}</Copyright>
  </PropertyGroup>
</Project>";

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void MigrateWithSpecialStringTypesInAssemblyInfoTest()
        {
            // Setup
            const string originalCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">
</Project>
";
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany( @""{defaultCompany}"" )]
[assembly: AssemblyConfiguration( $""{defaultConfiguration}"" )]
[assembly: AssemblyCopyright( $@""{defaultCopyRight}"" )]
[assembly: AssemblyTitle( @$""{defaultTitle}"" )]
";

            const string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <Company>{defaultCompany}</Company>
    <Configuration>{defaultConfiguration}</Configuration>
    <Copyright>{defaultCopyRight}</Copyright>
    <AssemblyTitle>{defaultTitle}</AssemblyTitle>
  </PropertyGroup>
</Project>";

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void MigratePropertyGroupAndRemoveGenerateAssemblyInfoTest()
        {
            // Setup
            const string originalCsProj = 
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  </PropertyGroup>
</Project>
";
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany( ""{defaultCompany}"" )]
[assembly: AssemblyConfiguration( ""{defaultConfiguration}"" )]
[assembly: AssemblyCopyright( ""{defaultCopyRight}"" )]
";

            const string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <PropertyGroup>
    <Company>{defaultCompany}</Company>
    <Configuration>{defaultConfiguration}</Configuration>
    <Copyright>{defaultCopyRight}</Copyright>
  </PropertyGroup>
</Project>";

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void MigratePropertyGroupAndRemovePropertyGroupWithJustGenerateAssemblyInfoTest()
        {
            // Setup
            const string originalCsProj = 
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  </PropertyGroup>
</Project>
";
            const string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany( ""{defaultCompany}"" )]
[assembly: AssemblyConfiguration( ""{defaultConfiguration}"" )]
[assembly: AssemblyCopyright( ""{defaultCopyRight}"" )]
";

            const string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <Company>{defaultCompany}</Company>
    <Configuration>{defaultConfiguration}</Configuration>
    <Copyright>{defaultCopyRight}</Copyright>
  </PropertyGroup>
</Project>";

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void MigrateItemGroupToEmptyCsProjTest()
        {
            // Setup
            const string originalCsProj = 
@"<Project Sdk=""Microsoft.NET.Sdk"">
</Project>
";
            string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly:ComVisible( {defaultComVisible} )]
[assembly:CLSCompliant( {defaultCls} )]
[assembly: Guid(""{defaultGuid}"")]
";

            string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <AssemblyAttribute Include=""System.Runtime.InteropServices.ComVisibleAttribute"">
        <_Parameter1>{defaultComVisible}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.CLSCompliantAttribute"">
        <_Parameter1>{defaultCls}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Runtime.InteropServices.GuidAttribute"">
        <_Parameter1>{defaultGuid}</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>";

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void MigrateItemGroupAndRemoveGenerateAssemblyInfoTest()
        {
            // Setup
            const string originalCsProj = 
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  </PropertyGroup>
</Project>
";
            string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly:ComVisible( {defaultComVisible} )]
[assembly:CLSCompliant( {defaultCls} )]
[assembly: Guid(""{defaultGuid}"")]
";

            string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <AssemblyAttribute Include=""System.Runtime.InteropServices.ComVisibleAttribute"">
        <_Parameter1>{defaultComVisible}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.CLSCompliantAttribute"">
        <_Parameter1>{defaultCls}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Runtime.InteropServices.GuidAttribute"">
        <_Parameter1>{defaultGuid}</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>";

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void MigrateItemGroupAndRemovePropertyGroupWithJustGenerateAssemblyInfoTest()
        {
            // Setup
            const string originalCsProj = 
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  </PropertyGroup>
</Project>
";
            string originalAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly:ComVisible( {defaultComVisible} )]
[assembly:CLSCompliant( {defaultCls} )]
[assembly: Guid(""{defaultGuid}"")]
";

            string expectedCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <AssemblyAttribute Include=""System.Runtime.InteropServices.ComVisibleAttribute"">
        <_Parameter1>{defaultComVisible}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.CLSCompliantAttribute"">
        <_Parameter1>{defaultCls}</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Runtime.InteropServices.GuidAttribute"">
        <_Parameter1>{defaultGuid}</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>";

            var config = new Ai2CsprojConfig
            {
                // Default should be to migrate all.
                DeleteOldAssemblyInfo = true
            };

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void ClsCompliantTest()
        {
            // Setup
            const string originalCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">
</Project>
";
            
            const string originalAssemblyInfo =
@"using System;

[assembly: CLSCompliant(true)]
";

            const string expectedCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <AssemblyAttribute Include=""System.CLSCompliantAttribute"">
        <_Parameter1>true</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>
";
            var config = new Ai2CsprojConfig
            {
                DeleteOldAssemblyInfo = true,                
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                originalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                ""
            );
        }
    }
}
