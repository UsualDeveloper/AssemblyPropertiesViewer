using AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base;
using AssemblyPropertiesViewer.Analyzers.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
using System.Diagnostics;
using System.Reflection;
using static System.Diagnostics.DebuggableAttribute;

namespace AssemblyPropertiesViewer.Analyzers.BasicAnalyzers
{
    public class DebugOrReleaseModeAnalyzer : AnalyzerBase, IAssemblyAnalyzer
    {
        public string Name => "Release or \"debug\" mode analyzer";

        public AnalysisResult Analyze(Assembly assembly)
        {
            var result = new AnalysisResult("Is \"debug\" compilation");
            result.Value = IsAssemblyCompiledInDebugMode(assembly).ToString();
            
            return result;
        }

        private bool IsAssemblyCompiledInDebugMode(Assembly assembly)
        {
            // https://msdn.microsoft.com/en-us/library/system.diagnostics.debuggableattribute.debuggingmodes(v=vs.110).aspx
            // https://msdn.microsoft.com/en-us/library/system.diagnostics.debuggableattribute(v=vs.110).aspx
            
            var debuggingFlags = GetAssemblyAttributeConstructorArgumentValueOrDefault<DebuggableAttribute, DebuggingModes>(assembly, 0);

            //bool isJitOptimizerDisabled, isJitTrackingEnabled;
            //switch (debuggingFlags)
            //{
            //    case DebuggingModes.None:
            //    case DebuggingModes.Default:
            //    case DebuggingModes.DisableOptimizations:
            //        isJitTrackingEnabled = true;
            //        isJitOptimizerDisabled = false;
            //        break;
            //    default:
            //        isJitTrackingEnabled = isJitOptimizerDisabled = false;
            //        break;
            //}

            //if (debuggingFlags.HasFlag(DebuggingModes.Default | DebuggingModes.DisableOptimizations))
            //{
            //    isJitTrackingEnabled = isJitOptimizerDisabled = true;
            //}

            //return (isJitOptimizerDisabled && isJitTrackingEnabled);

            return debuggingFlags.HasFlag(DebuggingModes.Default | DebuggingModes.DisableOptimizations);
        }

        
    }
}
