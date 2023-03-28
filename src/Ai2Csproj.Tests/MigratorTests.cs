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
    [TestClass]
    public sealed partial class MigratorTests
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
        private const string defaultKeyFile = "somekey.file";
        private const string defaultKeyName = "somekeyname";
        private const string defaultPublicKey = "publicKey";
        private const string defaultCounterSignature = "somecountersignature";

        private const string defaultCulture = "en-US";

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
            GetDefaultStartingAssemblyInfo( false, false );

        // ---------------- Test Helpers ----------------

        private static MigrationResult DoMigrationTest(
            Ai2CsprojConfig config,
            string startingCsProj,
            string startingAssemblyInfo,
            string expectedCsProj,
            string expectedAssemblyInfo
        )
        {
            return CommonTests.DoMigrationTest(
                config,
                startingCsProj,
                startingAssemblyInfo,
                expectedCsProj,
                expectedAssemblyInfo
            );
        }

        private static Ai2CsprojConfig GetConfigWithAllMigrated( SupportedAssemblyAttributes? excludedValues )
        {
            var config = new Ai2CsprojConfig();
            foreach( SupportedAssemblyAttributes type in Enum.GetValues<SupportedAssemblyAttributes>() )
            {
                if( excludedValues is not null )
                {
                    if( ( excludedValues & type ) == type )
                    {
                        continue;
                    }
                }
                config = config with { TypesToMigrate = config.TypesToMigrate | type };
            }

            return config;
        }

        private static Ai2CsprojConfig GetConfigWithAllDeleted( SupportedAssemblyAttributes? excludedValues )
        {
            var config = new Ai2CsprojConfig();
            foreach( SupportedAssemblyAttributes type in Enum.GetValues<SupportedAssemblyAttributes>() )
            {
                if( excludedValues is not null )
                {
                    if( ( excludedValues & type ) == type )
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
                    if( ( excludedValues & type ) == type )
                    {
                        continue;
                    }
                }
                config = config with { TypesToLeave = config.TypesToLeave | type };
            }

            return config;
        }

        private static string GetDefaultStartingAssemblyInfo(
            bool includeSuffix,
            bool includeNamespace
        )
        {
            string suffixString = includeSuffix ? "Attribute" : "";

            string systemNamespace = includeNamespace ? "System." : "";
            string reflectionNamespace = includeNamespace ? "System.Reflection." : "";
            string resourcesNameSpace = includeNamespace ? "System.Resources." : "";
            string compilerServicesNameSpace = includeNamespace ? "System.Runtime.CompilerServices." : "";
            string interopServicesNameSpace = includeNamespace ? "System.Runtime.InteropServices." : "";

            return
$@"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: {reflectionNamespace}AssemblyCompany{suffixString}( ""{defaultCompany}"" )]
[assembly: {reflectionNamespace}AssemblyConfiguration{suffixString}( ""{defaultConfiguration}"" )]
[assembly: {reflectionNamespace}AssemblyCopyright{suffixString}( ""{defaultCopyRight}"" )]
[assembly: {reflectionNamespace}AssemblyDescription{suffixString}( ""{defaultDescription}"" )]
[assembly: {reflectionNamespace}AssemblyFileVersion{suffixString}( ""{defaultFileVersion}"" )]
[assembly: {reflectionNamespace}AssemblyInformationalVersion{suffixString}( ""{defaultInfoVersion}"" )]
[assembly: {reflectionNamespace}AssemblyProduct{suffixString}( ""{defaultProduct}"" )]
[assembly: {reflectionNamespace}AssemblyTitle{suffixString}( ""{defaultTitle}"" )]
[assembly: {reflectionNamespace}AssemblyVersion{suffixString}( ""{defaultVersion}"" )]
[assembly: {resourcesNameSpace}NeutralResourcesLanguage{suffixString}( ""{defaultLanguage}"" )]

[assembly: {reflectionNamespace}AssemblyTrademark{suffixString}( ""{defaultTrademark}"" )]
[assembly: {compilerServicesNameSpace}InternalsVisibleTo{suffixString}( ""{defaultInternals1}"" ),InternalsVisibleTo( ""{defaultInternals2}"" )]
[assembly: {interopServicesNameSpace}ComVisible{suffixString}( {defaultComVisible} )]
[assembly: {systemNamespace}CLSCompliant{suffixString}( {defaultCls} )]
[assembly: {interopServicesNameSpace}Guid{suffixString}(""{defaultGuid}"")]
[assembly: {reflectionNamespace}AssemblyKeyFile{suffixString}( ""{defaultKeyFile}"" )]
[assembly: {reflectionNamespace}AssemblyKeyName{suffixString}( ""{defaultKeyName}"" )]
[assembly: {reflectionNamespace}AssemblySignatureKey{suffixString}( ""{defaultPublicKey}"", ""{defaultCounterSignature}"" )]
[assembly: {reflectionNamespace}AssemblyCulture{suffixString}( ""{defaultCulture}"" )]
";
        }
    }
}
