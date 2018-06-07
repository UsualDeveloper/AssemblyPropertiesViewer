﻿using AssemblyPropertiesViewer.Analyzers.Models;
using System.Reflection;
using System.Collections.Generic;

namespace AssemblyPropertiesViewer.Analyzers.Interfaces
{
    public interface IAssemblyAnalyzer
    {
        string Name { get; }
        
        AnalysisResult Analyze(Assembly assembly);

        IEnumerable<ISearchFilter> GetSearchFilters();
    }
}
