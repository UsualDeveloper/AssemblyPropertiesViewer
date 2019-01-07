namespace AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces
{
    /// <summary>
    /// Interface that should be implemented by each search filter type defined.
    /// </summary>
    public interface ISearchFilter
    {
        string Name { get; }

        string Description { get; }

        bool IsFilterEnabled { get; set; }

        void Accept(ISearchFilterVisitor visitor);
    }
}
