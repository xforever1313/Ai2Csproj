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

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using SethCS.Extensions;

namespace Ai2Csproj
{
    public readonly struct PreprocessorDefinesCollection : IEquatable<PreprocessorDefinesCollection>
    {
        // ---------------- Constructor ----------------

        public PreprocessorDefinesCollection() :
            this( ImmutableArray.Create<string>() )
        {
        }

        public PreprocessorDefinesCollection( ImmutableArray<string> defines )
        {
            this.Defines = defines;
        }

        // ---------------- Properties ----------------

        public ImmutableArray<string> Defines { get; }

        // ---------------- Functions ----------------

        public PreprocessorDefinesCollection Add( string define )
        {
            if( this.Defines.IsDefault )
            {
                return new PreprocessorDefinesCollection( ImmutableArray.Create( define ) );
            }

            return new PreprocessorDefinesCollection( this.Defines.Add( define ) );
        }

        public override bool Equals( [NotNullWhen( true )] object? obj )
        {
            if( obj is null )
            {
                return false;
            }
            else if( obj is PreprocessorDefinesCollection )
            {
                return false;
            }
            else
            {
                return Equals( (PreprocessorDefinesCollection)obj );
            }
        }

        public bool Equals( PreprocessorDefinesCollection other )
        {
            if( this.Defines.IsDefault )
            {
                return other.Defines.IsDefault;
            }
            else if( other.Defines.IsDefault )
            {
                return this.Defines.IsDefault;
            }

            return this.Defines.SequenceEqual( other.Defines );
        }

        public override int GetHashCode()
        {
            return this.Defines.GetHashCode();
        }

        public override string ToString()
        {
            return this.Defines.ToListString();
        }

        public static bool operator ==( PreprocessorDefinesCollection left, PreprocessorDefinesCollection right )
        {
            return left.Equals( right );
        }

        public static bool operator !=( PreprocessorDefinesCollection left, PreprocessorDefinesCollection right )
        {
            return !( left == right );
        }
    }
}
