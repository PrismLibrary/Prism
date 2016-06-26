using DryIoc;
using Prism.Common;
using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.DryIoc
{
    /// <summary>
    /// Extension methods to register a <see cref="Xamarin.Forms.Page"/> with <see cref="PageNavigationRegistry"/> using <see cref="IContainer"/>
    /// </summary>
    public static class DryIocExtensions
    {
        /// <summary>
        /// Registers a Page for navigation using a convention based approach, which uses the name of the Type being passed in as the unique name.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        public static void RegisterTypeForNavigation<TView>(this IContainer container) where TView : Page
        {
            container.RegisterTypeForNavigation<TView>(typeof(TView).Name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation<TView>(this IContainer container, string name) where TView : Page
        {
            container.RegisterMany<TView>(serviceKey: name);
            PageNavigationRegistry.Register(name, typeof(TView));
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TView">The Type of Page to register</typeparam>
        /// <typeparam name="TViewModel">The ViewModel to use as the unique name for the Page</typeparam>
        public static void RegisterTypeForNavigation<TView, TViewModel>(this IContainer container) where TView : Page
            where TViewModel : BindableBase
        {
            container.RegisterTypeForNavigation<TView>(typeof(TViewModel).FullName);
        }
    }
}