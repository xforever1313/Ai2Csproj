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
    public sealed class EnumTests
    {
        // ---------------- Tests ----------------

        /// <summary>
        /// Sanity check to make sure each enum is unique
        /// in their value.
        /// 
        /// We should actually compile error, but one more sanity check
        /// doesn't hurt.
        /// </summary>
        [TestMethod]
        public void SupportedTypesUniquenessTest()
        {
            foreach( SupportedAssemblyAttributes left in Enum.GetValues<SupportedAssemblyAttributes>() )
            {
                foreach( SupportedAssemblyAttributes right in Enum.GetValues<SupportedAssemblyAttributes>() )
                {
                    // Use strings since if we compare the two directly, it may
                    // integer compare, and return true.
                    if( left.ToString() == right.ToString() )
                    {
                        Assert.AreEqual( left, right );
                    }
                    else
                    {
                        Assert.AreNotEqual( left, right );
                    }
                }
            }
        }

        [TestMethod]
        public void SupportedTypesDivisibleBy2Test()
        {
            foreach( SupportedAssemblyAttributes uut in Enum.GetValues<SupportedAssemblyAttributes>() )
            {
                Assert.AreEqual( 0, (long)uut % 2L );
            }
        }
    }
}
