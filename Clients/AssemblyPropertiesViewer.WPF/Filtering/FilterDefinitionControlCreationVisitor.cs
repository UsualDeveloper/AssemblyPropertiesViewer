using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using AssemblyPropertiesViewer.Controls;
using AssemblyPropertiesViewer.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AssemblyPropertiesViewer.Services.Filtering
{
    /// <summary>
    /// Visitor class creating appropriate filter defining control for specific filter type.
    /// </summary>
    public class FilterDefinitionControlCreationVisitor : IFilteringControlCreationService
    {
        public FilterDefinitionControl FilterControl
        {
            get
            {
                AssertIsFilteringControlCreated();

                return filteringControl;
            }
            private set
            {
                MarkFilteringControlAsCreated();

                filteringControl = value;
            }
        }

        private bool wasControlCreationInvoked = false;
        private FilterDefinitionControl filteringControl;

        public void Visit(DropDownFilter filter)
        {
            AssertIsValidFilterInstance(filter);
            
            ComboBox availableValuesCtrl = new ComboBox();

            availableValuesCtrl.ItemsSource = filter.AvailableValues;
            availableValuesCtrl.SelectedValuePath = "Key";
            availableValuesCtrl.DisplayMemberPath = "Value";

            if (filter.SelectedValue != null)
            {
                availableValuesCtrl.SelectedValue = filter.SelectedValue;
            }
            SetBindingToControl(nameof(DropDownFilter.SelectedValue), ComboBox.SelectedValueProperty, availableValuesCtrl);
            
            FilterControl = GetControlForFilter(availableValuesCtrl, filter);
        }
        
        public void Visit(StringFilter filter)
        {
            AssertIsValidFilterInstance(filter);
            
            TextBox textBox = new TextBox();
            textBox.ToolTip = filter.Description;
            textBox.DataContext = filter;
            SetBindingToControl(nameof(StringFilter.MatchPattern), TextBox.TextProperty, textBox);

            FilterControl = GetControlForFilter(textBox, filter);
        }
        
        public void Visit(BooleanFilter filter)
        {
            AssertIsValidFilterInstance(filter);
            
            CheckBox checkBox = new CheckBox();
            checkBox.ToolTip = filter.Description;
            checkBox.DataContext = filter;
            SetBindingToControl(nameof(BooleanFilter.IsSelected), CheckBox.IsCheckedProperty, checkBox);

            FilterControl = GetControlForFilter(checkBox, filter);
        }

        /// <summary>
        /// Default method to be called for filter types for that type-specific handling was not implemented.
        /// </summary>
        /// <param name="filter">Filter to handle.</param>
        /// <remarks>
        /// This is the default Visit method that should be called only for parameter of type 
        /// that is not handled by Visit methods defined for specific filter types.
        /// </remarks>
        public void Visit(ISearchFilter filter)
        {
            AssertIsValidFilterInstance(filter);
            
            Label filteringNotSupportedInfo = new Label();
            filteringNotSupportedInfo.Content = "Filtering control is not available for this filter type.";
            filteringNotSupportedInfo.ToolTip = filter.Description;

            FilterControl = GetControlForFilter(filteringNotSupportedInfo, filter);
        }

        private void SetBindingToControl<T>(string bindingPath, DependencyProperty propertyToBind, T controlToBind, BindingMode bindingMode = BindingMode.Default) where T : FrameworkElement
        {
            var binding = new Binding();
            binding.Path = new PropertyPath(bindingPath);
            binding.Mode = bindingMode;

            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(controlToBind, propertyToBind, binding);
        }

        private void AssertIsValidFilterInstance(ISearchFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
        }

        private void AssertIsFilteringControlCreated()
        {
            if (!wasControlCreationInvoked)
                throw new InvalidOperationException("Please use the visitor in Accept method of appropriate element to create the control before getting its value.");
        }

        private void MarkFilteringControlAsCreated()
        {
            if (!wasControlCreationInvoked)
                wasControlCreationInvoked = true;
        }

        /// <summary>
        /// Method that creates controls set for a single control input filter. 
        /// </summary>
        /// <param name="inputCtrl">Main input element that allows to specify criteria.</param>
        /// <param name="filter">Associated filter definition instance.</param>
        /// <returns>Controls set for defining a single filter matching criteria.</returns>
        private FilterDefinitionControl GetControlForFilter(FrameworkElement inputCtrl, ISearchFilter filter)
        {
            var filterControl = new FilterDefinitionControl();
            
            filterControl.SetFilterSpecificControls(filter.Name, inputCtrl);
            filterControl.DataContext = filter;
            
            SetBindingToControl(nameof(ISearchFilter.IsFilterEnabled), FilterDefinitionControl.IsFilterEnabledProperty, filterControl, BindingMode.TwoWay);
            SetBindingToControl(nameof(ISearchFilter.IsFilterEnabled), FilterDefinitionControl.IsEnabledProperty, filterControl, BindingMode.OneWay);
            
            return filterControl;
        }
    }
}
