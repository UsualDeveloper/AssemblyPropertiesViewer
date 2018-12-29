using AssemblyPropertiesViewer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyPropertiesViewer.Services
{
    internal class FileSystemService : IFileSystemService
    {
        private const string AssemblyFileSearchPattern = "*.dll";

        public IEnumerable<string> GetAssembliesInDirectory(string directoryPath, bool getRecursivelyFromSubdirectories)
        {
            return Directory.EnumerateFiles(
                directoryPath,
                AssemblyFileSearchPattern,
                getRecursivelyFromSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
    }
}
