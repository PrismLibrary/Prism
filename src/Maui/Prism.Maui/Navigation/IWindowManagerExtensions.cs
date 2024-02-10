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
        var window = windowManager.Windows.OfType<PrismWindow>().First(x => x.IsActive);

        if (window.CurrentPage is null)
            throw new InvalidOperationException("No current page has been set.");

        return Xaml.Navigation.GetNavigationService(window.CurrentPage);
    }
}
