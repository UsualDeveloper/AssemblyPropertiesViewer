using System;
using System.Windows;
using System.Windows.Controls;

namespace AssemblyPropertiesViewer.Controls
{
    public class FilterDefinitionControl : Grid
    {
        public static readonly DependencyProperty IsFilterEnabledProperty = DependencyProperty.Register(
            nameof(IsFilterEnabled),
            typeof(bool), typeof(FilterDefinitionControl),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsFilterEnabledPropertyChanged)));

        /// <summary>
        /// Flag indicating is filter associated with the control is enabled.
        /// </summary>
        public bool IsFilterEnabled
        {
            get { return (bool)GetValue(IsFilterEnabledProperty); }
            set { SetValue(IsFilterEnabledProperty, value); }
        }

        private TextBlock filterNameLabel;
        private Panel filterSpecificFilteringPanel;
        private bool isLayoutInitialized = false;

        public FilterDefinitionControl() : base()
        {
            SetupLayout();
        }

        public void SetFilterSpecificControls(string filterName, FrameworkElement filteringControl)
        {
            filterNameLabel.Text = filterName;

            filterSpecificFilteringPanel.Children.Clear();
            filterSpecificFilteringPanel.Children.Add(filteringControl);
        }

        private void SetupLayout()
        {
            if (isLayoutInitialized)
            {
                throw new InvalidOperationException("Control internal layout is already initialized.");
            }

            ColumnDefinitions.Clear();
            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());

            filterNameLabel = new TextBlock();
            Children.Add(filterNameLabel);
            Grid.SetColumn(filterNameLabel, 0);

            filterSpecificFilteringPanel = new Grid();
            Children.Add(filterSpecificFilteringPanel);
            Grid.SetColumn(filterSpecificFilteringPanel, 1);

            isLayoutInitialized = true;
        }

        private static void IsFilterEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // no additional handling required yet
        }
    }
}
