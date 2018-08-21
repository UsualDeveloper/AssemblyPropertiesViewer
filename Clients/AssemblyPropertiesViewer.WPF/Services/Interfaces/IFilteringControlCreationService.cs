using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using AssemblyPropertiesViewer.Services.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    public interface IFilteringControlCreationService : ISearchFilterVisitor
    {
        FilterDefinitionControl FilterControl { get; }
    }
}
