using System.ComponentModel;
using Prism.AppModel;
using Prism.Common;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Navigation.Xaml;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism.Navigation
{
    /// <summary>
    /// Represents a window used for Prism navigation in a Maui application.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class PrismWindow : Window
    {
        /// <summary>
        /// The default name for the Prism window.
        /// </summary>
        public const string DefaultWindowName = "__PrismRootWindow";

        /// <summary>
        /// Initializes a new instance of the <see cref="PrismWindow"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the window.</param>
        public PrismWindow(string name = DefaultWindowName)
        {
            Name = name;
            ModalPopping += PrismWindow_ModalPopping;
        }

        /// <summary>
        /// Gets the name of the window.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the window is active.
        /// </summary>
        public bool IsActive { get; internal set; }

        /// <summary>
        /// Gets the current page displayed in the window.
        /// </summary>
        internal Page CurrentPage => Page is null ? null : MvvmHelpers.GetCurrentPage(Page);

        /// <summary>
        /// Gets a value indicating whether the current page is the root page.
        /// </summary>
        internal bool IsRootPage => GetRootPage(Page) == CurrentPage;

        private Page GetRootPage(Page page) =>
            page switch
            {
                TabbedPage tabbed => GetRootPage(tabbed.CurrentPage),
                NavigationPage nav => GetRootPage(nav.RootPage),
                FlyoutPage flyout => GetRootPage(flyout.Detail),
                _ => page
            };

        /// <summary>
        /// Handles the system back button press.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnSystemBack()
        {
            var currentPage = CurrentPage;
            if (currentPage?.Parent is NavigationPage navPage)
            {
                // The NavigationPage has already taken care of the GoBack
                return;
            }

            var container = currentPage.GetContainerProvider();

            if (IsRoot(currentPage))
            {
                var app = container.Resolve<IApplication>() as Application;
                app.Quit();
                return;
            }
            else if (currentPage is IDialogContainer dialogContainer)
            {
                if (dialogContainer.Dismiss.CanExecute(null))
                    dialogContainer.Dismiss.Execute(null);
            }
            else
            {
                var navigation = container.Resolve<INavigationService>();
                navigation.GoBackAsync();
            }
        }

        private bool IsRoot(Page page)
        {
            if (page == Page) return true;

            return page.Parent switch
            {
                FlyoutPage flyout => IsRoot(flyout),
                TabbedPage tabbed => IsRoot(tabbed),
                NavigationPage navigation => IsRoot(navigation),
                _ => false
            };
        }

        private async void PrismWindow_ModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if (PageNavigationService.NavigationSource == PageNavigationSource.Device)
            {
                e.Cancel = true;
                var dialogModal = IDialogContainer.DialogStack.LastOrDefault();
                if (dialogModal is not null)
                {
                    if (dialogModal.Dismiss.CanExecute(null))
                        dialogModal.Dismiss.Execute(null);
                }
                else
                {
                    var navService = Xaml.Navigation.GetNavigationService(e.Modal);
                    await navService.GoBackAsync();
                }
            }
        }

        /// <summary>
        /// Called when the window is activated.
        /// </summary>
        protected override void OnActivated()
        {
            IsActive = true;
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(CurrentPage, x => x.IsActive = true);
        }

        /// <summary>
        /// Called when the window is deactivated.
        /// </summary>
        protected override void OnDeactivated()
        {
            IsActive = false;
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(CurrentPage, x => x.IsActive = true);
        }

        /// <summary>
        /// Called when the window is resumed.
        /// </summary>
        protected override void OnResumed()
        {
            MvvmHelpers.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(CurrentPage, x => x.OnResume());
        }

        /// <summary>
        /// Called when the window is stopped.
        /// </summary>
        protected override void OnStopped()
        {
            MvvmHelpers.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(CurrentPage, x => x.OnSleep());
        }
    }
}
