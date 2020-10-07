using System;
using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.Ioc
{
    /// <summary>
    /// Provides Navigation Registration Extensions for Region Navigation
    /// </summary>
    public static class RegionNavigationRegistrationExtensions
    {
        /// <summary>
        /// Registers a <see cref="View"/> for region navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of <see cref="View"/> to register</typeparam>
        /// <param name="containerRegistry"><see cref="IContainerRegistry"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the View</param>
        public static void RegisterForRegionNavigation<TView>(this IContainerRegistry containerRegistry, string name = null)
            where TView : View
        {
            var viewType = typeof(TView);

            if (string.IsNullOrWhiteSpace(name))
                name = viewType.Name;

            containerRegistry.RegisterForNavigation(viewType, name);
        }

        /// <summary>
        /// Registers a <see cref="View"/> for region navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of <see cref="View" />to register</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the BindingContext for the View</typeparam>
        /// <param name="name">The unique name to register with the View</param>
        /// <param name="containerRegistry"></param>
        public static void RegisterForRegionNavigation<TView, TViewModel>(this IContainerRegistry containerRegistry, string name = null)
            where TView : View
            where TViewModel : class
        {
            containerRegistry.RegisterForNavigationWithViewModel<TViewModel>(typeof(TView), name);
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
