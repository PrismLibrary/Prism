namespace Prism.AppModel;

/// <summary>
/// Interface that defines lifecycle events for pages.
/// </summary>
public interface IPageLifecycleAware
{
    /// <summary>
    /// Called when the page is appearing.
    /// </summary>
    void OnAppearing();

    /// <summary>
    /// Called when the page is disappearing.
    /// </summary>
    void OnDisappearing();
}
