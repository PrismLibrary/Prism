namespace Prism.Common;

/// <summary>
/// Interface to signify that a class must have knowledge of a specific <see cref="Microsoft.Maui.Controls.Page"/> instance in order to function properly.
/// </summary>
public interface IPageAccessor
{
    /// <summary>
    /// The <see cref="Microsoft.Maui.Controls.Page"/> instance.
    /// </summary>
    Page Page { get; set; }
}
