using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using AssemblyPropertiesViewer.Controls;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    public interface IFilteringControlCreationService : ISearchFilterVisitor
    {
        FilterDefinitionControl FilterControl { get; }
    }
}
