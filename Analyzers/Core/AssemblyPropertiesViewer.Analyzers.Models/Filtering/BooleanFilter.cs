using AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces;
using System;

namespace AssemblyPropertiesViewer.Analyzers.Models.Filtering
{
    [Serializable]
    public class BooleanFilter : SearchFilterBase, ISearchFilter
    {
        public bool IsSelected { get; set; }

        public BooleanFilter(string name, string description) : base(name, description)
        {

        }

        public override void Accept(ISearchFilterVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
