#nullable enable
namespace Prism.Mvvm;

/// <summary>
/// Exception thrown when an error occurs during view creation.
/// </summary>
public class ViewCreationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ViewCreationException class with the specified view name and view type.
    /// </summary>
    /// <param name="viewName">The name of the view that failed to create.</param>
    /// <param name="viewType">The type of view that failed to create (Page, Region, or Dialog).</param>
    public ViewCreationException(string viewName, ViewType viewType)
        : this(viewName, viewType, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ViewCreationException class with the specified view name, view type, and inner exception.
    /// </summary>
    /// <param name="viewName">The name of the view that failed to create.</param>
    /// <param name="viewType">The type of view that failed to create (Page, Region, or Dialog).</param>
    /// <param name="innerException">The inner exception that caused the view creation to fail.</param>
    public ViewCreationException(string viewName, ViewType viewType, Exception? innerException)
        : base($"Unable to create {viewType} '{viewName}'.", innerException)
    {
        ViewName = viewName;
        ViewType = viewType;
    }

    /// <summary>
    /// Gets the type of view that failed to create (Page, Region, or Dialog).
    /// </summary>
    public ViewType ViewType { get; }

    /// <summary>
    /// Gets the name of the view that failed to create.
    /// </summary>
    public string ViewName { get; }
}
