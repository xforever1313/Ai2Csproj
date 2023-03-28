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
    internal static class CommonTests
    {
        // ---------------- Functions ----------------

        public static MigrationResult DoMigrationTest(
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

            return result;
        }
    }
}
