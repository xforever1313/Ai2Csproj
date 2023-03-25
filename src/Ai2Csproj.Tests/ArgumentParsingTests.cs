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
using System.Text;
using System.Threading.Tasks;

namespace Ai2Csproj.Tests
{
    [TestClass]
    public sealed partial class ArgumentParsingTests
    {
        // ---------------- Tests ----------------

        [TestMethod]
        public void ShowHelpTest()
        {
            // Setup
            string[] args = new string[] { "--help" };
            var uut = new ArgumentParser();

            // Act
            uut.Parse( args );

            // Check
            Assert.IsTrue( uut.ShowHelp );
            Assert.IsFalse( uut.ShowVersion );
            Assert.IsFalse( uut.ShowCredits );
            Assert.IsFalse( uut.ShowLicense );
            Assert.IsFalse( uut.ShowSupportedTypes );
        }

        [TestMethod]
        public void ShowVersionTest()
        {
            // Setup
            string[] args = new string[] { "--version" };
            var uut = new ArgumentParser();

            // Act
            uut.Parse( args );

            // Check
            Assert.IsFalse( uut.ShowHelp );
            Assert.IsTrue( uut.ShowVersion );
            Assert.IsFalse( uut.ShowCredits );
            Assert.IsFalse( uut.ShowLicense );
            Assert.IsFalse( uut.ShowSupportedTypes );
        }

        [TestMethod]
        public void ShowCreditsTest()
        {
            // Setup
            string[] args = new string[] { "--print_credits" };
            var uut = new ArgumentParser();

            // Act
            uut.Parse( args );

            // Check
            Assert.IsFalse( uut.ShowHelp );
            Assert.IsFalse( uut.ShowVersion );
            Assert.IsTrue( uut.ShowCredits );
            Assert.IsFalse( uut.ShowLicense );
            Assert.IsFalse( uut.ShowSupportedTypes );
        }

        [TestMethod]
        public void ShowLicenseTest()
        {
            // Setup
            string[] args = new string[] { "--print_license" };
            var uut = new ArgumentParser();

            // Act
            uut.Parse( args );

            // Check
            Assert.IsFalse( uut.ShowHelp );
            Assert.IsFalse( uut.ShowVersion );
            Assert.IsFalse( uut.ShowCredits );
            Assert.IsTrue( uut.ShowLicense );
            Assert.IsFalse( uut.ShowSupportedTypes );
        }

        [TestMethod]
        public void ShowSupportedTypesTest()
        {
            // Setup
            string[] args = new string[] { "--list_supported_types" };
            var uut = new ArgumentParser();

            // Act
            uut.Parse( args );

            // Check
            Assert.IsFalse( uut.ShowHelp );
            Assert.IsFalse( uut.ShowVersion );
            Assert.IsFalse( uut.ShowCredits );
            Assert.IsFalse( uut.ShowLicense );
            Assert.IsTrue( uut.ShowSupportedTypes );
        }

        [TestMethod]
        public void ProjectPathArgumentTest()
        {
            string[] args = new string[] { "--project_path=someproject.csproj" };

            var expectedConfig = new Ai2CsprojConfig
            {
                CsProjPath = "someproject.csproj"
            };

            DoArgumentParsingToConfigTest( args, expectedConfig );
        }

        [TestMethod]
        public void AssemblyInfoPathArgumentTest()
        {
            string[] args = new string[] { "--assembly_info_path=Properties/AssemblyInfoSomething.cs" };

            var expectedConfig = new Ai2CsprojConfig
            {
                AssmblyInfoPath = "Properties/AssemblyInfoSomething.cs"
            };

            DoArgumentParsingToConfigTest( args, expectedConfig );
        }

        [TestMethod]
        public void DeleteOldAssemblyInfoTest()
        {
            string[] args = new string[] { "--delete_old_assembly_info" };

            var expectedConfig = new Ai2CsprojConfig
            {
                DeleteOldAssemblyInfo = true
            };

            DoArgumentParsingToConfigTest( args, expectedConfig );
        }

        [TestMethod]
        public void MigrateUnsupportedTypesTest()
        {
            string[] args = new string[] { "--migrate_unsupported_types" };

            var expectedConfig = new Ai2CsprojConfig
            {
                MigrateUnsupportedTypes = true
            };

            DoArgumentParsingToConfigTest( args, expectedConfig );
        }

        [TestMethod]
        public void DryRunTest()
        {
            string[] args = new string[] { "--dry_run" };

            var expectedConfig = new Ai2CsprojConfig
            {
                DryRun = true
            };

            DoArgumentParsingToConfigTest( args, expectedConfig );
        }

        [TestMethod]
        public void NoBackupTest()
        {
            string[] args = new string[] { "--no_backup" };

            var expectedConfig = new Ai2CsprojConfig
            {
                DeleteBackup = true
            };

            DoArgumentParsingToConfigTest( args, expectedConfig );
        }

        // ---------------- Test Helpers ----------------

        /// <summary>
        /// Tests to see if the passed in arguments results in the
        /// given config.
        /// </summary>
        private void DoArgumentParsingToConfigTest( string[] args, Ai2CsprojConfig expectedConfig )
        {
            // Setup
            var uut = new ArgumentParser();

            // Act
            uut.Parse( args );

            // Check
            Assert.IsFalse( uut.ShowHelp );
            Assert.IsFalse( uut.ShowVersion );
            Assert.IsFalse( uut.ShowCredits );
            Assert.IsFalse( uut.ShowLicense );
            Assert.IsFalse( uut.ShowSupportedTypes );
            Assert.AreEqual( expectedConfig, uut.Config );
        }
    }
}
