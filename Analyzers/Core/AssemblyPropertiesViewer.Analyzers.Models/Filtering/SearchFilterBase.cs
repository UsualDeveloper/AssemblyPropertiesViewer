using AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces;
using System;

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
        public bool IsFilterEnabled { get; set; }

        public SearchFilterBase(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public abstract void Accept(ISearchFilterVisitor visitor);
    }
}
