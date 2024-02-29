using Prism.Dialogs;
using Prism.Navigation.Xaml;

namespace Prism.Navigation;

/// <summary>
/// Provides extensions for the <see cref="IWindowManager"/>
/// </summary>
public static class IWindowManagerExtensions
{
    /// <summary>
    /// Gets the <see cref="INavigationService"/> for the currently displayed <see cref="Page"/>.
    /// </summary>
    /// <param name="windowManager">The <see cref="IWindowManager"/>.</param>
    /// <returns>The <see cref="INavigationService"/> for the current <see cref="Page"/>.</returns>
    public static INavigationService GetCurrentNavigationService(this IWindowManager windowManager)
    {
        var page = windowManager.GetCurrentPage();
        return Xaml.Navigation.GetNavigationService(page);
    }

    /// <summary>
    /// Gets the <see cref="IDialogService"/> for the currently displayed <see cref="Page"/>.
    /// </summary>
    /// <param name="windowManager">The <see cref="IWindowManager"/>.</param>
    /// <returns>The <see cref="IDialogService"/> for the current <see cref="Page"/>.</returns>
    public static IDialogService GetCurrentDialogService(this IWindowManager windowManager)
    {
        var page = windowManager.GetCurrentPage();
        var container = page.GetContainerProvider();
        return container.Resolve<IDialogService>();
    }

    private static Page GetCurrentPage(this IWindowManager windowManager)
    {
        var window = windowManager.Current;
        if (window is null)
            throw new InvalidOperationException("No Window has been set in the Application");
        else if (window is not PrismWindow prismWindow)
            throw new InvalidOperationException($"Prism applications only support the use of PrismWindow, but found '{window.GetType().FullName}'.");
        else if (prismWindow.CurrentPage is null)
            throw new InvalidOperationException("No current page has been set.");
        else
            return prismWindow.CurrentPage;
    }
}
