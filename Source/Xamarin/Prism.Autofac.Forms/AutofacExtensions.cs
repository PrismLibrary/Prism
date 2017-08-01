using System;
using Autofac;
using Prism.Mvvm;
using Xamarin.Forms;
using Prism.Navigation;

namespace Prism.Autofac
{
    /// <summary>
    /// Autofac View Registration Extensions
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <param name="builder"><see cref="ContainerBuilder"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static ContainerBuilder RegisterTypeForNavigation<TView>(this ContainerBuilder builder, string name = null) where TView : Page
        {
            Type viewType = typeof(TView);

            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            
            return builder.RegisterTypeForNavigation(viewType, name);
        }

        /// <summary>
        /// Registers a Page for navigation
        /// </summary>
        /// <param name="builder"><see cref="ContainerBuilder"/> used to register type for Navigation.</param>
        /// <param name="viewType">The type of Page to register</param>
        /// <param name="name">The unique name to register with the Page</param>
        /// <returns><see cref="ContainerBuilder"/></returns>
        public static ContainerBuilder RegisterTypeForNavigation(this ContainerBuilder builder, Type viewType, string name)
        {
            PageNavigationRegistry.Register(name, viewType);
            RegisterTypeIfMissing(builder, viewType, name);
            return builder;
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the BindingContext for the Page</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        /// <param name="builder"></param>
        public static ContainerBuilder RegisterTypeForNavigation<TView, TViewModel>(this ContainerBuilder builder, string name = null)
            where TView : Page
            where TViewModel : class
        {
            return builder.RegisterTypeForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="builder"><see cref="ContainerBuilder"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="androidView">Android Specific View Type</param>
        /// <param name="iOSView">iOS Specific View Type</param>
        /// <param name="otherView">Other Platform Specific View Type</param>
        /// <param name="windowsView">Windows Specific View Type</param>
        /// <param name="winPhoneView">Windows Phone Specific View Type</param>
        /// <returns><see cref="ContainerBuilder"/></returns>
        [Obsolete("This signature of the RegisterTypeForNavigationOnPlatform method is obsolete due to Device.OnPlatform being deprecated. Use the new IPlatform[] overload instead.")]
        public static ContainerBuilder RegisterTypeForNavigationOnPlatform<TView, TViewModel>(this ContainerBuilder builder, string name = null, Type androidView = null, Type iOSView = null, Type otherView = null, Type windowsView = null, Type winPhoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            if (Device.OS == TargetPlatform.Android && androidView != null)
            {
                return builder.RegisterTypeForNavigationWithViewModel<TViewModel>(androidView, name);
            }
            else if (Device.OS == TargetPlatform.iOS && iOSView != null)
            {
                return builder.RegisterTypeForNavigationWithViewModel<TViewModel>(iOSView, name);
            }
            else if (Device.OS == TargetPlatform.Other && otherView != null)
            {
                return builder.RegisterTypeForNavigationWithViewModel<TViewModel>(otherView, name);
            }
            else if (Device.OS == TargetPlatform.Windows && windowsView != null)
            {
                return builder.RegisterTypeForNavigationWithViewModel<TViewModel>(windowsView, name);
            }
            else if (Device.OS == TargetPlatform.WinPhone && winPhoneView != null)
            {
                return builder.RegisterTypeForNavigationWithViewModel<TViewModel>(winPhoneView, name);
            }
            else
            {
                return builder.RegisterTypeForNavigation<TView, TViewModel>(name);
            }
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="builder"><see cref="ContainerBuilder"/> used to register type for Navigation.</param>
        /// <param name="platforms"></param>
        public static void RegisterTypeForNavigationOnPlatform<TView, TViewModel>(this ContainerBuilder builder, params IPlatform[] platforms)
            where TView : Page
            where TViewModel : class
        {
            var name = typeof(TView).Name;
            RegisterTypeForNavigationOnPlatform<TView, TViewModel>(builder, name, platforms);
        }

        /// <summary>
        /// Registers a Page for navigation based on the current Device OS using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be shared across multiple Device Operating Systems if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">Shared ViewModel Type</typeparam>
        /// <param name="builder"><see cref="ContainerBuilder"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page. If left empty or null will default to the View name.</param>
        /// <param name="platforms"></param>
        public static void RegisterTypeForNavigationOnPlatform<TView, TViewModel>(this ContainerBuilder builder, string name, params IPlatform[] platforms)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            foreach (var platform in platforms)
            {
                if (Device.RuntimePlatform == platform.RuntimePlatform.ToString())
                {
                    builder.RegisterTypeForNavigationWithViewModel<TViewModel>(platform.ViewType, name);
                    return;
                }
            }

            builder.RegisterTypeForNavigation<TView, TViewModel>(name);
        }

        /// <summary>
        /// Registers a Page for navigation based on the Device Idiom using a shared ViewModel
        /// </summary>
        /// <typeparam name="TView">Default View Type to be used across multiple Idioms if they are not specified directly.</typeparam>
        /// <typeparam name="TViewModel">The shared ViewModel</typeparam>
        /// <param name="builder"><see cref="ContainerBuilder"/> used to register type for Navigation.</param>
        /// <param name="name">The common name used for Navigation. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="desktopView">Desktop Specific View Type</param>
        /// <param name="tabletView">Tablet Specific View Type</param>
        /// <param name="phoneView">Phone Specific View Type</param>
        /// <returns><see cref="ContainerBuilder"/></returns>
        public static ContainerBuilder RegisterTypeForNavigationOnIdiom<TView, TViewModel>(this ContainerBuilder builder, string name = null, Type desktopView = null, Type tabletView = null, Type phoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            if (Device.Idiom == TargetIdiom.Desktop && desktopView != null)
            {
                return builder.RegisterTypeForNavigationWithViewModel<TViewModel>(desktopView, name);
            }
            else if (Device.Idiom == TargetIdiom.Phone && phoneView != null)
            {
                return builder.RegisterTypeForNavigationWithViewModel<TViewModel>(phoneView, name);
            }
            else if (Device.Idiom == TargetIdiom.Tablet && tabletView != null)
            {
                return builder.RegisterTypeForNavigationWithViewModel<TViewModel>(tabletView, name);
            }
            else
            {
                return builder.RegisterTypeForNavigation<TView, TViewModel>(name);
            }
        }

        private static ContainerBuilder RegisterTypeForNavigationWithViewModel<TViewModel>(this ContainerBuilder builder, Type viewType, string name)
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            return builder.RegisterTypeForNavigation(viewType, name);
        }

        /// <summary>
        /// Registers a type in the builder only if that type was not already registered,
        /// after the builder is already created.
        /// Uses a new ContainerBuilder instance to update the builder.
        /// </summary>
        /// <param name="builder">The Container Builder.</param>
        /// <param name="type">The type to register.</param>
        /// <param name="name">The name you will use to resolve the component in future.</param>
        private static void RegisterTypeIfMissing(ContainerBuilder builder, Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            builder.RegisterType(type).Named<Page>(name);
        }
    }
}
