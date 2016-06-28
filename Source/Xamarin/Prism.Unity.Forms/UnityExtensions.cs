using System;
using Microsoft.Practices.Unity;
using Xamarin.Forms;
using Prism.Common;
using Prism.Mvvm;

namespace Prism.Unity
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <param name="container"><see cref="IUnityContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static IUnityContainer RegisterTypeForNavigation<TView>(this IUnityContainer container, string name = null) where TView : Page
        {
            var viewType = typeof(TView);

            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            return container.RegisterTypeForNavigation(viewType, name);
        }

        /// <summary>
        /// Registers a Page for navigation
        /// </summary>
        /// <param name="container"><see cref="IUnityContainer"/> used to register type for Navigation.</param>
        /// <param name="viewType">The type of Page to register</param>
        /// <param name="name">The unique name to register with the Page</param>
        /// <returns><see cref="IUnityContainer"/></returns>
        public static IUnityContainer RegisterTypeForNavigation(this IUnityContainer container, Type viewType, string name)
        {
            PageNavigationRegistry.Register(name, viewType);
            return container.RegisterType(typeof(object), viewType, name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the BindingContext for the Page</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        /// <param name="container"></param>
        public static IUnityContainer RegisterTypeForNavigation<TView, TViewModel>(this IUnityContainer container, string name = null)
            where TView : Page
            where TViewModel : class
        {
            return container.RegisterTypeForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="container"><see cref="IUnityContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="AndroidView">Android Specific View Type</param>
        /// <param name="iOSView">iOS Specific View Type</param>
        /// <param name="OtherView">Other Platform Specific View Type</param>
        /// <param name="WindowsView">Windows Specific View Type</param>
        /// <param name="WinPhoneView">Windows Phone Specific View Type</param>
        /// <returns><see cref="IUnityContainer"/></returns>
        public static IUnityContainer RegisterTypeForNavigationOnPlatform<TView, TViewModel>(this IUnityContainer container, string name = null, Type AndroidView = null, Type iOSView = null, Type OtherView = null, Type WindowsView = null, Type WinPhoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (Device.OS == TargetPlatform.Android && AndroidView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(AndroidView, name);
            }
            else if (Device.OS == TargetPlatform.iOS && iOSView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(iOSView, name);
            }
            else if (Device.OS == TargetPlatform.Other && OtherView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(OtherView, name);
            }
            else if (Device.OS == TargetPlatform.Windows && WindowsView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(WindowsView, name);
            }
            else if (Device.OS == TargetPlatform.WinPhone && WinPhoneView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(WinPhoneView, name);
            }
            else
            {
                return container.RegisterTypeForNavigation<TView, TViewModel>(name);
            }
        }

        /// <summary>
        /// Registers a Page for navigation based on the Device Idiom using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be used across multiple Idioms if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">The shared ViewModel</typeparam>
        /// <param name="container"><see cref="IUnityContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The common name used for Navigation. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="DesktopView">Desktop Specific View Type</param>
        /// <param name="TabletView">Tablet Specific View Type</param>
        /// <param name="PhoneView">Phone Specific View Type</param>
        /// <returns><see cref="IUnityContainer"/></returns>
        public static IUnityContainer RegisterTypeForNavigationOnIdiom<TView, TViewModel>(this IUnityContainer container, string name = null, Type DesktopView = null, Type TabletView = null, Type PhoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (Device.Idiom == TargetIdiom.Desktop && DesktopView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(DesktopView, name);
            }
            else if (Device.Idiom == TargetIdiom.Phone && PhoneView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(PhoneView, name);
            }
            else if (Device.Idiom == TargetIdiom.Tablet && TabletView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(TabletView, name);
            }
            else
            {
                return container.RegisterTypeForNavigation<TView, TViewModel>(name);
            }
        }


        //TODO: decide if Prism will contniue to support navigating via ViewModels, or should we stick to just a single approach to navigation
        //public static IUnityContainer RegisterTypeForViewModelNavigation<TView, TViewModel>(this IUnityContainer container)
        //    where TView : Page
        //    where TViewModel : class
        //{
        //    return container.RegisterTypeForNavigationWithViewModel<TViewModel>(typeof(TView), typeof(TViewModel).FullName);
        //}

        //public static IUnityContainer RegisterTypeForViewModelNavigationOnPlatform<TView, TViewModel>(this IUnityContainer container, Type AndroidView = null, Type iOSView = null, Type OtherView = null, Type WindowsView = null, Type WinPhoneView = null)
        //    where TView : Page
        //    where TViewModel : class
        //{
        //    return container.RegisterTypeForNavigationOnPlatform<TView, TViewModel>(typeof(TViewModel).FullName, AndroidView, iOSView, OtherView, WindowsView, WinPhoneView);
        //}

        //public static IUnityContainer RegisterTypeForViewModelNavigationOnIdiom<TView, TViewModel>(this IUnityContainer container, Type DesktopView = null, Type TabletView = null, Type PhoneView = null)
        //    where TView : Page
        //    where TViewModel : class
        //{
        //    return container.RegisterTypeForNavigationOnIdiom<TView, TViewModel>(typeof(TViewModel).FullName, DesktopView, TabletView, PhoneView);
        //}

        private static IUnityContainer RegisterTypeForNavigationWithViewModel<TViewModel>(this IUnityContainer container, Type viewType, string name = null)
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            return container.RegisterTypeForNavigation(viewType, name);
        }
    }
}
