namespace Prism.Navigation;

/// <summary>Describes a change of the selected tab in a tabbed page attached to a window.</summary>
public record TabChangedContext
{
    /// <summary>The tabbed page whose selection changed.</summary>
    public TabbedPage TabbedPage { get; init; }
    /// <summary>The previously selected child, which may be a navigation page.</summary>
    public Page PreviousTab { get; init; }
    /// <summary>The newly selected child, which may be a navigation page.</summary>
    public Page CurrentTab { get; init; }
}
