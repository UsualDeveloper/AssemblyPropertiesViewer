using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace AssemblyPropertiesViewer.Analyzers.Interfaces
{
    public interface IAssemblyAnalyzer
    {
        string Name { get; }
        
        AnalysisResult Analyze(Assembly assembly);

        IEnumerable<ISearchFilter> GetSearchFilters();
    }
}
