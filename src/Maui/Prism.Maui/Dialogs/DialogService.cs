using Prism.Common;
using Prism.Navigation;

#nullable enable
namespace Prism.Dialogs;

/// <summary>
/// Provides a default scoped implementation of the <see cref="IDialogService"/>.
/// </summary>
public sealed class DialogService : DialogServiceBase
{
    private readonly IPageAccessor _pageAccessor;
    private readonly IWindowManager _windowManager;

    /// <summary>
    /// Creates a new instance of the <see cref="DialogService"/> for Maui Applications
    /// </summary>
    /// <param name="pageAccessor">The <see cref="IPageAccessor"/> used to determine where in the Navigation Stack we need to process the Dialog.</param>
    /// <param name="windowManager">The <see cref="IWindowManager"/> used to resolve the current page when the scoped page is no longer attached.</param>
    /// <exception cref="ArgumentNullException">Throws when any constructor arguments are null.</exception>
    public DialogService(IPageAccessor pageAccessor, IWindowManager windowManager)
    {
        ArgumentNullException.ThrowIfNull(pageAccessor);
        ArgumentNullException.ThrowIfNull(windowManager);
        _pageAccessor = pageAccessor;
        _windowManager = windowManager;
    }

    /// <inheritdoc/>
    protected override Page? GetCurrentPage()
    {
        var page = _pageAccessor.Page;

        // If the scoped page is still connected to a window, use it
        if (page?.GetParentWindow() is not null)
            return page;

        // Fallback: the scoped page was detached (e.g. after absolute navigation).
        // Resolve the current page from the active window instead.
        if (_windowManager.Current is PrismWindow prismWindow)
            return prismWindow.CurrentPage;

        return page;
    }
}
