using Prism.AppModel;
using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs.Xaml;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Prism.Services.Dialogs
{
    public sealed class DialogService : IDialogService
    {
        public const string PopupOverlayStyle = "PrismDialogMaskStyle";

        private IContainerProvider _containerExtension { get; }
        private IApplicationProvider _applicationProvider { get; }

        public DialogService(IApplicationProvider applicationProvider, IContainerExtension containerExtension)
        {
            _applicationProvider = applicationProvider;
            _containerExtension = containerExtension;
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            try
            {
                parameters = UriParsingHelper.GetSegmentParameters(name, parameters);

                var view = CreateViewFor(UriParsingHelper.GetSegmentName(name));

                var dialogAware = InitializeDialog(view, parameters);
                var currentPage = GetCurrentContentPage();

                dialogAware.RequestClose += DialogAware_RequestClose;

                void DialogAware_RequestClose(IDialogParameters outParameters)
                {
                    try
                    {
                        var result = CloseDialog(outParameters ?? new DialogParameters(), currentPage);
                        if(result.Exception is DialogException de && de.Message == DialogException.CanCloseIsFalse)
                        {
                            return;
                        }

                        dialogAware.RequestClose -= DialogAware_RequestClose;
                        callback?.Invoke(result);
                        GC.Collect();
                    }
                    catch(DialogException dex)
                    {
                        if(dex.Message != DialogException.CanCloseIsFalse)
                        {
                            callback?.Invoke(new DialogResult
                            {
                                Exception = dex,
                                Parameters = parameters
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        callback?.Invoke(new DialogResult
                        {
                            Exception = ex,
                            Parameters = parameters
                        });
                    }
                }

                if(!parameters.TryGetValue<bool>(KnownDialogParameters.CloseOnBackgroundTapped, out var closeOnBackgroundTapped))
                {
                    var dialogLayoutCloseOnBackgroundTapped = DialogLayout.GetCloseOnBackgroundTapped(view);
                    if(dialogLayoutCloseOnBackgroundTapped.HasValue)
                    {
                        closeOnBackgroundTapped = dialogLayoutCloseOnBackgroundTapped.Value;
                    }
                }

                InsertPopupViewInCurrentPage(currentPage as ContentPage, view, closeOnBackgroundTapped, DialogAware_RequestClose);

                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = false);
                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = true);
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                callback?.Invoke(new DialogResult { Exception = ex });
            }
        }

        private IDialogResult CloseDialog(IDialogParameters parameters, ContentPage currentPage)
        {
            try
            {
                if(parameters is null)
                {
                    parameters = new DialogParameters();
                }

                if (!(currentPage is ContentPage contentPage))
                {
                    throw new DialogException(DialogException.RequiresContentPage);
                }

                if (!(bool)contentPage.Content.GetValue(IsPopupHostProperty))
                {
                    throw new DialogException(DialogException.HostPageIsNotDialogHost);
                }

                var hostView = (AbsoluteLayout)contentPage.Content;

                var popupContainer = (DialogContainer)hostView.Children.First(x => x is DialogContainer dc && dc.IsPopupContent);
                var view = popupContainer.Content;
                var dialogAware = GetDialogController(view);

                if(!dialogAware.CanCloseDialog())
                {
                    throw new DialogException(DialogException.CanCloseIsFalse);
                }

                var pageContainer = (DialogContainer)hostView.Children.First(x => x is DialogContainer dc && dc.IsPageContent);
                contentPage.Content = pageContainer.Content;
                contentPage.Padding = pageContainer.Padding;
                contentPage.SetValue(IsPopupHostProperty, false);

                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(view, aa => aa.IsActive = false);
                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(currentPage, aa => aa.IsActive = true);
                dialogAware.OnDialogClosed();

                return new DialogResult
                {
                    Parameters = parameters
                };
            }
            catch(DialogException)
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

            if (ViewModelLocator.GetAutowireViewModel(view) is null)
            {
                ViewModelLocator.SetAutowireViewModel(view, true);
            }

            return view;
        }

        private IDialogAware GetDialogController(View view)
        {
            if(view is IDialogAware viewAsDialogAware)
            {
                return viewAsDialogAware;
            }
            else if(view.BindingContext is null)
            {
                throw new DialogException(DialogException.NoViewModel);
            }
            else if(view.BindingContext is IDialogAware dialogAware)
            {
                return dialogAware;
            }

            throw new DialogException(DialogException.ImplementIDialogAware);
        }

        private IDialogAware InitializeDialog(View view, IDialogParameters parameters)
        {
            var dialog = GetDialogController(view);

            dialog.OnDialogOpened(parameters);

            if(dialog is IAbracadabra)
            {
                PageUtilities.Abracadabra(dialog, parameters);
            }

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
            switch(page)
            {
                case ContentPage cp:
                    return cp;
                case TabbedPage tp:
                    return GetCurrentPage(tp.CurrentPage);
                case NavigationPage np:
                    return GetCurrentPage(np.CurrentPage);
                case CarouselPage carouselPage:
                    return GetCurrentPage(carouselPage.CurrentPage);
                case MasterDetailPage mdp:
                    mdp.IsPresented = false;
                    return GetCurrentPage(mdp.Detail);
                case Shell shell:
                    return GetCurrentPage((shell.CurrentItem.CurrentItem as IShellSectionController).PresentedPage);
                default:
                    // If we get some random Page Type
                    if(page != null)
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

        private void InsertPopupViewInCurrentPage(ContentPage currentPage, View popupView, bool hideOnBackgroundTapped, Action<IDialogParameters> callback)
        {
            View mask = DialogLayout.GetMask(popupView);

            if(mask is null)
            {
                Style overlayStyle = GetOverlayStyle(popupView);

                mask = new BoxView
                {
                    Style = overlayStyle
                };
            }

            mask.SetBinding(VisualElement.WidthRequestProperty, new Binding { Path = "Width", Source = currentPage });
            mask.SetBinding(VisualElement.HeightRequestProperty, new Binding { Path = "Height", Source = currentPage });

            if (hideOnBackgroundTapped)
            {
                var dismissCommand = new Command(() => callback(new DialogParameters()));
                mask.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = dismissCommand
                });
            }

            var overlay = new AbsoluteLayout();
            overlay.SetValue(IsPopupHostProperty, true);
            var existingContent = currentPage.Content;
            existingContent.BindingContext = currentPage.BindingContext;
            var content = new DialogContainer
            {
                Padding = currentPage.Padding,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsPageContent = true
            };
            currentPage.Padding = new Thickness(0);

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
                                source: currentPage));
            }

            var relativeHeight = DialogLayout.GetRelativeHeightRequest(popupView);
            if (relativeHeight != null)
            {
                popupContainer.SetBinding(DialogContainer.HeightRequestProperty,
                    new Binding("Height",
                                BindingMode.OneWay,
                                new RelativeContentSizeConverter { RelativeSize = relativeHeight.Value },
                                source: currentPage));
            }

            AbsoluteLayout.SetLayoutFlags(content, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(content, new Rectangle(0f, 0f, currentPage.Width, currentPage.Height));
            AbsoluteLayout.SetLayoutFlags(popupContainer, AbsoluteLayoutFlags.PositionProportional);
            var popupBounds = DialogLayout.GetLayoutBounds(popupView);
            AbsoluteLayout.SetLayoutBounds(popupContainer, popupBounds);
            overlay.Children.Add(content);

            if (DialogLayout.GetUseMask(popupContainer.Content) ?? true)
            {
                overlay.Children.Add(mask);
            }

            overlay.Children.Add(popupContainer);
            currentPage.Content = overlay;

            // The original content needs to be reparented after the Page Content has been reset.
            content.Content = existingContent;
        }

        private static Style GetOverlayStyle(View popupView)
        {
            var style = DialogLayout.GetMaskStyle(popupView);
            if(style != null)
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

        internal static readonly BindableProperty IsPopupHostProperty =
            BindableProperty.CreateAttached("IsPopupHost", typeof(bool), typeof(DialogService), false);
    }
}
