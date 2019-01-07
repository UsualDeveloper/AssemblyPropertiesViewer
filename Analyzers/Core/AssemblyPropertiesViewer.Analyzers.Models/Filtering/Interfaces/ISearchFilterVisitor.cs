namespace AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces
{
    /// <summary>
    /// Interface for visitors that are associated with search filter classes.
    /// </summary>
    public interface ISearchFilterVisitor
    {
        void Visit(StringFilter filter);

        void Visit(BooleanFilter filter);

        void Visit(DropDownFilter filter);
    }
}
