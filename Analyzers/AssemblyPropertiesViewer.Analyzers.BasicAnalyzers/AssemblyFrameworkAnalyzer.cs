using AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base;
using AssemblyPropertiesViewer.Analyzers.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;

namespace AssemblyPropertiesViewer.Analyzers.BasicAnalyzers
{
    public class AssemblyFrameworkAnalyzer : AnalyzerBase, IAssemblyAnalyzer
    {
        public string Name => "Target framework analyzer";

        public AnalysisResult Analyze(Assembly assembly)
        {
            var result = new AnalysisResult("Target framework");
            var assemblyTargetFramework = GetAssemblyAttributeConstructorArgumentValueOrDefault<TargetFrameworkAttribute, string>(assembly, 0);
            result.Value = $"{assemblyTargetFramework} (image runtime version: {assembly.ImageRuntimeVersion})";

            return result;
        }

        public override IEnumerable<ISearchFilter> GetSearchFilters()
        {
            var filters = new List<ISearchFilter>(2);

            // simple filter based on substring comparison
            // .NETFramework,Version=v4.5.2
            filters.Add(new StringFilter("FrameworkVersion", "Find assemblies whose target framework version string contains specific substring."));

            // filter for image runtime version substring-based comparison
            filters.Add(new StringFilter("ImageRuntimeVersion", "Find assemblies whose image runtime version string contains specific substring."));

            return filters;
        }
    }
}
