using System.Collections.Generic;
using System.Windows;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    public interface IWindowService
    {
        /// <summary>
        /// Opens new window as a child window of a specific parent. The owner of the window is set as window containing specific calling element.
        /// </summary>
        /// <typeparam name="T">Type of the child window to be opened.</typeparam>
        /// <param name="elementInParentWindow">Element of the window to be set as parent of the window to be shown.</param>
        /// <param name="dataContext">Data context object to be set for the child window.</param>
        void OpenChildWindow<T>(DependencyObject elementInParentWindow, object dataContext) where T : Window, new();

        /// <summary>
        /// Opens file selection dialog with file type filtering, if filters were specified.
        /// </summary>
        /// <param name="elementInOwnerWindow">DependencyObject attached to the window to be set as owner of the file selection dialog.</param>
        /// <param name="filtersString">File type filters in a string format (for example: "Text files (*.txt)|*.txt|All files (*.*)|*.*").</param>
        /// <returns>Path to the selected file or an empty string if no file was selected.</returns>
        string OpenFileSelectionDialog(DependencyObject elementInOwnerWindow, string filtersString = null);
    }
}
