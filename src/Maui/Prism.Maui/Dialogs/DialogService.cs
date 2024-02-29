using Prism.Common;

#nullable enable
namespace Prism.Dialogs;

/// <summary>
/// Provides a default scoped implementation of the <see cref="IDialogService"/>.
/// </summary>
public sealed class DialogService : DialogServiceBase
{
    private readonly IPageAccessor _pageAccessor;

    /// <summary>
    /// Creates a new instance of the <see cref="DialogService"/> for Maui Applications
    /// </summary>
    /// <param name="pageAccessor">The <see cref="IPageAccessor"/> used to determine where in the Navigation Stack we need to process the Dialog.</param>
    /// <exception cref="ArgumentNullException">Throws when any constructor arguments are null.</exception>
    public DialogService(IPageAccessor pageAccessor) 
    {
        ArgumentNullException.ThrowIfNull(pageAccessor);
        _pageAccessor = pageAccessor;
    }

    /// <inheritdoc/>
    protected override Page? GetCurrentPage() => _pageAccessor.Page;
}
