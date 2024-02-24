namespace Prism.AppModel;


/// <summary>
/// Defines methods for responding to application lifecycle events.
/// </summary>
public interface IApplicationLifecycleAware
{
    /// <summary>
    /// Called when the application resumes from a suspended state.
    /// </summary>
    void OnResume();

    /// <summary>
    /// Called when the application enters a sleep or suspended state.
    /// </summary>
    void OnSleep();
}
