using AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base;
using AssemblyPropertiesViewer.Analyzers.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace AssemblyPropertiesViewer.Analyzers.BasicAnalyzers
{
    public class AssemblyFullNameAnalyzer : AnalyzerBase, IAssemblyAnalyzer
    {
        public string Name => "Full name analyzer";

        public AnalysisResult Analyze(Assembly assembly)
        {
            return new AnalysisResult("Full name") { Value = assembly.FullName };
        }

        public override IEnumerable<ISearchFilter> GetSearchFilters()
        {
            var filters = new List<ISearchFilter>()
            {
                new StringFilter("Full name search", "Filter for searching by assemblie''s full name.")
            };
            
            return filters;
        }
    }
}
