namespace Prism.Navigation;

#nullable enable
/// <summary>
/// Defines an interface for managing windows within an application.
/// </summary>
public interface IWindowManager
{
    /// <summary>
    /// Gets a read-only collection of all currently open windows.
    /// </summary>
    IReadOnlyList<Window> Windows { get; }

    /// <summary>
    /// Gets the currently active window, if any.
    /// </summary>
    Window? Current { get; }

    /// <summary>
    /// Opens a new window.
    /// </summary>
    /// <param name="window">The Window object to be opened.</param>
    /// <exception cref="ArgumentNullException">Thrown if the window parameter is null.</exception>
    void OpenWindow(Window window);

    /// <summary>
    /// Closes a specified window.
    /// </summary>
    /// <param name="window">The Window object to be closed.</param>
    /// <exception cref="ArgumentNullException">Thrown if the window parameter is null.</exception>
    void CloseWindow(Window window);
}

