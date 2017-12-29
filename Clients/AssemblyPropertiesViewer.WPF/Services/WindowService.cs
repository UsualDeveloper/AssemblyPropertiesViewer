using AssemblyPropertiesViewer.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AssemblyPropertiesViewer.Services
{
    internal class WindowService : IWindowService
    {
        public void OpenChildWindow<T>(DependencyObject elementInParentWindow, object dataContext) where T : Window, new()
        {
            var parentWindow = GetWindowByContainedElement(elementInParentWindow);

            var childWindow = new T();

            childWindow.Owner = parentWindow;
            childWindow.DataContext = dataContext;

            childWindow.Show();
        }

        /// <summary>
        /// Opens file selection dialog with file type filtering, if filters were specified.
        /// </summary>
        /// <param name="elementInOwnerWindow">DependencyObject attached to the window to be set as owner of the file selection dialog.</param>
        /// <param name="filters">Collection of dialog file type filters with extensions as keys and description as values (sample pair key and value: "[*.txt; Text files (*.txt)]").</param>
        /// <returns>Path to the selected file or an empty string if no file was selected.</returns>
        public string OpenFileSelectionDialog(DependencyObject elementInOwnerWindow, IReadOnlyDictionary<string, string> filtersCollection = null)
        {
            string filtersString = CreateFiltersString(filtersCollection);

            return OpenFileSelectionDialog(elementInOwnerWindow, filtersString);
        }

        /// <summary>
        /// Opens file selection dialog with file type filtering, if filters were specified.
        /// </summary>
        /// <param name="elementInOwnerWindow">DependencyObject attached to the window to be set as owner of the file selection dialog.</param>
        /// <param name="filtersString">File type filters in a string format (for example: "Text files (*.txt)|*.txt|All files (*.*)|*.*").</param>
        /// <returns>Path to the selected file or an empty string if no file was selected.</returns>
        public string OpenFileSelectionDialog(DependencyObject elementInOwnerWindow, string filtersString = null)
        {
            OpenFileDialog openDlg = new OpenFileDialog();

            if (!string.IsNullOrEmpty(filtersString))
            {
                openDlg.Filter = filtersString;
            }

            var parentWindow = GetWindowByContainedElement(elementInOwnerWindow);
            if (openDlg.ShowDialog(parentWindow) ?? false)
            {
                return openDlg.FileName;
            }

            return string.Empty;
        }

        private string CreateFiltersString(IReadOnlyDictionary<string, string> filtersCollection)
        {
            if (filtersCollection == null)
                return string.Empty;

            var sb = new StringBuilder();
            bool isAnyFilterDefined = false;
            foreach (var filter in filtersCollection)
            {
                if (string.IsNullOrWhiteSpace(filter.Key) || string.IsNullOrWhiteSpace(filter.Value))
                    throw new ArgumentException("Incorrect filters definition for the dialog. Both filter and its description must be of non-empty values.");

                if (isAnyFilterDefined)
                {
                    sb.Append("|");
                }
                else
                {
                    isAnyFilterDefined = true;
                }

                sb.AppendFormat("{0}|{1}", filter.Value, filter.Key);
            }

            if (isAnyFilterDefined)
            {
                return sb.ToString();
            }

            return string.Empty;
        }

        private Window GetWindowByContainedElement(DependencyObject elementInWindow)
        {
            if (elementInWindow == null)
                throw new ArgumentNullException(nameof(elementInWindow));

            return Window.GetWindow(elementInWindow);
        }
    }
}
