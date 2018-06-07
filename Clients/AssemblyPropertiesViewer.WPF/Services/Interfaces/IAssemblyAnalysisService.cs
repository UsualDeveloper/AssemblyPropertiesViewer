using AssemblyPropertiesViewer.Analyzers.Models;
using System.Collections.Generic;
using System;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    public interface IAssemblyAnalysisService
    {
        bool IsAnalysisInProgress { get; }

        IEnumerable<AnalysisResult> InspectAssembly(string assemblyFilePath);

        long GetFileSize(string filePath);

        void InspectFolderAndFilterResults(string searchFolderPath, bool searchRecursively, IReadOnlyDictionary<Type, IEnumerable<ISearchFilter>> searchCriteria);

        IReadOnlyDictionary<Type, IEnumerable<ISearchFilter>> GetAvailableSearchFilters();
    }
}
