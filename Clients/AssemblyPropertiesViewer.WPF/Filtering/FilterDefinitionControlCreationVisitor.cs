using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AssemblyPropertiesViewer.Services.Filtering
{
    public class FilterDefinitionControlCreationVisitor : IFilteringControlCreationService
    {
        public FrameworkElement FilterControl
        {
            get
            {
                AssertWasVisitorAccepted();

                return filteringControl;
            }
            private set
            {
                MarkVisitorAccepted();

                filteringControl = value;
            }
        }

        private bool wasControlCreationInvoked = false;
        private FrameworkElement filteringControl;

        public void Visit(DropDownFilter filter)
        {
            AssertIsValidFilterInstance(filter);

            // TODO: change into user control to keep view-specific information in the XAML
            ComboBox availableValuesCtrl = new ComboBox();

            availableValuesCtrl.ItemsSource = filter.AvailableValues;

            if (filter.SelectedValue != null)
            {
                availableValuesCtrl.SelectedValue = filter.SelectedValue;
            }
            SetBindingToControl("SelectedValue", ComboBox.SelectedValueProperty, availableValuesCtrl);

            //FilterControlDefinition = new FilterDefinition("-"); // this probably must be a class for the bindings to work properly
            //FilterControlDefinition.FilterControl = availableValuesCtrl;
            FilterControl = availableValuesCtrl;
        }

        public void Visit(StringFilter filter)
        {
            AssertIsValidFilterInstance(filter);

            // TODO: change into user control to keep view-specific information in the XAML
            TextBox textBox = new TextBox();
            textBox.ToolTip = filter.Description;
            textBox.DataContext = filter;
            SetBindingToControl("MatchPattern", TextBox.TextProperty, textBox);

            FilterControl = GetControlsSetForFilter(textBox, filter);
        }

        public void Visit(BooleanFilter filter)
        {
            AssertIsValidFilterInstance(filter);

            // TODO: change into user control to keep view-specific information in the XAML
            CheckBox checkBox = new CheckBox();
            checkBox.ToolTip = filter.Description;
            checkBox.DataContext = filter;
            SetBindingToControl("IsSelected", CheckBox.IsCheckedProperty, checkBox);

            FilterControl = GetControlsSetForFilter(checkBox, filter);
        }

        /// <summary>
        /// Default method to be called for filter types for that type-specific handling was not implemented.
        /// </summary>
        /// <param name="filter"></param>
        public void Visit(ISearchFilter filter)
        {
            AssertIsValidFilterInstance(filter);

            // TODO: change into user control to keep view-specific information in the XAML
            Label filteringNotSupportedInfo = new Label();
            filteringNotSupportedInfo.Content = "Filtering control is not available for this filter type.";
            filteringNotSupportedInfo.ToolTip = filter.Description;

            FilterControl = GetControlsSetForFilter(filteringNotSupportedInfo, filter);
            MarkVisitorAccepted();
        }

        private void SetBindingToControl<T>(string bindingPath, DependencyProperty propertyToBind, T controlToBind) where T : FrameworkElement
        {
            var binding = new Binding();
            binding.Path = new PropertyPath(bindingPath);
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

        private void AssertWasVisitorAccepted()
        {
            if (!wasControlCreationInvoked)
                throw new InvalidOperationException("Please use the visitor in Accept method of appropriate element to create the control before getting its value.");
        }

        private void MarkVisitorAccepted()
        {
            if (!wasControlCreationInvoked)
                wasControlCreationInvoked = true;
        }

        /// <summary>
        /// Method that creates simple controls set for a simple single control input filter. 
        /// </summary>
        /// <param name="inputCtrl">Main input element that allows to specify criteria.</param>
        /// <param name="filter">Associated filter definition instance.</param>
        /// <returns></returns>
        private FrameworkElement GetControlsSetForFilter(FrameworkElement inputCtrl, ISearchFilter filter)
        {
            var panel = new Grid();
            panel.ColumnDefinitions.Add(new ColumnDefinition());
            panel.ColumnDefinitions.Add(new ColumnDefinition());

            var label = new TextBlock() { Text = filter.Name };
            panel.Children.Add(label);
            Grid.SetColumn(label, 0);

            panel.Children.Add(inputCtrl);
            Grid.SetColumn(inputCtrl, 1);

            return panel;
        }
    }
}
