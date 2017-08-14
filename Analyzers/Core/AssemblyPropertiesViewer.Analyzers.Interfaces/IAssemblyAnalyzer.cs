using AssemblyPropertiesViewer.Analyzers.Models;
using System.Reflection;

namespace AssemblyPropertiesViewer.Analyzers.Interfaces
{
    public interface IAssemblyAnalyzer
    {
        string Name { get; }
        
        AnalysisResult Analyze(Assembly assembly);
    }
}
