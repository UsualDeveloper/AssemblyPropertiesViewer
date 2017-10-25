using AssemblyPropertiesViewer.Analyzers.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;

namespace AssemblyPropertiesViewer.ViewModel
{
    public class PropertiesViewModel : ViewModelBase
    {
        public string AssemblyPath { get; private set; }

        public string AssemblyName { get; private set; }

        public long AssemblySize { get; private set; }

        public IEnumerable<AnalysisResult> AssemblyAnalysisResults { get; private set; }

        public PropertiesViewModel(string assemblyPath, long assemblySize, IEnumerable<AnalysisResult> assemblyAnalysisResults)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentNullException(nameof(assemblyPath));

            if (assemblySize < 0)
                throw new ArgumentOutOfRangeException(nameof(assemblySize), "Assembly size must be defined as an integer of a non-negative value.");

            if (assemblyAnalysisResults == null)
                throw new ArgumentNullException(nameof(assemblyAnalysisResults));

            this.AssemblyPath = assemblyPath;
            this.AssemblySize = assemblySize;
            this.AssemblyAnalysisResults = assemblyAnalysisResults;


            this.AssemblyName = GetAssemblyFileName(assemblyPath);
            
        }

        private string GetAssemblyFileName(string assemblyPath)
        {
            try
            {
                return Path.GetFileName(assemblyPath);
            }
            catch (ArgumentException)
            {
                return "/cannot get file name/";
            }
        }
    }
}
