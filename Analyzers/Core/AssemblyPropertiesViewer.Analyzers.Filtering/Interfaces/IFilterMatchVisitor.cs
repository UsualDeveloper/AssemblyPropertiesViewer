using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyPropertiesViewer.Analyzers.Filtering.Interfaces
{
    public interface IFilterMatchVisitor : ISearchFilterVisitor
    {
        void InitializeVisitorForAccept(AnalysisResult fileAnalysisResults);
        bool IsAcceptedFilterMatching { get; }
    }
}
