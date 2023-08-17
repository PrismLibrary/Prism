using System;
using System.Linq;
using System.Threading.Tasks;
using Prism.Common;
using Prism.Dialogs.Xaml;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Dialogs
{
    /// <summary>
    /// Provides the ability to display dialogs from ViewModels.
    /// </summary>
    public sealed class DialogService : IDialogService
    {
        /// <summary>
        /// Gets the key for specifying or retrieving popup overlay style from Application Resources.
        /// </summary>
        public const string PopupOverlayStyle = "PrismDialogMaskStyle";

        private IContainerProvider _containerExtension { get; }
        private IApplicationProvider _applicationProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="applicationProvider">An object that provides Application components.</param>
        /// <param name="containerProvider">An object that can resolve services.</param>
        public DialogService(IApplicationProvider applicationProvider, IContainerProvider containerProvider)
        {
            _applicationProvider = applicationProvider;
            _containerExtension = containerProvider;
        }

        /// <inheritdoc/>
        public async void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        {
            try
            {
                parameters = UriParsingHelper.GetSegmentParameters(name, parameters);

                var view = CreateViewFor(UriParsingHelper.GetSegmentName(name));

                var dialogAware = InitializeDialog(view, parameters);
                var currentPage = GetCurrentContentPage();

                var dialogModal = new DialogPage();

                DialogUtilities.InitializeListener(dialogAware, DialogAware_RequestClose);

                async Task DialogAware_RequestClose(IDialogResult outResult)
                {
                    try
                    {
                        var result = await CloseDialogAsync(outResult ?? new DialogResult(), currentPage, dialogModal);
                        dialogModal.RaiseDialogResult(result);
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
                        dialogModal.RaiseDialogResult(result);

                        if (dex.Message != DialogException.CanCloseIsFalse)
                        {
                            await callback.Invoke(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        var result = new DialogResult
                        {
                            Exception = ex,
                            Parameters = parameters
                        };
                        dialogModal.RaiseDialogResult(result);
                        await callback.Invoke(result);
                    }
                }

                if (!parameters.TryGetValue<bool>(KnownDialogParameters.CloseOnBackgroundTapped, out var closeOnBackgroundTapped))
                {
                    var dialogLayoutCloseOnBackgroundTapped = DialogLayout.GetCloseOnBackgroundTapped(view);
                    if (dialogLayoutCloseOnBackgroundTapped.HasValue)
                    {
                        closeOnBackgroundTapped = dialogLayoutCloseOnBackgroundTapped.Value;
                    }
                }

                InsertPopupViewInCurrentPage(currentPage as ContentPage, dialogModal, view, closeOnBackgroundTapped, dialogAware.RequestClose);

                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = false);
                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = true);
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                await callback.Invoke(new DialogResult { Exception = ex });
            }
        }

        private async System.Threading.Tasks.Task<IDialogResult> CloseDialogAsync(IDialogResult result, ContentPage currentPage, DialogPage dialogModal)
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

                await currentPage.Navigation.PopModalAsync(true);

                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = false);
                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = true);
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

        private View CreateViewFor(string name)
        {
            var view = (View)_containerExtension.Resolve<object>(name);
            PageUtilities.SetAutowireViewModel(view);

            return view;
        }

        private IDialogAware GetDialogController(View view)
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

        private IDialogAware InitializeDialog(View view, IDialogParameters parameters)
        {
            var dialog = GetDialogController(view);

            dialog.OnDialogOpened(parameters);

            return dialog;
        }

        private ContentPage GetCurrentContentPage()
        {
            var cp = GetCurrentPage();
            var mp = TryGetModalPage(cp);
            return mp ?? cp;
        }

        private ContentPage TryGetModalPage(ContentPage cp)
        {
            var mp = cp.Navigation.ModalStack.LastOrDefault();
            if (mp != null)
            {
                return GetCurrentPage(mp);
            }

            return null;
        }

        private ContentPage GetCurrentPage(Page page = null)
        {
            switch (page)
            {
                case ContentPage cp:
                    return cp;
                case TabbedPage tp:
                    return GetCurrentPage(tp.CurrentPage);
                case NavigationPage np:
                    return GetCurrentPage(np.CurrentPage);
                case CarouselPage carouselPage:
                    return GetCurrentPage(carouselPage.CurrentPage);
                case FlyoutPage flyout:
                    flyout.IsPresented = false;
                    return GetCurrentPage(flyout.Detail);
                case Shell shell:
                    return GetCurrentPage((shell.CurrentItem.CurrentItem as IShellSectionController).PresentedPage);
                default:
                    // If we get some random Page Type
                    if (page != null)
                    {
                        Xamarin.Forms.Internals.Log.Warning("Warning", $"An Unknown Page type {page.GetType()} was found walk walking the Navigation Stack. This is not supported by the DialogService");
                        return null;
                    }

                    var mainPage = _applicationProvider.MainPage;
                    if (mainPage is null)
                    {
                        return null;
                    }

                    return GetCurrentPage(mainPage);
            }
        }

        private async void InsertPopupViewInCurrentPage(ContentPage currentPage, DialogPage modalPage, View popupView, bool hideOnBackgroundTapped, DialogCloseListener closeEvent)
        {
            View mask = DialogLayout.GetMask(popupView);

            if (mask is null)
            {
                Style overlayStyle = GetOverlayStyle(popupView);

                mask = new BoxView
                {
                    Style = overlayStyle
                };
            }

            mask.SetBinding(VisualElement.WidthRequestProperty, new Binding { Path = "Width", Source = modalPage });
            mask.SetBinding(VisualElement.HeightRequestProperty, new Binding { Path = "Height", Source = modalPage });

            var dismissCommand = new Command(closeEvent.Invoke);
            if (hideOnBackgroundTapped)
            {
                mask.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = dismissCommand
                });
            }
            modalPage.Dismiss = dismissCommand;

            var overlay = new AbsoluteLayout();
            var popupContainer = new DialogContainer
            {
                IsPopupContent = true,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = popupView,
            };

            var relativeWidth = DialogLayout.GetRelativeWidthRequest(popupView);
            if (relativeWidth != null)
            {
                popupContainer.SetBinding(DialogContainer.WidthRequestProperty,
                    new Binding("Width",
                                BindingMode.OneWay,
                                new RelativeContentSizeConverter { RelativeSize = relativeWidth.Value },
                                source: modalPage));
            }

            var relativeHeight = DialogLayout.GetRelativeHeightRequest(popupView);
            if (relativeHeight != null)
            {
                popupContainer.SetBinding(DialogContainer.HeightRequestProperty,
                    new Binding("Height",
                                BindingMode.OneWay,
                                new RelativeContentSizeConverter { RelativeSize = relativeHeight.Value },
                                source: modalPage));
            }

            AbsoluteLayout.SetLayoutFlags(popupContainer, AbsoluteLayoutFlags.PositionProportional);
            var popupBounds = DialogLayout.GetLayoutBounds(popupView);
            AbsoluteLayout.SetLayoutBounds(popupContainer, popupBounds);

            if (DialogLayout.GetUseMask(popupContainer.Content) ?? true)
            {
                overlay.Children.Add(mask);
            }

            overlay.Children.Add(popupContainer);

            modalPage.Content = overlay;
            modalPage.DialogView = popupView;
            await currentPage.Navigation.PushModalAsync(modalPage, true);
        }

        private static Style GetOverlayStyle(View popupView)
        {
            var style = DialogLayout.GetMaskStyle(popupView);
            if (style != null)
            {
                return style;
            }

            if (Application.Current.Resources.ContainsKey(PopupOverlayStyle))
            {
                style = (Style)Application.Current.Resources[PopupOverlayStyle];
                if (style.TargetType == typeof(BoxView))
                {
                    return style;
                }
            }

            var overlayStyle = new Style(typeof(BoxView));
            overlayStyle.Setters.Add(new Setter { Property = VisualElement.OpacityProperty, Value = 0.75 });
            overlayStyle.Setters.Add(new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Black });

            Application.Current.Resources[PopupOverlayStyle] = overlayStyle;
            return overlayStyle;
        }
    }
}
