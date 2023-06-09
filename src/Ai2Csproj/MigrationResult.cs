﻿// Ai2Csproj - A program that migrates AssemblyInfo files to csproj.
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

namespace Ai2Csproj
{
    internal class MigrationResult
    {
        // ---------------- Constructor ----------------

        public MigrationResult( string assemblyInfoContents, string csprojContents )
        {
            this.AssemblyInfoContents = assemblyInfoContents;
            this.CsprojContents = csprojContents;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The contents to write to the AssemblyInfo file.
        /// If empty string, it means delete the file.
        /// </summary>
        public string AssemblyInfoContents { get; private set; }

        /// <summary>
        /// The contents of the csproj to write out.
        /// </summary>
        public string CsprojContents { get; private set; }
    }
}
