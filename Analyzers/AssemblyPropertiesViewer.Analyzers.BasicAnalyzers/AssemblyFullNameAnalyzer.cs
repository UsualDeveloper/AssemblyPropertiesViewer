using AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base;
using AssemblyPropertiesViewer.Analyzers.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
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
    }
}
