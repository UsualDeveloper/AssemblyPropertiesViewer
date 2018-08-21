using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using AssemblyPropertiesViewer.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AssemblyPropertiesViewer.Services.Filtering
{
    public class FilterDefinitionControlCreationVisitor : IFilteringControlCreationService
    {
        public FilterDefinitionControl FilterControl
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
        private FilterDefinitionControl filteringControl;

        public void Visit(DropDownFilter filter)
        {
            AssertIsValidFilterInstance(filter);

            // TODO: change into user control to keep view-specific information in the XAML
            ComboBox availableValuesCtrl = new ComboBox();

            availableValuesCtrl.ItemsSource = filter.AvailableValues;
            availableValuesCtrl.SelectedValuePath = "Key";
            availableValuesCtrl.DisplayMemberPath = "Value";

            if (filter.SelectedValue != null)
            {
                availableValuesCtrl.SelectedValue = filter.SelectedValue;
            }
            SetBindingToControl(nameof(DropDownFilter.SelectedValue), ComboBox.SelectedValueProperty, availableValuesCtrl);
            
            FilterControl = GetControlsSetForFilter(availableValuesCtrl, filter);
        }

        public void Visit(StringFilter filter)
        {
            AssertIsValidFilterInstance(filter);

            // TODO: change into user control to keep view-specific information in the XAML
            TextBox textBox = new TextBox();
            textBox.ToolTip = filter.Description;
            textBox.DataContext = filter;
            SetBindingToControl(nameof(StringFilter.MatchPattern), TextBox.TextProperty, textBox);

            FilterControl = GetControlsSetForFilter(textBox, filter);
        }

        public void Visit(BooleanFilter filter)
        {
            AssertIsValidFilterInstance(filter);

            // TODO: change into user control to keep view-specific information in the XAML
            CheckBox checkBox = new CheckBox();
            checkBox.ToolTip = filter.Description;
            checkBox.DataContext = filter;
            SetBindingToControl(nameof(BooleanFilter.IsSelected), CheckBox.IsCheckedProperty, checkBox);

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
        private FilterDefinitionControl GetControlsSetForFilter(FrameworkElement inputCtrl, ISearchFilter filter)
        {
            var filterControl = new FilterDefinitionControl();
            
            filterControl.ColumnDefinitions.Add(new ColumnDefinition());
            filterControl.ColumnDefinitions.Add(new ColumnDefinition());
            
            var label = new TextBlock() { Text = filter.Name };
            filterControl.Children.Add(label);
            Grid.SetColumn(label, 0);

            filterControl.Children.Add(inputCtrl);
            Grid.SetColumn(inputCtrl, 1);

            filterControl.DataContext = filter;
            //TODO: verify binding mode
            SetBindingToControl(nameof(ISearchFilter.IsFilterEnabled), FilterDefinitionControl.IsFilterEnabledProperty, filterControl, BindingMode.TwoWay);

            SetBindingToControl(nameof(ISearchFilter.IsFilterEnabled), FilterDefinitionControl.IsEnabledProperty, filterControl, BindingMode.OneWay);
            
            return filterControl;
        }
    }

    // TODO: move this class to appropriate place
    public class FilterDefinitionControl : Grid
    {
        public static readonly DependencyProperty IsFilterEnabledProperty = DependencyProperty.Register(
            nameof(IsFilterEnabled), 
            typeof(bool), typeof(FilterDefinitionControl), 
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsFilterEnabledPropertyChanged)));
        
        public bool IsFilterEnabled
        {
            get { return (bool)GetValue(IsFilterEnabledProperty); }
            set { SetValue(IsFilterEnabledProperty, value); }
        }
        

        private static void IsFilterEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // no additional handling required yet
        }
    }
}
