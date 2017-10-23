using AssemblyPropertiesViewer.Services.Interfaces;
using System;
using System.Windows;

namespace AssemblyPropertiesViewer.Services
{
    internal class WindowService : IWindowService
    {
        public void OpenChildWindow<T>(DependencyObject elementInParentWindow, object dataContext) where T : Window, new()
        {
            if (elementInParentWindow == null)
                throw new ArgumentNullException(nameof(elementInParentWindow));

            var childWindow = new T();

            childWindow.Owner = Window.GetWindow(elementInParentWindow);
            childWindow.DataContext = dataContext;

            childWindow.Show();
        }
    }
}
