using AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base;
using AssemblyPropertiesViewer.Analyzers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyPropertiesViewer.Analyzers.Models;
using System.Reflection;

namespace AssemblyPropertiesViewer.Analyzers.BasicAnalyzers
{
    public class AssemblyStrongNameAnalyzer : AnalyzerBase, IAssemblyAnalyzer
    {
        public string Name => "Strong name analyzer";

        public AnalysisResult Analyze(Assembly assembly)
        {
            var result = new AnalysisResult("Strong name info");

            bool isStrongNameAssembly = IsStrongNameAssembly(assembly);

            result.Value = isStrongNameAssembly ? "The assembly is strong-named." : "The assembly is NOT strong-named.";
            
            return result;
        }

        private bool IsStrongNameAssembly(Assembly assembly)
        {
            byte[] tokenBytes = assembly.GetName().GetPublicKeyToken();
            return tokenBytes != null && tokenBytes.Length > 0;
        }
    }
}
