namespace Prism.Windows;

#nullable enable
/// <summary>Creates and locates explicitly targeted, independently scoped native windows.</summary>
/// <remarks>
/// All windows and calls must use the application's original WinUI dispatcher thread.
/// This service deliberately exposes no ambient or active window. Application modules
/// and singleton services are shared; opening another window does not initialize modules again.
/// </remarks>
public interface IWindowManager
{
    /// <summary>Gets a snapshot of the windows whose scopes are still open.</summary>
    IReadOnlyList<IWindowContext> Windows { get; }

    /// <summary>Creates a scope, resolves and attaches a shell, and activates its window.</summary>
    /// <param name="window">An unused window created on the application's WinUI thread.</param>
    /// <param name="createShell">Creates the shell using the supplied window container.</param>
    /// <param name="onInitialized">Runs once after this shell loads and its regions are available.</param>
    /// <returns>The new window's explicit ownership context.</returns>
    /// <remarks>
    /// Resolve shell dependencies through <paramref name="createShell"/>'s provider, not
    /// the application's root container. Startup failures close the window and dispose
    /// its scope. A transient Unloaded event does not end a successful window's lifetime.
    /// </remarks>
    IWindowContext OpenWindow(Window window, Func<IContainerProvider, UIElement> createShell,
        Action<IWindowContext>? onInitialized = null);

    /// <summary>Gets the context of an open, managed window.</summary>
    /// <param name="window">The exact target window.</param>
    /// <returns>The window's context.</returns>
    /// <exception cref="InvalidOperationException">The window is unknown or closed.</exception>
    IWindowContext GetWindow(Window window);
}
