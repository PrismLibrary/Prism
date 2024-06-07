using System.ComponentModel;

#nullable enable
namespace Prism.Mvvm;

/// <summary>
/// Exception thrown when an error occurs during ViewModel creation.
/// </summary>
public class ViewModelCreationException : Exception
{
    private static Func<object, string>? _viewNameDelegate;

    /// <summary>
    /// Gets the name of the view associated with the exception, based on platform-specific logic.
    /// </summary>
    /// <param name="view">The view instance for which ViewModel creation failed.</param>
    /// <returns>The name of the view, or "Platform not initialized" if the platform is not initialized.</returns>
    private static string GetViewName(object view) => _viewNameDelegate is null ? "Platform not initialized" : _viewNameDelegate(view);

    /// <summary>
    /// Sets the delegate used to retrieve view names for exceptions.
    /// </summary>
    /// <param name="viewNameDelegate">The delegate that retrieves view names.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetViewNameDelegate(Func<object, string> viewNameDelegate) => _viewNameDelegate = viewNameDelegate;

    /// <summary>
    /// Initializes a new instance of the ViewModelCreationException class with the specified view and inner exception.
    /// </summary>
    /// <param name="view">The view for which ViewModel creation failed.</param>
    /// <param name="innerException">The inner exception that caused the ViewModel creation to fail.</param>
    public ViewModelCreationException(object view, Exception innerException)
        : base($"Unable to Create ViewModel for '{view.GetType().FullName}'.", innerException)
    {
        View = view;
        ViewName = GetViewName(view);
    }

    /// <summary>
    /// Gets the view instance for which ViewModel creation failed.
    /// </summary>
    public object View { get; }

    /// <summary>
    /// Gets the name of the view associated with the exception.
    /// </summary>
    public string ViewName { get; }
}
