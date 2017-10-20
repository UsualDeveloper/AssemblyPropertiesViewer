using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Messages;
using AssemblyPropertiesViewer.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
        
        private void AnalyzeDroppedAssembly(DragEventArgs arg)
        {
            string filePath = GetFilePathForDroppedFileData(arg.Data);

            if (string.IsNullOrEmpty(filePath))
                return;

            long fileSize = analysisService.GetFileSize(filePath);
            var assemblyAnalysisResults = analysisService.InspectAssembly(filePath);

            var assemblyPropertiesViewModel = new PropertiesViewModel(filePath, fileSize, assemblyAnalysisResults);
            ShowAnalysisResults(filePath, assemblyPropertiesViewModel);
        }

        private void ShowAnalysisResults(string filePath, PropertiesViewModel assemblyPropertiesViewModel)
        {
            Messenger.Default.Send(new ShowPropertiesMessage(assemblyPropertiesViewModel));
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