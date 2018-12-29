using AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base;
using AssemblyPropertiesViewer.Analyzers.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            
            filters.Add(new DropDownFilter("FrameworkVersion", "Find assemblies whose target framework version string contains specific substring.")
            {
                AvailableValues = new StringDictionary() {
                    { ".NETFramework,Version=v2", "v2" },
                    { ".NETFramework,Version=v3", "v3" },
                    { ".NETFramework,Version=v3.5", "v3.5" },
                    { ".NETFramework,Version=v4", "v4" },
                    { ".NETFramework,Version=v4.5", "v4.5" },
                    { ".NETFramework,Version=v4.5.1", "v4.5.1" },
                    { ".NETFramework,Version=v4.5.2", "v4.5.2" },
                    { ".NETFramework,Version=v4.6", "v4.6" },
                    { ".NETFramework,Version=v4.6.1", "v4.6.1" }
                }
            });

            return filters;
        }
    }
}
