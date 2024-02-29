using System.ComponentModel;
using Prism.Navigation;

#nullable enable
namespace Prism.Dialogs;

/// <summary>
/// Provides an experimental implementation of the <see cref="IDialogService"/> which can be used from a Singleton Context.
/// </summary>
/// <remarks>
/// This is experimental and may produce undesirable outcomes.
/// To use this register this like `container.RegisterSingleton&lt;IDialogService, SingletonDialogService&gt;();`
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class SingletonDialogService : DialogServiceBase
{
    private readonly IWindowManager _windowManager;

    /// <summary>
    /// Initializes a new SingletonDialogService
    /// </summary>
    /// <param name="windowManager">An instance of the <see cref="IWindowManager"/>.</param>
    public SingletonDialogService(IWindowManager windowManager)
    {
        ArgumentNullException.ThrowIfNull(windowManager);
        _windowManager = windowManager;
    }

    /// <inheritdoc/>
    protected override Page? GetCurrentPage()
    {
        if (_windowManager.Current is null)
            throw new NotSupportedException("There is currently no application window.");
        else if (_windowManager.Current is not PrismWindow prismWindow)
            throw new NotSupportedException($"The current window '{_windowManager.Current.GetType().FullName}' is not a PrismWindow.");
        else
            return prismWindow.CurrentPage;
    }
}
