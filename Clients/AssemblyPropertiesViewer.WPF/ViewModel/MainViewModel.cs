using AssemblyPropertiesViewer.Services;
using AssemblyPropertiesViewer.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

        private readonly IWindowService windowService;

        private readonly ContextMenuViewModel contextMenuVM;

        public RelayCommand<DragEventArgs> DropAssemblyCommand { get; private set; }

        public ICommand ToggleTitleBarVisibilityCommand { get; private set; }

        public ICommand CloseApplicationCommand { get; private set; }

        public ICommand AnalyzeAssemblyCommand { get; private set; }

        public ICommand AnalyzeFolderCommand { get; private set; }

        public bool IsAnalysisInProgress
        {
            get { return analysisService.IsAnalysisInProgress; }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IAssemblyAnalysisService analysisService, IWindowService windowService)
        {
            if (IsInDesignMode)
                return;

            this.analysisService = analysisService;
            this.windowService = windowService;

            this.contextMenuVM = new ContextMenuViewModel(this);

            DropAssemblyCommand = new RelayCommand<DragEventArgs>(AnalyzeDroppedAssembly);

            ToggleTitleBarVisibilityCommand = contextMenuVM.ToggleTitleBarVisibilityCommand;
            CloseApplicationCommand = contextMenuVM.CloseApplicationCommand;
            AnalyzeAssemblyCommand = contextMenuVM.AnalyzeAssemblyCommand;
            AnalyzeFolderCommand = contextMenuVM.AnalyzeFolderCommand;
        }

        private void AnalyzeDroppedAssembly(DragEventArgs arg)
        {
            string filePath = GetFilePathForDroppedFileData(arg.Data);

            AnalyzeAssembly(filePath, arg.Source as DependencyObject);
        }

        private void SelectAndAnalyzeAssembly(DependencyObject sourceVisualElement)
        {
            const string FileOpenDlgTypeFilteringString = "Assemblies (*.dll)|*.dll|All files (*.*)|*.*";

            string selectedFilePath = windowService.OpenFileSelectionDialog(sourceVisualElement, FileOpenDlgTypeFilteringString);

            if (string.IsNullOrEmpty(selectedFilePath))
                return;

            AnalyzeAssembly(selectedFilePath, sourceVisualElement);
        }

        private void FindAssembliesByAnalysisResults(DependencyObject sourceVisualElement)
        {
            //TODO: invoke searching automatically if someone drags and drops a folder onto the main window
            string searchFolderPath = windowService.OpenFolderSelectionDialog(sourceVisualElement);

            if (string.IsNullOrEmpty(searchFolderPath))
                return;

            //TODO: replace with a proper view model transition
            var searchViewModel = new FolderSearchCriteriaViewModel(new FilterDefinitionControlCreationVisitor());
            searchViewModel.SearchCriteria = analysisService.GetAvailableSearchFilters();
            
            // if defining search filtering values was cancelled
            if (!windowService.OpenChildDialogWithResult<FolderSearchCriteriaWindow>(sourceVisualElement, searchViewModel))
                return;

            analysisService.InspectFolderAndFilterResults(searchFolderPath, searchViewModel.SearchRecursively, searchViewModel.SearchCriteria);
        }

        private string GetFilePathForDroppedFileData(IDataObject droppedFileData)
        {
            const string FileNameFormat = "FileNameW";

            if (!droppedFileData.GetDataPresent(FileNameFormat, false))
                return string.Empty;

            return Convert.ToString((droppedFileData.GetData(FileNameFormat) as string[]).Single());
        }

        private void AnalyzeAssembly(string filePath, DependencyObject visualElementInvokingAnalysis)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            long fileSize = analysisService.GetFileSize(filePath);
            var assemblyAnalysisResults = analysisService.InspectAssembly(filePath);
            var assemblyPropertiesViewModel = new PropertiesViewModel(filePath, fileSize, assemblyAnalysisResults);

            windowService.OpenChildWindow<PropertiesWindow>(visualElementInvokingAnalysis, assemblyPropertiesViewModel);
        }

        protected class ContextMenuViewModel
        {
            public ICommand ToggleTitleBarVisibilityCommand { get; private set; }
            public ICommand CloseApplicationCommand { get; private set; }
            public ICommand AnalyzeAssemblyCommand { get; private set; }
            public ICommand AnalyzeFolderCommand { get; private set; }

            MainViewModel mainWindowViewModel;

            public ContextMenuViewModel(MainViewModel mainWindowViewModel)
            {
                this.mainWindowViewModel = mainWindowViewModel;

                InitializeCommands();
            }

            private void InitializeCommands()
            {
                ToggleTitleBarVisibilityCommand = new RelayCommand<Window>(
                                                                    (Window window) => { window.WindowStyle = (window.WindowStyle != WindowStyle.None) ? WindowStyle.None : WindowStyle.SingleBorderWindow; },
                                                                    (Window window) => window.IsInitialized);

                CloseApplicationCommand  = new RelayCommand(
                                                        () => { App.Current.Shutdown(); },
                                                        () => true);

                AnalyzeAssemblyCommand = new RelayCommand<DependencyObject>(
                                                (DependencyObject sourceVisualElement) => { mainWindowViewModel.SelectAndAnalyzeAssembly(sourceVisualElement); },
                                                (DependencyObject sourceVisualElement) => !mainWindowViewModel.IsAnalysisInProgress);

                AnalyzeFolderCommand = new RelayCommand<DependencyObject>(
                                                (DependencyObject sourceVisualElement) => { mainWindowViewModel.FindAssembliesByAnalysisResults(sourceVisualElement); },
                                                (DependencyObject sourceVisualElement) => !mainWindowViewModel.IsAnalysisInProgress);
            }
        }
    }
}