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
    }
}