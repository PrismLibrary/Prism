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
    /// <inheritdoc/>
    public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
    {
        IDialogContainer? dialogModal = null;
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
            IDialogContainer.DialogStack.Add(dialogModal);
            var dialogAware = GetDialogController(view);

            async Task DialogAware_RequestClose(IDialogResult outResult)
            {
                bool didCloseDialog = true;
                try
                {
                    var result = await CloseDialogAsync(outResult ?? new DialogResult(), currentPage, dialogModal);
                    if (result.Exception is DialogException de && de.Message == DialogException.CanCloseIsFalse)
                    {
                        didCloseDialog = false;
                        return;
                    }

                    await callback.Invoke(result);
                    GC.Collect();
                }
                catch (DialogException dex)
                {
                    if (dex.Message == DialogException.CanCloseIsFalse)
                    {
                        didCloseDialog = false;
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
                        await InvokeError(callback, dex, parameters);
                    }
                }
                catch (Exception ex)
                {
                    await InvokeError(callback, ex, parameters);
                }
                finally
                {
                    if (didCloseDialog && dialogModal is not null)
                    {
                        IDialogContainer.DialogStack.Remove(dialogModal);
                    }
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

            PageNavigationService.NavigationSource = PageNavigationSource.DialogService;
            dialogModal.ConfigureLayout(currentPage, view, closeOnBackgroundTapped, dismissCommand, parameters);
            PageNavigationService.NavigationSource = PageNavigationSource.Device;

            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = false);
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = true);
        }
        catch (Exception ex)
        {
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

    private static async Task<IDialogResult> CloseDialogAsync(IDialogResult result, Page currentPage, IDialogContainer dialogModal)
    {
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

            var view = dialogModal.DialogView;
            var dialogAware = GetDialogController(view);

            if (!dialogAware.CanCloseDialog())
            {
                throw new DialogException(DialogException.CanCloseIsFalse);
            }

            PageNavigationService.NavigationSource = PageNavigationSource.DialogService;
            await dialogModal.DoPop(currentPage);
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
        }
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
