using AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces;
using System;
using System.Collections.Specialized;

namespace AssemblyPropertiesViewer.Analyzers.Models.Filtering
{
    [Serializable]
    public class DropDownFilter : SearchFilterBase, ISearchFilter
    {
        public StringDictionary AvailableValues { get; set; }

        public string SelectedValue
        {
            get
            {
                return selectedValue;
            }
            set
            {
                if (AvailableValues == null || !AvailableValues.ContainsKey(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The value specified does not exist in available values collection.");
                }

                selectedValue = value;
            }
        }

        private string selectedValue;

        public DropDownFilter(string name, string description) : base(name, description)
        {

        }

        public override void Accept(ISearchFilterVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
