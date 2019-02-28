using Prism.Mvvm;
using System;

namespace Prism.Ioc
{
    public static class IContainerRegistryExtensions
    {
        /// <summary>
        /// Registers an object to be used as a dialog in the IDialogService.
        /// </summary>
        /// <typeparam name="TView">The Type of object to register as the dialog</typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="name">The unique name to register with the dialog.</param>
        public static void RegisterDialog<TView>(this IContainerRegistry containerRegistry, string name = null)
        {
            containerRegistry.RegisterForNavigation<TView>(name);
        }

        /// <summary>
        /// Registers an object to be used as a dialog in the IDialogService.
        /// </summary>
        /// <typeparam name="TView">The Type of object to register as the dialog</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the DataContext for the dialog</typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="name">The unique name to register with the dialog.</param>
        public static void RegisterDialog<TView, TViewModel>(this IContainerRegistry containerRegistry, string name = null) where TViewModel : Services.Dialogs.IDialogAware
        {
            containerRegistry.RegisterForNavigation<TView, TViewModel>(name);
        }

        /// <summary>
        /// Registers an object that implements IDialogWindow to be used to host all dialogs in the IDialogService.
        /// </summary>
        /// <typeparam name="TWindow">The Type of the Window class that will be used to host dialogs in the IDialogService</typeparam>
        /// <param name="containerRegistry"></param>
        public static void RegisterDialogWindow<TWindow>(this IContainerRegistry containerRegistry) where TWindow : Services.Dialogs.IDialogWindow
        {
            containerRegistry.Register(typeof(Services.Dialogs.IDialogWindow), typeof(TWindow));
        }

        /// <summary>
        /// Registers an object for navigation
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="type">The type of object to register</param>
        /// <param name="name">The unique name to register with the obect.</param>
        public static void RegisterForNavigation(this IContainerRegistry containerRegistry, Type type, string name)
        {
            containerRegistry.Register(typeof(object), type, name);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register as the view</typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="name">The unique name to register with the object.</param>
        public static void RegisterForNavigation<T>(this IContainerRegistry containerRegistry, string name = null)
        {
            Type type = typeof(T);
            string viewName = string.IsNullOrWhiteSpace(name) ? type.Name : name;
            containerRegistry.RegisterForNavigation(type, viewName);
        }

        /// <summary>
        /// Registers an object for navigation with the ViewModel type to be used as the DataContext.
        /// </summary>
        /// <typeparam name="TView">The Type of object to register as the view</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the DataContext for the view</typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="name">The unique name to register with the view</param>
        public static void RegisterForNavigation<TView, TViewModel>(this IContainerRegistry containerRegistry, string name = null)
        {
            containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(typeof(TView), name);
        }

        private static void RegisterForNavigationWithViewModel<TViewModel>(this IContainerRegistry containerRegistry, Type viewType, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            ViewModelLocationProvider.Register(viewType.ToString(), typeof(TViewModel));
            containerRegistry.RegisterForNavigation(viewType, name);
        }
    }
}
