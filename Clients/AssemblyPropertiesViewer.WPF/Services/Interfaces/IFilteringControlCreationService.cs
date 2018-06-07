using AssemblyPropertiesViewer.Analyzers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    public interface IFilteringControlCreationService : ISearchFilterDefinitionVisitor
    {
        FrameworkElement FilteringControl { get; }
    }
}
