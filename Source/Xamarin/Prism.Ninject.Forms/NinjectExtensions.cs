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
        /// Registers a Page for navigation using a convention based approach, which uses the name of the Type being passed in as the unique name.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        public static void RegisterTypeForNavigation<TView>(this IKernel kernel) where TView : Page
        {
            kernel.RegisterTypeForNavigation<TView>(typeof(TView).Name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <param name="kernel"><see cref="IKernel"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation<TView>(this IKernel kernel, string name) where TView : Page
        {
            Type type = typeof(TView);
            kernel.Bind<object>().To(type).Named(name);

            PageNavigationRegistry.Register(name, type);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <typeparam name="TViewModel">The BindableBase ViewModel to use as the unique name for the Page</typeparam>
        /// <param name="kernel"><see cref="IKernel"/> used to register type for Navigation.</param>
        public static void RegisterTypeForNavigation<TView, TViewModel>(this IKernel kernel)
            where TView : Page
            where TViewModel : BindableBase
        {
            Type type = typeof(TView);
            string name = typeof(TViewModel).FullName;

            kernel.Bind<object>().To(type).Named(name);

            PageNavigationRegistry.Register(name, type);
        }
    }
}
