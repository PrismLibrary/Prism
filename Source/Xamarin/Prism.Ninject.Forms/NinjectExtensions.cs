using System;
using Ninject;
using Xamarin.Forms;
using Prism.Common;
using Prism.Mvvm;

namespace Prism.Ninject
{
    public static class NinjectExtensions
    {
        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <param name="container"><see cref="IKernel"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation<TView>(this IKernel container, string name = null) where TView : Page
        {
            var viewType = typeof(TView);

            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            container.RegisterTypeForNavigation(viewType, name);
        }

        /// <summary>
        /// Registers a Page for navigation
        /// </summary>
        /// <param name="container"><see cref="IKernel"/> used to register type for Navigation.</param>
        /// <param name="viewType">The type of Page to register</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation(this IKernel container, Type viewType, string name)
        {
            PageNavigationRegistry.Register(name, viewType);
            container.Bind<object>().To(viewType).Named(name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the unique name for the Page</typeparam>
        /// <param name="container"><see cref="IKernel"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation<TView, TViewModel>(this IKernel container, string name = null)
            where TView : Page
            where TViewModel : class
        {
            container.RegisterTypeForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="container"><see cref="IKernel"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="AndroidView">Android Specific View Type</param>
        /// <param name="iOSView">iOS Specific View Type</param>
        /// <param name="OtherView">Other Platform Specific View Type</param>
        /// <param name="WindowsView">Windows Specific View Type</param>
        /// <param name="WinPhoneView">Windows Phone Specific View Type</param>
        public static void RegisterTypeForNavigationOnPlatform<TView, TViewModel>(this IKernel container, string name = null, Type AndroidView = null, Type iOSView = null, Type OtherView = null, Type WindowsView = null, Type WinPhoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (Device.OS == TargetPlatform.Android && AndroidView != null)
            {
                container.RegisterTypeForNavigationWithViewModel<TViewModel>(AndroidView, name);
            }
            else if (Device.OS == TargetPlatform.iOS && iOSView != null)
            {
                container.RegisterTypeForNavigationWithViewModel<TViewModel>(iOSView, name);
            }
            else if (Device.OS == TargetPlatform.Other && OtherView != null)
            {
                container.RegisterTypeForNavigationWithViewModel<TViewModel>(OtherView, name);
            }
            else if (Device.OS == TargetPlatform.Windows && WindowsView != null)
            {
                container.RegisterTypeForNavigationWithViewModel<TViewModel>(WindowsView, name);
            }
            else if (Device.OS == TargetPlatform.WinPhone && WinPhoneView != null)
            {
                container.RegisterTypeForNavigationWithViewModel<TViewModel>(WinPhoneView, name);
            }
            else
            {
                container.RegisterTypeForNavigation<TView, TViewModel>(name);
            }
        }

        /// <summary>
        /// Registers a Page for navigation based on the Device Idiom using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be used across multiple Idioms if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">The shared ViewModel</typeparam>
        /// <param name="container"><see cref="IKernel"/> used to register type for Navigation.</param>
        /// <param name="name">The common name used for Navigation. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="DesktopView">Desktop Specific View Type</param>
        /// <param name="TabletView">Tablet Specific View Type</param>
        /// <param name="PhoneView">Phone Specific View Type</param>
        public static void RegisterTypeForNavigationOnIdiom<TView, TViewModel>(this IKernel container, string name = null, Type DesktopView = null, Type TabletView = null, Type PhoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (Device.Idiom == TargetIdiom.Desktop && DesktopView != null)
            {
                container.RegisterTypeForNavigationWithViewModel<TViewModel>(DesktopView, name);
            }
            else if (Device.Idiom == TargetIdiom.Phone && PhoneView != null)
            {
                container.RegisterTypeForNavigationWithViewModel<TViewModel>(PhoneView, name);
            }
            else if (Device.Idiom == TargetIdiom.Tablet && TabletView != null)
            {
                container.RegisterTypeForNavigationWithViewModel<TViewModel>(TabletView, name);
            }
            else
            {
                container.RegisterTypeForNavigation<TView, TViewModel>(name);
            }
        }

        //TODO: decide if Prism will contniue to support navigating via ViewModels, or should we stick to just a single approach to navigation
        //public static void RegisterTypeForViewModelNavigation<TView, TViewModel>(this IKernel container)
        //    where TView : Page
        //    where TViewModel : class
        //{
        //    container.RegisterTypeForNavigationWithViewModel<TViewModel>(typeof(TView), typeof(TViewModel).FullName);
        //}

        //public static void RegisterTypeForViewModelNavigationOnPlatform<TView, TViewModel>(this IKernel container, Type AndroidView = null, Type iOSView = null, Type OtherView = null, Type WindowsView = null, Type WinPhoneView = null)
        //    where TView : Page
        //    where TViewModel : class
        //{
        //    container.RegisterTypeForNavigationOnPlatform<TView, TViewModel>(typeof(TViewModel).FullName, AndroidView, iOSView, OtherView, WindowsView, WinPhoneView);
        //}

        //public static void RegisterTypeForViewModelNavigationOnIdiom<TView, TViewModel>(this IKernel container, Type DesktopView = null, Type TabletView = null, Type PhoneView = null)
        //    where TView : Page
        //    where TViewModel : class
        //{
        //    container.RegisterTypeForNavigationOnIdiom<TView, TViewModel>(typeof(TViewModel).FullName, DesktopView, TabletView, PhoneView);
        //}


        private static void RegisterTypeForNavigationWithViewModel<TViewModel>(this IKernel container, Type viewType, string name = null)
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            container.RegisterTypeForNavigation(viewType, name);
        }
    }
}
