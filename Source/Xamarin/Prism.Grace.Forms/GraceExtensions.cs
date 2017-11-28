using Grace.DependencyInjection;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace Prism.Grace.Forms
{
    public static class GraceExtensions
    {
        /// <summary>
		/// Registers a Page for navigation.
		/// </summary>
		/// <typeparam name="TView">The Type of Page to register</typeparam>
		/// <param name="container"><see cref="DependencyInjectionContainer"/> used to register type for Navigation.</param>
		/// <param name="name">The unique name to register with the Page</param>
		public static DependencyInjectionContainer ConfigureTypeForNavigation<TView>(this DependencyInjectionContainer container, string name = null) where TView : Page
        {
            var viewType = typeof(TView);

            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            return container.ConfigureTypeForNavigation(viewType, name);
        }

        /// <summary>
        /// Registers a Page for navigation
        /// </summary>
        /// <param name="container"><see cref="DependencyInjectionContainer"/> used to register type for Navigation.</param>
        /// <param name="viewType">The type of Page to register</param>
        /// <param name="name">The unique name to register with the Page</param>
        /// <returns><see cref="DependencyInjectionContainer"/></returns>
        public static DependencyInjectionContainer ConfigureTypeForNavigation(this DependencyInjectionContainer container, Type viewType, string name)
        {
            PageNavigationRegistry.Register(name, viewType);

            container.Configure(c => c.Export(viewType).AsKeyed(typeof(object), name));

            return container;
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the BindingContext for the Page</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        /// <param name="container"></param>
        public static DependencyInjectionContainer ConfigureTypeForNavigation<TView, TViewModel>(this DependencyInjectionContainer container, string name = null)
            where TView : Page
            where TViewModel : class
        {
            return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="container"><see cref="DependencyInjectionContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="androidView">Android Specific View Type</param>
        /// <param name="iOSView">iOS Specific View Type</param>
        /// <param name="otherView">Other Platform Specific View Type</param>
        /// <param name="windowsView">Windows Specific View Type</param>
        /// <param name="winPhoneView">Windows Phone Specific View Type</param>
        /// <returns><see cref="DependencyInjectionContainer"/></returns>
        [Obsolete("This signature of the RegisterTypeForNavigationOnPlatform method is obsolete due to Device.OnPlatform being deprecated. Use the new IPlatform[] overload instead.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static DependencyInjectionContainer ConfigureTypeForNavigationOnPlatform<TView, TViewModel>(this DependencyInjectionContainer container, string name = null, Type androidView = null, Type iOSView = null, Type otherView = null, Type windowsView = null, Type winPhoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            if (Device.OS == TargetPlatform.Android && androidView != null)
            {
                return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(androidView, name);
            }
            else if (Device.OS == TargetPlatform.iOS && iOSView != null)
            {
                return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(iOSView, name);
            }
            else if (Device.OS == TargetPlatform.Other && otherView != null)
            {
                return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(otherView, name);
            }
            else if (Device.OS == TargetPlatform.Windows && windowsView != null)
            {
                return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(windowsView, name);
            }
            else if (Device.OS == TargetPlatform.WinPhone && winPhoneView != null)
            {
                return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(winPhoneView, name);
            }
            else
            {
                return container.ConfigureTypeForNavigation<TView, TViewModel>(name);
            }
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="container"><see cref="DependencyInjectionContainer"/> used to register type for Navigation.</param>
        /// <param name="platforms"></param>
        public static DependencyInjectionContainer ConfigureTypeForNavigationOnPlatform<TView, TViewModel>(this DependencyInjectionContainer container, params IPlatform[] platforms)
            where TView : Page
            where TViewModel : class
        {
            var name = typeof(TView).Name;
            return ConfigureTypeForNavigationOnPlatform<TView, TViewModel>(container, name, platforms);
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="container"><see cref="DependencyInjectionContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page. If left empty or null will default to the View name.</param>
        /// <param name="platforms"></param>
        public static DependencyInjectionContainer ConfigureTypeForNavigationOnPlatform<TView, TViewModel>(this DependencyInjectionContainer container, string name, params IPlatform[] platforms)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            foreach (var platform in platforms)
            {
                if (Device.RuntimePlatform == platform.RuntimePlatform.ToString())
                    return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(platform.ViewType, name);
            }

            return container.ConfigureTypeForNavigation<TView, TViewModel>(name);
        }

        /// <summary>
        /// Registers a Page for navigation based on the Device Idiom using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be used across multiple Idioms if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">The shared ViewModel</typeparam>
        /// <param name="container"><see cref="DependencyInjectionContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The common name used for Navigation. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="desktopView">Desktop Specific View Type</param>
        /// <param name="tabletView">Tablet Specific View Type</param>
        /// <param name="phoneView">Phone Specific View Type</param>
        /// <returns><see cref="DependencyInjectionContainer"/></returns>
        public static DependencyInjectionContainer ConfigureTypeForNavigationOnIdiom<TView, TViewModel>(this DependencyInjectionContainer container, string name = null, Type desktopView = null, Type tabletView = null, Type phoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            if (Device.Idiom == TargetIdiom.Desktop && desktopView != null)
            {
                return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(desktopView, name);
            }
            else if (Device.Idiom == TargetIdiom.Phone && phoneView != null)
            {
                return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(phoneView, name);
            }
            else if (Device.Idiom == TargetIdiom.Tablet && tabletView != null)
            {
                return container.ConfigureTypeForNavigationWithViewModel<TViewModel>(tabletView, name);
            }
            else
            {
                return container.ConfigureTypeForNavigation<TView, TViewModel>(name);
            }
        }

        private static DependencyInjectionContainer ConfigureTypeForNavigationWithViewModel<TViewModel>(this DependencyInjectionContainer container, Type viewType, string name)
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            return container.ConfigureTypeForNavigation(viewType, name);
        }
    }
}
