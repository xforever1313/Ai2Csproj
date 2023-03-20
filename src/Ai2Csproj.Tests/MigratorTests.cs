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

using System.Xml.Linq;

namespace Ai2Csproj.Tests
{
    [TestClass]
    public sealed class MigratorTests
    {
        // ---------------- Fields ----------------

        private const string defaultCompany = "bettadelic.art";
        private const string defaultConfiguration = "Release";
        private const string defaultCopyRight = "Copyright (C) Seth Hendrick";
        private const string defaultDescription = "My Test Assembly";
        private const string defaultFileVersion = "1.2.3";
        private const string defaultInfoVersion = "2.3.4";
        private const string defaultProduct = "My Fancy Product!";
        private const string defaultTitle = "TestAssembly";
        private const string defaultVersion = "5.6.7.8";
        private const string defaultLanguage = "en-US";
        private const string defaultInternals1 = "LibSethCS";
        private const string defaultInternals2 = "Seth.Analyzer";
        private const bool defaultComVisible = true;
        private const string defaultTrademark = "My Trademark";
        private const bool defaultCls = false;

        private const string defaultGuid = "9ED54F84-A89D-4fcd-A854-44251E925F09";

        private const string defaultStartingCsProj =
$@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
	<PackageReference Include=""Microsoft.CodeAnalysis.CSharp"" />
	<PackageReference Include=""Mono.Options"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""..\..\SethCS\LibSethCS\LibSethCS.csproj"" />
  </ItemGroup>

</Project>
";
        private static readonly string defaultStartingAssemblyInfo =
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany( ""{defaultCompany}"" )]
[assembly: AssemblyConfiguration( ""{defaultConfiguration}"" )]
[assembly: AssemblyCopyright( ""{defaultCopyRight}"" )]
[assembly: AssemblyDescription( ""{defaultDescription}"" )]
[assembly: AssemblyFileVersion( ""{defaultFileVersion}"" )]
[assembly: AssemblyInformationalVersion( ""{defaultInfoVersion}"" )]
[assembly: AssemblyProduct( ""{defaultProduct}"" )]
[assembly: AssemblyTitle( ""{defaultTitle}"" )]
[assembly: AssemblyVersion( ""{defaultVersion}"" )]
[assembly: NeutralResourcesLanguage( ""{defaultLanguage}"" )]

[assembly: AssemblyTrademark( ""{defaultTrademark}"" )]
[assembly: InternalsVisibleTo( ""{defaultInternals1}"" ),InternalsVisibleTo( ""{defaultInternals2}"" )]
[assembly:ComVisible( {defaultComVisible} )]
[assembly:CLSCompliant( {defaultCls} )]
[assembly: Guid(""{defaultGuid}"")]
";
        // ---------------- Tests ----------------

        [TestMethod]
        public void MigrateAllAttributesTest()
        {
            string expectedCsProj =
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
  </ItemGroup>
</Project>";

            // Setup
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

        // -------- Delete Tests --------

        [TestMethod]
        public void DeleteAllAttributesTest()
        {
            // Setup
            var config = GetConfigWithAllDeleted( null );
            config = config with { DeleteOldAssemblyInfo = true };

            // Act / Check
            DoMigrationTest(
                config,
                defaultStartingCsProj,
                defaultStartingAssemblyInfo,
                defaultStartingCsProj,
                ""
            );
        }

        // -------- Leave Tests --------

        [TestMethod]
        public void LeaveAllAttributesTest()
        {
            // Setup
            var config = GetConfigWithAllLeave( null );

            // Act / Check
            DoMigrationTest(
                config,
                defaultStartingCsProj,
                defaultStartingAssemblyInfo,
                defaultStartingCsProj,
                defaultStartingAssemblyInfo
            );
        }

        // ---------------- Test Helpers ----------------

        private void DoMigrationTest(
            Ai2CsprojConfig config,
            string startingCsProj,
            string startingAssemblyInfo,
            string expectedCsProj,
            string expectedAssemblyInfo
        )
        {
            var uut = new Migrator( config );
            MigrationResult result = uut.Migrate( startingCsProj, startingAssemblyInfo );

            XDocument expectedCsProjXml = XDocument.Parse( expectedCsProj );
            XDocument actualCsProjXml = XDocument.Parse( result.CsprojContents );

            Assert.AreEqual( expectedCsProjXml.ToString(), actualCsProjXml.ToString() );
            Assert.AreEqual( expectedAssemblyInfo, result.AssemblyInfoContents );
        }

        private static Ai2CsprojConfig GetConfigWithAllDeleted( SupportedAssemblyAttributes? excludedValues )
        {
            var config = new Ai2CsprojConfig();
            foreach( SupportedAssemblyAttributes type in Enum.GetValues<SupportedAssemblyAttributes>() )
            {
                if( excludedValues is not null )
                {
                    if( ( excludedValues & type ) == excludedValues )
                    {
                        continue;
                    }
                }
                config = config with { TypesToDelete = config.TypesToDelete | type };
            }

            return config;
        }

        private static Ai2CsprojConfig GetConfigWithAllLeave( SupportedAssemblyAttributes? excludedValues )
        {
            var config = new Ai2CsprojConfig();
            foreach( SupportedAssemblyAttributes type in Enum.GetValues<SupportedAssemblyAttributes>() )
            {
                if( excludedValues is not null )
                {
                    if( ( excludedValues & type ) == excludedValues )
                    {
                        continue;
                    }
                }
                config = config with { TypesToLeave = config.TypesToLeave | type };
            }

            return config;
        }
    }
}
