using Prism.Mvvm;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace Prism.Ioc
{
    public static class IContainerRegistryExtensions
    {
        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <param name="containerRegistry"><see cref="IContainerRegistry"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterForNavigation<TView>(this IContainerRegistry containerRegistry, string name = null) where TView : Page
        {
            var viewType = typeof(TView);

            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            containerRegistry.RegisterForNavigation(viewType, name);
        }

        /// <summary>
        /// Registers a Page for navigation
        /// </summary>
        /// <param name="containerRegistry"><see cref="IContainerRegistry"/> used to register type for Navigation.</param>
        /// <param name="viewType">The type of Page to register</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterForNavigation(this IContainerRegistry containerRegistry, Type viewType, string name)
        {
            PageNavigationRegistry.Register(name, viewType);
            containerRegistry.Register(typeof(object), viewType, name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the BindingContext for the Page</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        /// <param name="containerRegistry"></param>
        public static void RegisterForNavigation<TView, TViewModel>(this IContainerRegistry containerRegistry, string name = null)
            where TView : Page
            where TViewModel : class
        {
            containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="containerRegistry"><see cref="IContainerRegistry"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="androidView">Android Specific View Type</param>
        /// <param name="iOSView">iOS Specific View Type</param>
        /// <param name="otherView">Other Platform Specific View Type</param>
        /// <param name="windowsView">Windows Specific View Type</param>
        /// <param name="winPhoneView">Windows Phone Specific View Type</param>
        [Obsolete("This signature of the RegisterForNavigationOnPlatform method is obsolete due to Device.OnPlatform being deprecated. Use the new IPlatform[] overload instead.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void RegisterForNavigationOnPlatform<TView, TViewModel>(this IContainerRegistry containerRegistry, string name = null, Type androidView = null, Type iOSView = null, Type otherView = null, Type windowsView = null, Type winPhoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            if (Device.OS == TargetPlatform.Android && androidView != null)
            {
                containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(androidView, name);
            }
            else if (Device.OS == TargetPlatform.iOS && iOSView != null)
            {
                containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(iOSView, name);
            }
            else if (Device.OS == TargetPlatform.Other && otherView != null)
            {
                containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(otherView, name);
            }
            else if (Device.OS == TargetPlatform.Windows && windowsView != null)
            {
                containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(windowsView, name);
            }
            else if (Device.OS == TargetPlatform.WinPhone && winPhoneView != null)
            {
                containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(winPhoneView, name);
            }
            else
            {
                containerRegistry.RegisterForNavigation<TView, TViewModel>(name);
            }
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="containerRegistry"><see cref="IContainerRegistry"/> used to register type for Navigation.</param>
        /// <param name="platforms"></param>
        public static void RegisterForNavigationOnPlatform<TView, TViewModel>(this IContainerRegistry containerRegistry, params IPlatform[] platforms)
            where TView : Page
            where TViewModel : class
        {
            var name = typeof(TView).Name;
            RegisterForNavigationOnPlatform<TView, TViewModel>(containerRegistry, name, platforms);
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="containerRegistry"><see cref="IContainerRegistry"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page. If left empty or null will default to the View name.</param>
        /// <param name="platforms"></param>
        public static void RegisterForNavigationOnPlatform<TView, TViewModel>(this IContainerRegistry containerRegistry, string name, params IPlatform[] platforms)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            foreach (var platform in platforms)
            {
                if (Device.RuntimePlatform == platform.RuntimePlatform.ToString())
                    containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(platform.ViewType, name);
            }

            containerRegistry.RegisterForNavigation<TView, TViewModel>(name);
        }

        /// <summary>
        /// Registers a Page for navigation based on the Device Idiom using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be used across multiple Idioms if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">The shared ViewModel</typeparam>
        /// <param name="containerRegistry"><see cref="IContainerRegistry"/> used to register type for Navigation.</param>
        /// <param name="name">The common name used for Navigation. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="desktopView">Desktop Specific View Type</param>
        /// <param name="tabletView">Tablet Specific View Type</param>
        /// <param name="phoneView">Phone Specific View Type</param>
        public static void RegisterForNavigationOnIdiom<TView, TViewModel>(this IContainerRegistry containerRegistry, string name = null, Type desktopView = null, Type tabletView = null, Type phoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            if (Device.Idiom == TargetIdiom.Desktop && desktopView != null)
            {
                containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(desktopView, name);
            }
            else if (Device.Idiom == TargetIdiom.Phone && phoneView != null)
            {
                containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(phoneView, name);
            }
            else if (Device.Idiom == TargetIdiom.Tablet && tabletView != null)
            {
                containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(tabletView, name);
            }
            else
            {
                containerRegistry.RegisterForNavigation<TView, TViewModel>(name);
            }
        }

        private static void RegisterForNavigationWithViewModel<TViewModel>(this IContainerRegistry containerRegistry, Type viewType, string name)
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            containerRegistry.RegisterForNavigation(viewType, name);
        }
    }
}
