using AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base;
using AssemblyPropertiesViewer.Analyzers.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
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
    }
}
