using Prism.Commands;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Dialogs.Xaml;

namespace Prism.Dialogs;

/// <summary>
/// Provides the ability to display dialogs from ViewModels.
/// </summary>
public sealed class DialogService : IDialogService
{
    private readonly IContainerProvider _container;
    private readonly IPageAccessor _pageAccessor;

    /// <summary>
    /// Creates a new instance of the <see cref="DialogService"/> for Maui Applications
    /// </summary>
    /// <param name="container">The <see cref="IContainerProvider"/> that will be used to help resolve the Dialog Views.</param>
    /// <param name="pageAccessor">The <see cref="IPageAccessor"/> used to determine where in the Navigation Stack we need to process the Dialog.</param>
    /// <exception cref="ArgumentNullException">Throws when any constructor arguments are null.</exception>
    public DialogService(IContainerProvider container, IPageAccessor pageAccessor)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _pageAccessor = pageAccessor ?? throw new ArgumentNullException(nameof(pageAccessor));
    }

    /// <inheritdoc/>
    public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
    {
        try
        {
            parameters = UriParsingHelper.GetSegmentParameters(name, parameters ?? new DialogParameters());

            // This needs to be resolved when called as a Module could load any time
            // and register new dialogs
            var registry = _container.Resolve<IDialogViewRegistry>();
            var view = registry.CreateView(_container, UriParsingHelper.GetSegmentName(name)) as View;

            var currentPage = _pageAccessor.Page;
            var dialogModal = _container.Resolve<IDialogContainer>();
            var dialogAware = GetDialogController(view);

            async Task DialogAware_RequestClose(IDialogResult outResult)
            {
                try
                {
                    var result = await CloseDialogAsync(outResult ?? new DialogResult(), currentPage, dialogModal);
                    if (result.Exception is DialogException de && de.Message == DialogException.CanCloseIsFalse)
                    {
                        return;
                    }

                    await callback.Invoke(result);
                    GC.Collect();
                }
                catch (DialogException dex)
                {
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
            }

            dialogAware.RequestClose = new (DialogAware_RequestClose);

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
            dialogModal.ConfigureLayout(_pageAccessor.Page, view, closeOnBackgroundTapped, dismissCommand);
            PageNavigationService.NavigationSource = PageNavigationSource.Device;

            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = false);
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = true);
        }
        catch (Exception ex)
        {
            callback.Invoke(ex);
        }
    }

    private async Task InvokeError(DialogCallback callback, Exception exception, IDialogParameters parameters)
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
