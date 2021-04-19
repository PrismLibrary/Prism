using System;
using System.Linq;
using Prism.AppModel;
using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs.Xaml;
using Xamarin.Forms;

namespace Prism.Services.Dialogs
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
        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            try
            {
                parameters = UriParsingHelper.GetSegmentParameters(name, parameters);

                var view = CreateViewFor(UriParsingHelper.GetSegmentName(name));

                var dialogAware = InitializeDialog(view, parameters);
                var currentPage = GetCurrentContentPage();

                var dialogModal = new DialogPage();
                dialogAware.RequestClose += DialogAware_RequestClose;

                void DialogAware_RequestClose(IDialogParameters outParameters)
                {
                    try
                    {
                        var result = CloseDialog(outParameters ?? new DialogParameters(), currentPage, dialogModal);
                        dialogModal.RaiseDialogResult(result);
                        if (result.Exception is DialogException de && de.Message == DialogException.CanCloseIsFalse)
                        {
                            return;
                        }

                        dialogAware.RequestClose -= DialogAware_RequestClose;
                        callback?.Invoke(result);
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
                            callback?.Invoke(result);
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
                        callback?.Invoke(result);
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

                InsertPopupViewInCurrentPage(currentPage as ContentPage, dialogModal, view, closeOnBackgroundTapped, DialogAware_RequestClose);

                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = false);
                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = true);
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                callback?.Invoke(new DialogResult { Exception = ex });
            }
        }

        private IDialogResult CloseDialog(IDialogParameters parameters, ContentPage currentPage, DialogPage dialogModal)
        {
            try
            {
                if (parameters is null)
                {
                    parameters = new DialogParameters();
                }

                var view = dialogModal.DialogView;
                var dialogAware = GetDialogController(view);

                if (!dialogAware.CanCloseDialog())
                {
                    throw new DialogException(DialogException.CanCloseIsFalse);
                }

                currentPage.Navigation.PopModalAsync(true);

                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = false);
                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = true);
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

        private void InsertPopupViewInCurrentPage(ContentPage currentPage, DialogPage modalPage, View popupView, bool hideOnBackgroundTapped, Action<IDialogParameters> callback)
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

            var dismissCommand = new Command(() => callback(new DialogParameters()));
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
            currentPage.Navigation.PushModalAsync(modalPage, true);
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
