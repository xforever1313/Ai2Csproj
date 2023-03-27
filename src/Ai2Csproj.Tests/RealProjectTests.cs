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
    public sealed class RealProjectTests
    {
        // ---------------- Fields ----------------

        private const string defaultOriginalCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">
</Project>
";

        // ---------------- Tests ----------------

        [TestMethod]
        public void XPTableTest()
        {
            // Setup

            // Taken from here:
            // https://raw.githubusercontent.com/schoetbi/XPTable/master/AssemblyInfo.cs
            const string originalAssemblyInfo =
@"using System;
using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle(""XPTable"")]
[assembly: AssemblyDescription(""A fully customizable ListView style control based on Java's JTable"")]
[assembly: AssemblyConfiguration("""")]
[assembly: AssemblyCompany(""Hall, adambl, adarmus, lanwin, peyn, schoetbi, Jeoffman"")]
[assembly: AssemblyProduct(""XPTable"")]
[assembly: AssemblyCopyright(""Copyright � 2005, Mathew Hall.  All rights reserved."")]
[assembly: AssemblyTrademark("""")]
[assembly: AssemblyCulture("""")] 
[assembly: CLSCompliant(true)]  

[assembly: AssemblyVersion(""1.6.1.*"")]
";

            const string expectedCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <AssemblyTitle>XPTable</AssemblyTitle>
    <Description>A fully customizable ListView style control based on Java's JTable</Description>
    <Configuration></Configuration>
    <Company>Hall, adambl, adarmus, lanwin, peyn, schoetbi, Jeoffman</Company>
    <Product>XPTable</Product>
    <Copyright>Copyright � 2005, Mathew Hall.  All rights reserved.</Copyright>
    <AssemblyVersion>1.6.1.*</AssemblyVersion>
  </PropertyGroup>
  <ItemGroup>
    <AssemblyAttribute Include=""System.Reflection.AssemblyTrademarkAttribute"">
        <_Parameter1></_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.Reflection.AssemblyCultureAttribute"">
        <_Parameter1></_Parameter1>
    </AssemblyAttribute>
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
                defaultOriginalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                ""
            );
        }

        [TestMethod]
        public void TreeViewAdvTest()
        {
            // Setup
            // Taken from here:
            // https://raw.githubusercontent.com/AdamsLair/treeviewadv/master/Aga.Controls/Properties/AssemblyInfo.cs
            const string originalAssemblyInfo =
@"using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Security.Permissions;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

[assembly: AssemblyTitle(""Aga.Controls"")]
[assembly: AssemblyCopyright(""Copyright © Andrey Gliznetsov 2006 - 2009"")]
[assembly: AssemblyDescription(""http://sourceforge.net/projects/treeviewadv/"")]

[assembly: AssemblyVersion(""1.7.7"")]
";
            const string expectedCsProj =
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <AssemblyTitle>Aga.Controls</AssemblyTitle>
    <Copyright>Copyright © Andrey Gliznetsov 2006 - 2009</Copyright>
    <Description>http://sourceforge.net/projects/treeviewadv/</Description>
    <AssemblyVersion>1.7.7</AssemblyVersion>
  </PropertyGroup>
  <ItemGroup>
    <AssemblyAttribute Include=""System.Runtime.InteropServices.ComVisibleAttribute"">
        <_Parameter1>false</_Parameter1>
    </AssemblyAttribute>
    <AssemblyAttribute Include=""System.CLSCompliantAttribute"">
        <_Parameter1>false</_Parameter1>
    </AssemblyAttribute>
  </ItemGroup>
</Project>
";

            const string expectedAssemblyInfo =
@"using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Security.Permissions;
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]
";

            var config = new Ai2CsprojConfig
            {
                DeleteOldAssemblyInfo = true,
                // Set to false since we don't support SecurityPermissions
                // at all.
                MigrateUnsupportedTypes = false
            };

            // Act / Check
            CommonTests.DoMigrationTest(
                config,
                defaultOriginalCsProj,
                originalAssemblyInfo,
                expectedCsProj,
                expectedAssemblyInfo
            );
        }
    }
}
