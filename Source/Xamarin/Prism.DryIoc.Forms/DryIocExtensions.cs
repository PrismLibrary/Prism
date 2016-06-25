using DryIoc;
using Prism.Common;
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
        /// <typeparam name="TPage">The Type of Page to register</typeparam>
        public static void RegisterTypeForNavigation<TPage>(this IContainer container) where TPage : Page
        {
            container.RegisterTypeForNavigation<TPage>(typeof(TPage).Name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TPage">The Type of Page to register</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation<TPage>(this IContainer container, string name) where TPage : Page
        {
            container.RegisterMany<TPage>(serviceKey: name);
            PageNavigationRegistry.Register(name, typeof(TPage));
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="TPage">The Type of Page to register</typeparam>
        /// <typeparam name="TClass">The Class to use as the unique name for the Page</typeparam>
        public static void RegisterTypeForNavigation<TPage, TClass>(this IContainer container) where TPage : Page
            where TClass : class
        {
            container.RegisterTypeForNavigation<TPage>(typeof(TClass).Name);
        }
    }
}