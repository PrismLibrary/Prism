using Prism.Commands;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services.Xaml;

namespace Prism.Services;

/// <summary>
/// Provides the ability to display dialogs from ViewModels.
/// </summary>
public sealed class DialogService : IDialogService
{
    private readonly IContainerProvider _container;
    private readonly IPageAccessor _pageAccessor;

    public DialogService(IContainerProvider container, IPageAccessor pageAccessor)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _pageAccessor = pageAccessor ?? throw new ArgumentNullException(nameof(pageAccessor));
    }

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

            async Task DialogAware_RequestClose(IDialogParameters outParameters)
            {
                try
                {
                    var result = await CloseDialogAsync(outParameters ?? new DialogParameters(), currentPage, dialogModal);
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
                        Parameters = parameters
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
            Exception = exception
        };
        await callback.Invoke(result);
    }

    private static async Task<IDialogResult> CloseDialogAsync(IDialogParameters parameters, Page currentPage, IDialogContainer dialogModal)
    {
        try
        {
            PageNavigationService.NavigationSource = PageNavigationSource.DialogService;

            parameters ??= new DialogParameters();

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

            return new DialogResult
            {
                Parameters = parameters
            };
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
                Parameters = parameters
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
