using AssemblyPropertiesViewer.Analyzers.Models;
using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace AssemblyPropertiesViewer.ViewModel
{
    public class PropertiesViewModel : ViewModelBase
    {
        public string AssemblyName { get; private set; }
        public long AssemblySize { get; private set; }
        public IEnumerable<AnalysisResult> AssemblyAnalysisResults { get; private set; }

        public PropertiesViewModel(string assemblyName, long assemblySize, IEnumerable<AnalysisResult> assemblyAnalysisResults)
        {
            AssemblyName = assemblyName;
            AssemblySize = assemblySize;
            AssemblyAnalysisResults = assemblyAnalysisResults;
        }
    }
}
