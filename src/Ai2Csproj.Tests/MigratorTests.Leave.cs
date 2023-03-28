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

namespace Ai2Csproj.Tests
{
    public sealed partial class MigratorTests
    {
        [TestMethod]
        public void LeaveAllAttributesTest()
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

            // If we want to leave all the attributes,
            // and we remove the GenerateAssemblyInfo,
            // we need to make sure we set all the generate methods
            // to false.
            const string expectedCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <PropertyGroup>
    <GenerateAssemblyCompanyAttribute>false</GenerateAssemblyCompanyAttribute>
    <GenerateAssemblyConfigurationAttribute>false</GenerateAssemblyConfigurationAttribute>
    <GenerateAssemblyCopyrightAttribute>false</GenerateAssemblyCopyrightAttribute>
    <GenerateAssemblyDescriptionAttribute>false</GenerateAssemblyDescriptionAttribute>
    <GenerateAssemblyFileVersionAttribute>false</GenerateAssemblyFileVersionAttribute>
    <GenerateAssemblyInformationalVersionAttribute>false</GenerateAssemblyInformationalVersionAttribute>
    <GenerateAssemblyProductAttribute>false</GenerateAssemblyProductAttribute>
    <GenerateAssemblyTitleAttribute>false</GenerateAssemblyTitleAttribute>
    <GenerateAssemblyVersionAttribute>false</GenerateAssemblyVersionAttribute>
    <GenerateNeutralResourcesLanguageAttribute>false</GenerateNeutralResourcesLanguageAttribute>
  </PropertyGroup>
</Project>
";
            var config = GetConfigWithAllLeave( null );

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                defaultStartingAssemblyInfo,
                expectedCsProj,
                defaultStartingAssemblyInfo
            );
        }

        [TestMethod]
        public void LeaveAllAttributesWithEmptyCsprojTest()
        {
            // Setup
            const string originalCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">
</Project>
";

            // If we want to leave all the attributes,
            // and we remove the GenerateAssemblyInfo,
            // we need to make sure we set all the generate methods
            // to false.
            const string expectedCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <GenerateAssemblyCompanyAttribute>false</GenerateAssemblyCompanyAttribute>
    <GenerateAssemblyConfigurationAttribute>false</GenerateAssemblyConfigurationAttribute>
    <GenerateAssemblyCopyrightAttribute>false</GenerateAssemblyCopyrightAttribute>
    <GenerateAssemblyDescriptionAttribute>false</GenerateAssemblyDescriptionAttribute>
    <GenerateAssemblyFileVersionAttribute>false</GenerateAssemblyFileVersionAttribute>
    <GenerateAssemblyInformationalVersionAttribute>false</GenerateAssemblyInformationalVersionAttribute>
    <GenerateAssemblyProductAttribute>false</GenerateAssemblyProductAttribute>
    <GenerateAssemblyTitleAttribute>false</GenerateAssemblyTitleAttribute>
    <GenerateAssemblyVersionAttribute>false</GenerateAssemblyVersionAttribute>
    <GenerateNeutralResourcesLanguageAttribute>false</GenerateNeutralResourcesLanguageAttribute>
  </PropertyGroup>
</Project>
";
            var config = GetConfigWithAllLeave( null );

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                defaultStartingAssemblyInfo,
                expectedCsProj,
                defaultStartingAssemblyInfo
            );
        }

        [TestMethod]
        public void LeaveOnlyCopyrightAndTrademarkAttributesTest()
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

            // If we want to leave all the attributes,
            // and we remove the GenerateAssemblyInfo,
            // we need to make sure we set all the generate methods
            // to false.
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
  </PropertyGroup>
  <PropertyGroup>
    <GenerateAssemblyCompanyAttribute>false</GenerateAssemblyCompanyAttribute>
    <GenerateAssemblyConfigurationAttribute>false</GenerateAssemblyConfigurationAttribute>
    <GenerateAssemblyDescriptionAttribute>false</GenerateAssemblyDescriptionAttribute>
    <GenerateAssemblyFileVersionAttribute>false</GenerateAssemblyFileVersionAttribute>
    <GenerateAssemblyInformationalVersionAttribute>false</GenerateAssemblyInformationalVersionAttribute>
    <GenerateAssemblyProductAttribute>false</GenerateAssemblyProductAttribute>
    <GenerateAssemblyTitleAttribute>false</GenerateAssemblyTitleAttribute>
    <GenerateAssemblyVersionAttribute>false</GenerateAssemblyVersionAttribute>
    <GenerateNeutralResourcesLanguageAttribute>false</GenerateNeutralResourcesLanguageAttribute>
  </PropertyGroup>
  <ItemGroup>
    <AssemblyAttribute Include=""System.Reflection.AssemblyTrademarkAttribute"">
        <_Parameter1>{defaultTrademark}</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>
";
            string expectedAssemblyInfo = GetDefaultStartingAssemblyInfo( false, false );
            expectedAssemblyInfo = expectedAssemblyInfo.Replace(
                $@"[assembly: AssemblyCopyright( ""{defaultCopyRight}"" )]{Environment.NewLine}",
                ""
            );
            expectedAssemblyInfo = expectedAssemblyInfo.Replace(
                $@"{Environment.NewLine}[assembly: AssemblyTrademark( ""{defaultTrademark}"" )]{Environment.NewLine}",
                ""
            );

            var config = GetConfigWithAllLeave(
                SupportedAssemblyAttributes.assembly_copyright | 
                SupportedAssemblyAttributes.assembly_trademark
            );

            // Act / Check
            DoMigrationTest(
                config,
                originalCsProj,
                defaultStartingAssemblyInfo,
                expectedCsProj,
                expectedAssemblyInfo
            );
        }
    }
}
