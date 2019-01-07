using AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces;
using System;

namespace AssemblyPropertiesViewer.Analyzers.Models.Filtering
{
    [Serializable]
    public class StringFilter : SearchFilterBase, ISearchFilter
    {
        public string MatchPattern { get; set; }

        public bool FullPatternMatchOnly { get; set; }

        public StringFilter(string name, string description) : base(name, description)
        {

        }

        public override void Accept(ISearchFilterVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
