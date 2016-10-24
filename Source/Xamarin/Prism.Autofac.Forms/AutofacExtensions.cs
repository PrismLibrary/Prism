using System;
using Autofac;
using Prism.Mvvm;
using Xamarin.Forms;
using Prism.Navigation;

namespace Prism.Autofac.Forms
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <param name="container"><see cref="IContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static IContainer RegisterTypeForNavigation<TView>(this IContainer container, string name = null) where TView : Page
        {
            Type viewType = typeof(TView);

            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            
            return container.RegisterTypeForNavigation(viewType, name);
        }

        /// <summary>
        /// Registers a Page for navigation
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> used to register type for Navigation.</param>
        /// <param name="viewType">The type of Page to register</param>
        /// <param name="name">The unique name to register with the Page</param>
        /// <returns><see cref="IContainer"/></returns>
        public static IContainer RegisterTypeForNavigation(this IContainer container, Type viewType, string name)
        {
            PageNavigationRegistry.Register(name, viewType);
            RegisterTypeIfMissing(container, viewType, name);
            return container;
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the BindingContext for the Page</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        /// <param name="container"></param>
        public static IContainer RegisterTypeForNavigation<TView, TViewModel>(this IContainer container, string name = null)
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
        /// <param name="container"><see cref="IContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="androidView">Android Specific View Type</param>
        /// <param name="iOSView">iOS Specific View Type</param>
        /// <param name="otherView">Other Platform Specific View Type</param>
        /// <param name="windowsView">Windows Specific View Type</param>
        /// <param name="winPhoneView">Windows Phone Specific View Type</param>
        /// <returns><see cref="IUnityContainer"/></returns>
        public static IContainer RegisterTypeForNavigationOnPlatform<TView, TViewModel>(this IContainer container, string name = null, Type androidView = null, Type iOSView = null, Type otherView = null, Type windowsView = null, Type winPhoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            if (Device.OS == TargetPlatform.Android && androidView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(androidView, name);
            }
            else if (Device.OS == TargetPlatform.iOS && iOSView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(iOSView, name);
            }
            else if (Device.OS == TargetPlatform.Other && otherView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(otherView, name);
            }
            else if (Device.OS == TargetPlatform.Windows && windowsView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(windowsView, name);
            }
            else if (Device.OS == TargetPlatform.WinPhone && winPhoneView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(winPhoneView, name);
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
        /// <param name="container"><see cref="IContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The common name used for Navigation. If left empty or null will default to the ViewModel root name. i.e. MyPageViewModel => MyPage</param>
        /// <param name="desktopView">Desktop Specific View Type</param>
        /// <param name="tabletView">Tablet Specific View Type</param>
        /// <param name="phoneView">Phone Specific View Type</param>
        /// <returns><see cref="IUnityContainer"/></returns>
        public static IContainer RegisterTypeForNavigationOnIdiom<TView, TViewModel>(this IContainer container, string name = null, Type desktopView = null, Type tabletView = null, Type phoneView = null)
            where TView : Page
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TView).Name;

            if (Device.Idiom == TargetIdiom.Desktop && desktopView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(desktopView, name);
            }
            else if (Device.Idiom == TargetIdiom.Phone && phoneView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(phoneView, name);
            }
            else if (Device.Idiom == TargetIdiom.Tablet && tabletView != null)
            {
                return container.RegisterTypeForNavigationWithViewModel<TViewModel>(tabletView, name);
            }
            else
            {
                return container.RegisterTypeForNavigation<TView, TViewModel>(name);
            }
        }

        private static IContainer RegisterTypeForNavigationWithViewModel<TViewModel>(this IContainer container, Type viewType, string name)
            where TViewModel : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));

            return container.RegisterTypeForNavigation(viewType, name);
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered,
        /// after the container is already created.
        /// Uses a new ContainerBuilder instance to update the Container.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="name">The name you will use to resolve the component in future.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        private static void RegisterTypeIfMissing(IContainer container, Type type, string name, bool registerAsSingleton = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!container.IsRegistered(type))
            {
                var containerUpdater = new ContainerBuilder();

                if (registerAsSingleton)
                    containerUpdater.RegisterType(type).Named<Page>(name).SingleInstance();
                else
                    containerUpdater.RegisterType(type).Named<Page>(name);

                containerUpdater.Update(container);
            }
        }
    }
}
