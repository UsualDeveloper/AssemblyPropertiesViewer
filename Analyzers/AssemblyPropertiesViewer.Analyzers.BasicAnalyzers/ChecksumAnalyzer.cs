using AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base;
using AssemblyPropertiesViewer.Analyzers.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace AssemblyPropertiesViewer.Analyzers.BasicAnalyzers
{
    public class ChecksumAnalyzer : AnalyzerBase, IAssemblyAnalyzer
    {
        public string Name => "MD5 Checksum analyzer";

        public AnalysisResult Analyze(Assembly assembly)
        {
            var hashComputationResult = new AnalysisResult("MD5 Checksum");

            try
            {
                using (var fileStream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read))
                {
                    using (var hashAlgorithm = MD5.Create())
                    {
                        var hashBytes = hashAlgorithm.ComputeHash(fileStream);

                        var sb = new StringBuilder(hashBytes.Length*2);
                        foreach (var b in hashBytes)
                        {
                            sb.Append($"{b:x2}");
                        }

                        hashComputationResult.Value = sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                hashComputationResult.Value = $"Error: {ex.Message}";
            }

            return hashComputationResult;
        }
    }
}
