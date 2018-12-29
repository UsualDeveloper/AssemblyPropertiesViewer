using AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base;
using AssemblyPropertiesViewer.Analyzers.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
using System.Reflection;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using System.Collections.Generic;

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
