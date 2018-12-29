using AssemblyPropertiesViewer.Analyzers.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyPropertiesViewer.ViewModel
{
    public class MultipleFilesAnalysisResultsViewModel
    {
        public string AnalyzedFolderPath { get; set; }

        public bool IsAnalysisRecursive { get; set; }

        public IReadOnlyDictionary<string, IEnumerable<AnalysisResult>> FilesAnalysisResults
        {
            get { return filesAnalysisResults; }
            set
            {
                filesAnalysisResults = value;

                UpdateResultsDataModel(filesAnalysisResults);
            }
        }

        private IReadOnlyDictionary<string, IEnumerable<AnalysisResult>> filesAnalysisResults;

        public IEnumerable<dynamic> ResultsDataModel { get; private set; }

        private void UpdateResultsDataModel(IReadOnlyDictionary<string, IEnumerable<AnalysisResult>> filesAnalysisResults)
        {
            ResultsDataModel = TransformAnalysisResultsToDynamicModel(filesAnalysisResults);
        }

        private IEnumerable<dynamic> TransformAnalysisResultsToDynamicModel(IReadOnlyDictionary<string, IEnumerable<AnalysisResult>> filesAnalysisResults)
        {
            var dataGridModel = new List<dynamic>();

            foreach (var oneFileAnalysis in filesAnalysisResults)
            {
                dynamic fileAnalysisModel = new ExpandoObject();

                fileAnalysisModel.FileName = Path.GetFileName(oneFileAnalysis.Key);

                fileAnalysisModel.Directory = Path.GetDirectoryName(oneFileAnalysis.Key);

                var fileAnalysisModelDict = (fileAnalysisModel as IDictionary<string, object>);
                foreach (var analyzersInfo in oneFileAnalysis.Value)
                {
                    AssertUniqueColumnNameForAnalysisResult(analyzersInfo.AssemblyPropertyName, fileAnalysisModelDict);

                    fileAnalysisModelDict[analyzersInfo.AssemblyPropertyName] = analyzersInfo.Value;
                }

                dataGridModel.Add(fileAnalysisModel);
            }

            return dataGridModel;
        }

        private void AssertUniqueColumnNameForAnalysisResult(string assemblyPropertyName, IDictionary<string, object> fileAnalysisModelDict)
        {
            if (fileAnalysisModelDict.ContainsKey(assemblyPropertyName))
            {
                throw new InvalidOperationException("Colliding column names: two or more analyzers use the same name for some of their results.");
            }
        }
    }
}
