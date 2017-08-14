using AssemblyPropertiesViewer.Analyzers.Models;
using System.Collections.Generic;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    public interface IAssemblyAnalysisService
    {
        IEnumerable<AnalysisResult> InspectAssembly(string assemblyFilePath);
    }
}
