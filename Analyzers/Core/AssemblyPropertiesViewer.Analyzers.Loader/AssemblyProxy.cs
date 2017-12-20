using AssemblyPropertiesViewer.Analyzers.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;

namespace AssemblyPropertiesViewer.Analyzers.Loader
{
    public class AssemblyProxy : MarshalByRefObject
    {
        Assembly loadedAssembly;
        IEnumerable<Assembly> referencedAssemblies;

        private const string BadImageExceptionMessage = "The specific file in not in a supported format and cannot be loaded. Please verify if the file is a valid assembly.";

        private const string FileLoadExceptionMessage = "Additional file load exception (this can be caused by additional references of the assembly to be analyzed).";
        private const string SecurityExceptionMessage = "Cannot analyze assembly due to security exception.";
        private const string GeneralExceptionMessage = "Error occurred during assembly analysis attempt.";

        public List<AnalysisResult> InspectAssembly(string filePath)
        {
            var analysisResults = new List<AnalysisResult>();

            try
            {
                LoadAssemblyIntoReflectionOnlyContext(filePath);
            }
            catch (BadImageFormatException)
            {
                analysisResults.Add(new AnalysisResult("Error") { Value = BadImageExceptionMessage });
                return analysisResults;
            }
            
            var assemblyAnalyzers = GetAnalyzers();
            foreach (var analyzer in assemblyAnalyzers)
            {
                analysisResults.Add(InspectAssemblyWithAnalyzer(analyzer));
            }

            return analysisResults;
        }

        private IEnumerable<IAssemblyAnalyzer> GetAnalyzers()
        {
            var analyzersCollection = new List<IAssemblyAnalyzer>();

            var analyzersLoader = new BasicAnalyzersLoader();
            var assemblyAnalyzersTypes = analyzersLoader.GetBasicAnalyzers();

            foreach (var analyzerType in assemblyAnalyzersTypes)
            {
                var analyzer = (Activator.CreateInstance(analyzerType) as IAssemblyAnalyzer);
                if (analyzer == null)
                {
                    throw new InvalidCastException($"Loaded analyzer type is not of a supported type ({analyzerType.FullName} does not implement {nameof(IAssemblyAnalyzer)}).");
                }

                analyzersCollection.Add(analyzer);
            }

            return analyzersCollection;
        }

        private void LoadAssemblyIntoReflectionOnlyContext(string filePath)
        {
            loadedAssembly = Assembly.ReflectionOnlyLoadFrom(filePath);
            
            // preload referenced assemblies, because in a limited privileges context assembly resolving event cannot be used
            LoadReferencedAssembliesIntoContext(loadedAssembly);
        }

        private void LoadReferencedAssembliesIntoContext(Assembly loadedAssembly)
        {
            var referencedAssemblies = new List<Assembly>();

            AssemblyName[] referencedAssemblyNames = loadedAssembly.GetReferencedAssemblies();

            if (referencedAssemblyNames == null)
                return;

            // TODO: verify
            foreach (var a in referencedAssemblyNames)
            {
                try
                {
                    referencedAssemblies.Add(Assembly.ReflectionOnlyLoad(a.FullName));
                }
                catch (Exception err)
                {
                    // TODO: handle this as a warning message in the analysis results
                }
            }

            this.referencedAssemblies = referencedAssemblies;
        }

        private AnalysisResult InspectAssemblyWithAnalyzer(IAssemblyAnalyzer analyzer)
        {
            try
            {
                if (loadedAssembly == null)
                    throw new InvalidOperationException("No assembly is loaded. An assembly has to be loaded before inspection attempt.");

                return analyzer.Analyze(loadedAssembly);
            }
            catch (FileLoadException)
            {
                return new AnalysisResult(analyzer.Name) { Value = FileLoadExceptionMessage };
            }
            catch (SecurityException)
            {
                return new AnalysisResult(analyzer.Name) { Value = SecurityExceptionMessage };
            }
            catch (Exception)
            {
                return new AnalysisResult(analyzer.Name) { Value = GeneralExceptionMessage };
            }
        }
    }
}
