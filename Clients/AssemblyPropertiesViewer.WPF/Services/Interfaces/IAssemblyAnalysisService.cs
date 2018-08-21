using AssemblyPropertiesViewer.Analyzers.Models;
using System.Collections.Generic;
using System;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    public interface IAssemblyAnalysisService
    {
        bool IsAnalysisInProgress { get; }

        IEnumerable<AnalysisResult> InspectAssembly(string assemblyFilePath/*, IReadOnlyDictionary<string, IEnumerable<ISearchFilter>> searchCriteria = null*/);

        long GetFileSize(string filePath);

        IReadOnlyDictionary<string, IEnumerable<AnalysisResult>> InspectFolderAndFilterResults(string searchFolderPath, bool searchRecursively, IReadOnlyDictionary<string, IEnumerable<ISearchFilter>> searchCriteria);

        IReadOnlyDictionary<string, IEnumerable<ISearchFilter>> GetAvailableSearchFilters();
    }
}
