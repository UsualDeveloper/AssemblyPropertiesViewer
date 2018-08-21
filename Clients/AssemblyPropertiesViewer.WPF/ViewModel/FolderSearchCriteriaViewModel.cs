using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using AssemblyPropertiesViewer.Services.Filtering;
using AssemblyPropertiesViewer.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AssemblyPropertiesViewer.ViewModel
{
    public class FolderSearchCriteriaViewModel : ViewModelBase
    {
        public IReadOnlyDictionary<string, IEnumerable<ISearchFilter>> SearchCriteria
        {
            get { return searchCriteria; }
            set
            {
                searchCriteria = value;

                UpdateFilterControls();
            }
        }
        IReadOnlyDictionary<string, IEnumerable<ISearchFilter>> searchCriteria;
        public bool SearchRecursively { get; set; } = true;

        public ICommand StartFolderSearchCommand { get; private set; }

        private readonly IFilteringControlCreationService fieldDefinitionVisitor;
        private readonly IWindowService windowService;

        public Dictionary<string, List<ItemsControlItem<FilterDefinitionControl>>> FilteringControls
        {
            get { return filteringControls; }
        }

        private Dictionary<string, List<ItemsControlItem<FilterDefinitionControl>>> filteringControls = new Dictionary<string, List<ItemsControlItem<FilterDefinitionControl>>>();

        public FolderSearchCriteriaViewModel(IFilteringControlCreationService fieldDefinitionVisitor, IWindowService windowService)
        {
            this.fieldDefinitionVisitor = fieldDefinitionVisitor;
            this.windowService = windowService;

            this.StartFolderSearchCommand = new RelayCommand<Window>(StartFolderSearchProcess);
        }
        
        private void StartFolderSearchProcess(Window window)
        {
            windowService.CloseWindowWithResult(window, true);
        }

        private void UpdateFilterControls()
        {
            filteringControls.Clear();

            if (searchCriteria == null)
                return;

            foreach (var searchCriteriaForAnalyzer in searchCriteria)
            {
                if (searchCriteriaForAnalyzer.Value == null)
                    continue;

                var searchCtrls = new List<ItemsControlItem<FilterDefinitionControl>>();

                foreach (var crit in searchCriteriaForAnalyzer.Value)
                {
                    crit.Accept(fieldDefinitionVisitor);

                    var filterAssignedControl = fieldDefinitionVisitor.FilterControl;
                    if (filterAssignedControl == null)
                        throw new ArgumentException($"No control defined for one of the filters assigned to analyzer type: {searchCriteriaForAnalyzer.Key}.");

                    searchCtrls.Add(new ItemsControlItem<FilterDefinitionControl> { ItemControl = filterAssignedControl });
                }

                filteringControls.Add(searchCriteriaForAnalyzer.Key, searchCtrls);
            }
        }
    }

    /// <summary>
    /// Additional class allowing to bind to ItemsControl with custom template defined 
    /// instead of triggering automatic item rendering.
    /// </summary>
    /// <typeparam name="T">Type of the control to be used.</typeparam>
    public class ItemsControlItem<T> where T: FrameworkElement
    {
        public T ItemControl { get; set; }
    }
}
