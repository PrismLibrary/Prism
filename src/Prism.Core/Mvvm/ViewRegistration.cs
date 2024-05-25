namespace Prism.Mvvm;

/// <summary>
/// Represents information about a registered view.
/// </summary>
public record ViewRegistration
{
    /// <summary>
    /// Gets the type of view this registration represents (Page, Region, or Dialog).
    /// </summary>
    public ViewType Type { get; init; }

    /// <summary>
    /// Gets the type of the view class associated with this registration.
    /// </summary>
    public Type View { get; init; }

    /// <summary>
    /// Gets the type of the view model associated with this registration, if any.
    /// </summary>
    public Type ViewModel { get; init; }

    /// <summary>
    /// Gets the unique name used to identify this view registration.
    /// </summary>
    public string Name { get; init; }
}

