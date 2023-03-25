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

using Ai2Csproj.Shared;

namespace Ai2Csproj.Exceptions
{
    internal class MissingVersionSourceException : Exception
    {
        // ---------------- Constructor ----------------

        public MissingVersionSourceException( VersionSource versionSource )
        {
            this.ExpectedVersionSource = versionSource;
            this.MissingAttributeType = AssemblyAttributeMapping.GetVersionSourceType( versionSource );
        }

        // ---------------- Properties ---------------

        public VersionSource ExpectedVersionSource { get; private set; }

        public Type MissingAttributeType { get; private set; }

        public override string Message =>
            $"Error when trying to create a Version tag in the csproj.  " +
            $"Version source specified was {this.ExpectedVersionSource}, but no {this.MissingAttributeType.FullName} attribute was in the AssemblyInfo file";
    }
}
