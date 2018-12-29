using System;

namespace AssemblyPropertiesViewer.Analyzers.Models
{
    [Serializable]
    public sealed class AnalysisResult
    {
        public string AnalyzerTypeFullName { get; set; }

        public string AssemblyPropertyName { get; private set; }

        public string Value { get; set; }

        public AnalysisResult(string assemblyPropertyName)
        {
            this.AssemblyPropertyName = assemblyPropertyName;
        }
    }
}
