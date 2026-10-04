using Prism.Commands;
using Prism.Common;
using Prism.Dialogs.Xaml;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Xaml;

#nullable enable
namespace Prism.Dialogs;

/// <summary>
/// Provides the ability to display dialogs from ViewModels.
/// </summary>
public abstract class DialogServiceBase : IDialogService
{
    private static readonly Dictionary<Page, Window> _closingDialogs = new(ReferenceEqualityComparer.Instance);

    /// <inheritdoc/>
    public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
    {
        IDialogContainer? dialogModal = null;
        var ready = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var closeState = 0; // open, closing, closed
        try
        {
            parameters = UriParsingHelper.GetSegmentParameters(name, parameters ?? new DialogParameters());

            var currentPage = GetCurrentPage();
            ArgumentNullException.ThrowIfNull(currentPage);
            var container = currentPage.GetContainerProvider();
            // This needs to be resolved when called as a Module could load any time
            // and register new dialogs
            var registry = container.Resolve<IDialogViewRegistry>();
            var view = registry.CreateView(container, UriParsingHelper.GetSegmentName(name)) as View 
                ?? throw new ViewCreationException(name, ViewType.Dialog);

            dialogModal = container.Resolve<IDialogContainer>();
            var dialogAware = GetDialogController(view);

            async Task DialogAware_RequestClose(IDialogResult outResult)
            {
                // Reject duplicates before CloseDialogAsync changes NavigationSource.
                if (Interlocked.CompareExchange(ref closeState, 1, 0) != 0)
                    return;
                var ownsAttempt = true;

                void ReleaseAttempt()
                {
                    if (!ownsAttempt)
                        return;
                    ownsAttempt = false;
                    Interlocked.CompareExchange(ref closeState, 0, 1);
                }

                try
                {
                    // OnDialogOpened/Loaded can request close before hosting finishes.
                    // Activate first so a late show continuation cannot reactivate a closed view.
                    if (!await ready.Task)
                        return;

                    var result = await CloseDialogAsync(outResult ?? new DialogResult(), currentPage, dialogModal,
                        () => Interlocked.Exchange(ref closeState, 2));
                    if (result.Exception is DialogException de && de.Message == DialogException.CanCloseIsFalse)
                    {
                        return;
                    }

                    // Native work and NavigationSource cleanup have completed. An error
                    // callback may correct the problem and immediately request another close.
                    ReleaseAttempt();
                    // DialogStack is updated when the container removes the overlay (e.g. DialogContainerPage.DoPop).
                    await callback.Invoke(result);
                    GC.Collect();
                }
                catch (DialogException dex)
                {
                    if (dex.Message == DialogException.CanCloseIsFalse)
                    {
                        return;
                    }

                    var result = new DialogResult
                    {
                        Exception = dex,
                        Parameters = parameters,
                        Result = ButtonResult.None
                    };

                    if (dex.Message != DialogException.CanCloseIsFalse)
                    {
                        ReleaseAttempt();
                        await InvokeError(callback, dex, parameters);
                    }
                }
                catch (Exception ex)
                {
                    ReleaseAttempt();
                    await InvokeError(callback, ex, parameters);
                }
                finally
                {
                    // Vetoed or failed removals may be retried. A successful removal
                    // stays terminal, including when lifecycle or result callbacks throw.
                    ReleaseAttempt();
                }
            }

            DialogUtilities.InitializeListener(dialogAware, DialogAware_RequestClose);

            dialogAware.OnDialogOpened(parameters);

            if (!parameters.TryGetValue<bool>(KnownDialogParameters.CloseOnBackgroundTapped, out var closeOnBackgroundTapped))
            {
                var dialogLayoutCloseOnBackgroundTapped = DialogLayout.GetCloseOnBackgroundTapped(view);
                if (dialogLayoutCloseOnBackgroundTapped.HasValue)
                {
                    closeOnBackgroundTapped = dialogLayoutCloseOnBackgroundTapped.Value;
                }
            }

            var dismissCommand = new DelegateCommand(() => dialogAware.RequestClose.Invoke(), dialogAware.CanCloseDialog);

            _ = InitializeDialogAsync();

            async Task InitializeDialogAsync()
            {
                Exception? error = null;
                var hosted = false;
                try
                {
                    PageNavigationService.NavigationSource = PageNavigationSource.DialogService;
                    await dialogModal.ConfigureLayout(currentPage, view, closeOnBackgroundTapped, dismissCommand, parameters);
                    hosted = true;
                    MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = false);
                    MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = true);
                }
                catch (Exception ex)
                {
                    error = ex;
                    if (!hosted)
                        Interlocked.Exchange(ref closeState, 2);
                }
                finally
                {
                    PageNavigationService.NavigationSource = PageNavigationSource.Device;
                }

                ready.TrySetResult(hosted);
                if (error is not null)
                    await InvokeError(callback, error, parameters);
            }
        }
        catch (Exception ex)
        {
            Interlocked.Exchange(ref closeState, 2);
            ready.TrySetResult(false);
            callback.Invoke(ex);
        }
    }

    /// <summary>
    /// Gets the currently displayed page for the Application
    /// </summary>
    /// <returns></returns>
    protected abstract Page? GetCurrentPage();

    private static async Task InvokeError(DialogCallback callback, Exception exception, IDialogParameters parameters)
    {
        var result = new DialogResult 
        {
            Parameters = parameters,
            Exception = exception,
            Result = ButtonResult.None
        };
        await callback.Invoke(result);
    }

    private static async Task<IDialogResult> CloseDialogAsync(IDialogResult result, Page currentPage, IDialogContainer dialogModal, Action onClosed)
    {
        Page? closingPage = null;
        var ownsModal = false;
        try
        {
            PageNavigationService.NavigationSource = PageNavigationSource.DialogService;

            result ??= new DialogResult();
            if (result.Parameters is null)
            {
                result = new DialogResult
                {
                    Exception = result.Exception,
                    Parameters = new DialogParameters(),
                    Result = result.Result
                };
            }

            var view = dialogModal.DialogView ?? throw new DialogException(DialogException.HostPageIsNotDialogHost);
            var dialogAware = GetDialogController(view);

            if (!dialogAware.CanCloseDialog())
            {
                throw new DialogException(DialogException.CanCloseIsFalse);
            }

            // Bind permission to this dialog's verified visual host and window, never
            // the current top modal (which may belong to a different popup).
            closingPage = GetDialogPage(dialogModal);
            if (closingPage?.GetParentWindow() is Window window && window.Navigation.ModalStack.Any(modal => ReferenceEquals(modal, closingPage)))
            {
                lock (_closingDialogs)
                    ownsModal = _closingDialogs.TryAdd(closingPage, window);
                if (!ownsModal)
                    throw new DialogException(DialogException.CanCloseIsFalse);
            }

            PageNavigationService.NavigationSource = PageNavigationSource.DialogService;
            await dialogModal.DoPop(currentPage);
            onClosed();
            PageNavigationService.NavigationSource = PageNavigationSource.Device;

            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = false);
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = true);
            dialogAware.OnDialogClosed();

            return result;
        }
        catch (DialogException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new DialogResult
            {
                Exception = ex,
                Parameters = result.Parameters,
                Result = result.Result
            };
        }
        finally
        {
            PageNavigationService.NavigationSource = PageNavigationSource.Device;
            if (ownsModal)
            {
                lock (_closingDialogs)
                    _closingDialogs.Remove(closingPage!);
            }
        }
    }

    internal static bool IsDialogClosing(Page modal, Window window)
    {
        lock (_closingDialogs)
            return _closingDialogs.TryGetValue(modal, out var owner) && ReferenceEquals(owner, window);
    }

    internal static Page? GetDialogPage(IDialogContainer dialog)
    {
        Page? page = null;
        for (Element? element = dialog.DialogView; element is not null && element is not Window; element = element.Parent)
        {
            if (element is Page ancestor)
                page = ancestor;
        }
        return page;
    }

    private static IDialogAware GetDialogController(View view)
    {
        if (view is IDialogAware viewAsDialogAware)
        {
            return viewAsDialogAware;
        }
        else if (view.BindingContext is null)
        {
            throw new DialogException(DialogException.NoViewModel);
        }
        else if (view.BindingContext is IDialogAware dialogAware)
        {
            return dialogAware;
        }

        throw new DialogException(DialogException.ImplementIDialogAware);
    }
}
