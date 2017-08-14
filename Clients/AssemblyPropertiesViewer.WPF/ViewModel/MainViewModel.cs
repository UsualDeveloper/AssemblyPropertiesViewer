using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AssemblyPropertiesViewer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IAssemblyAnalysisService analysisService;

        private IEnumerable<AnalysisResult> assemblyAnalysisResults;

        public RelayCommand<DragEventArgs> DropAssemblyCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IAssemblyAnalysisService analysisService)
        {
            if (IsInDesignMode)
                return;

            this.analysisService = analysisService;

            DropAssemblyCommand = new RelayCommand<DragEventArgs>(AnalyzeDroppedAssembly);
        }
        
        public IEnumerable<AnalysisResult> AssemblyAnalysisResults
        {
            get { return assemblyAnalysisResults; }
            private set
            {
                assemblyAnalysisResults = value;

                RaisePropertyChanged(nameof(AssemblyAnalysisResults));
            }
        }

        private void AnalyzeDroppedAssembly(DragEventArgs arg)
        {
            AssemblyAnalysisResults = Enumerable.Empty<AnalysisResult>();

            string filePath = GetFilePathForDroppedFileData(arg.Data);

            if (string.IsNullOrEmpty(filePath))
                return;

            AssemblyAnalysisResults = analysisService.InspectAssembly(filePath);
        }


        private string GetFilePathForDroppedFileData(IDataObject droppedFileData)
        {
            const string fileNameFormat = "FileNameW";

            if (!droppedFileData.GetDataPresent(fileNameFormat, false))
                return string.Empty;

            return Convert.ToString((droppedFileData.GetData(fileNameFormat) as string[]).Single());
        }
    }
}