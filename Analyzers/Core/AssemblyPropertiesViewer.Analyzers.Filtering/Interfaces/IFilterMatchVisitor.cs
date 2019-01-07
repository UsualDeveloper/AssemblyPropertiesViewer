using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces;

namespace AssemblyPropertiesViewer.Analyzers.Filtering.Interfaces
{
    public interface IFilterMatchVisitor : ISearchFilterVisitor
    {
        void InitializeVisitorForAccept(AnalysisResult fileAnalysisResults);
        bool IsAcceptedFilterMatching { get; }
    }
}
