using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyPropertiesViewer.Analyzers.Models.Filtering
{
    /// <summary>
    /// The base filter definition type and all inheriting types shoul be marked as serializable in order to enBle passing them between domains.
    /// </summary>
    [Serializable]
    public abstract class SearchFilterBase
    {
        /// <summary>
        /// Name of the filter, should be unique in a scope on one analyzer.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Text description of the filter.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Flag indicating if specific filter criteria model is enabled (or marked as active) and should be used in the filtering process.
        /// </summary>
        public bool IsFilterEnabled { get; set; } = true;

        public SearchFilterBase(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
        
        public abstract void Accept(ISearchFilterVisitor visitor);
    }

    public interface ISearchFilterVisitor
    {
        void Visit(StringFilter filter);

        void Visit(BooleanFilter filter);

        void Visit(DropDownFilter filter);
    }

    public interface ISearchFilter
    {
        string Name { get; }

        string Description { get; }

        bool IsFilterEnabled { get; set; }

        void Accept(ISearchFilterVisitor visitor);
    }

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
