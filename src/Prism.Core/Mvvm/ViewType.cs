namespace Prism.Mvvm;

/// <summary>
/// Enumerates the different types of views supported by the framework.
/// </summary>
public enum ViewType
{
    /// <summary>
    /// Unknown view type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Represents a full-screen page or window.
    /// </summary>
    Page,

    /// <summary>
    /// Represents a reusable region within a view.
    /// </summary>
    Region,

    /// <summary>
    /// Represents a modal dialog or popup window.
    /// </summary>
    Dialog,
}
