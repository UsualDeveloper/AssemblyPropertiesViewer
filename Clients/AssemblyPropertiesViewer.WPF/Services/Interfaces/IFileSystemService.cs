using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    internal interface IFileSystemService
    {
        IEnumerable<string> GetAssembliesInDirectory(string directoryPath, bool getRecursivelyFromSubdirectories);
    }
}
