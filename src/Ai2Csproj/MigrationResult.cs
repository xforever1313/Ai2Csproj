using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai2Csproj
{
    internal class MigrationResult
    {
        // ---------------- Constructor ----------------

        public MigrationResult( string assemblyInfoContents, string csprojContents )
        {
            this.AssemblyInfoContents = assemblyInfoContents;
            this.CprojContents = csprojContents;
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
        public string CprojContents { get; private set; }
    }
}
