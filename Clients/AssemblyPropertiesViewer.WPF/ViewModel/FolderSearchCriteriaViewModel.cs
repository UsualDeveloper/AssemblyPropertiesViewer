﻿using AssemblyPropertiesViewer.Analyzers.Models;
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

namespace AssemblyPropertiesViewer.ViewModel
{
    public class FolderSearchCriteriaViewModel : ViewModelBase
    {
        public IReadOnlyDictionary<Type, IEnumerable<ISearchFilter>> SearchCriteria
        {
            get { return searchCriteria; }
            set
            {
                searchCriteria = value;

                UpdateFilterControls();
            }
        }
        IReadOnlyDictionary<Type, IEnumerable<ISearchFilter>> searchCriteria;
        public bool SearchRecursively { get; set; } = true;

        public RelayCommand<RoutedEventArgs> StartFolderSearchCommand { get; private set; }

        private readonly IFilteringControlCreationService fieldDefinitionVisitor;

        public Dictionary<Type, List<FilterDefinitionItemControl>> FilteringControls
        {
            get { return filteringControls; }
        }

        private Dictionary<Type, List<FilterDefinitionItemControl>> filteringControls = new Dictionary<Type, List<FilterDefinitionItemControl>>();

        public FolderSearchCriteriaViewModel(IFilteringControlCreationService fieldDefinitionVisitor)
        {
            this.fieldDefinitionVisitor = fieldDefinitionVisitor;

            this.StartFolderSearchCommand = new RelayCommand<RoutedEventArgs>(StartFolderSearchProcess);
        }

        private void StartFolderSearchProcess(RoutedEventArgs e)
        {

        }

        void UpdateFilterControls()
        {
            filteringControls.Clear();

            if (searchCriteria == null)
                return;

            foreach (var searchCriteriaForAnalyzer in searchCriteria)
            {
                if (searchCriteriaForAnalyzer.Value == null)
                    continue;

                var searchCtrls = new List<FilterDefinitionItemControl>();

                foreach (var crit in searchCriteriaForAnalyzer.Value)
                {
                    crit.Accept(fieldDefinitionVisitor);

                    var filterAssignedControl = fieldDefinitionVisitor.FilteringControl;
                    if (filterAssignedControl == null)
                        throw new ArgumentException($"No control defined for one of the filters assigned to analyzer type: {searchCriteriaForAnalyzer.Key.FullName}.");

                    searchCtrls.Add(new FilterDefinitionItemControl() { FilterControl = filterAssignedControl } );
                }

                filteringControls.Add(searchCriteriaForAnalyzer.Key, searchCtrls);
            }
        }
    }

    public class FilterDefinitionItemControl
    {
        public bool IsFilterEnabled { get; set; } = true;

        public FrameworkElement FilterControl { get; set; }
    }
}
