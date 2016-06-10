using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Prism.Common;
using Xamarin.Forms;

namespace Prism.Autofac.Forms
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers a Page for navigation using a convention based approach, which uses the name of the Type being passed in as the unique name.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        public static void RegisterTypeForNavigation<T>(this IContainer container) where T : Page
        {
            container.RegisterTypeForNavigation<T>(typeof(T).Name);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        /// <param name="name">The unique name to register with the Page</param>
        public static void RegisterTypeForNavigation<T>(this IContainer container, string name) where T : Page
        {
            Type type = typeof(T);

            container.RegisterTypeIfMissing(type, name);

            PageNavigationRegistry.Register(name, type);
        }

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of Page to register</typeparam>
        /// <typeparam name="C">The Class to use as the unique name for the Page</typeparam>
        /// <param name="container"></param>
        public static void RegisterTypeForNavigation<T, C>(this IContainer container)
            where T : Page
            where C : class
        {
            Type type = typeof(T);
            string name = typeof(C).FullName;

            container.RegisterTypeIfMissing(type, name);

            PageNavigationRegistry.Register(name, type);
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered,
        /// after the container is already created.
        /// Uses a new ContainerBuilder instance to update the Container.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="name">The name you will use to resolve the component in future.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        public static void RegisterTypeIfMissing(this IContainer container, Type type, string name, bool registerAsSingleton = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (container.IsRegistered(type))
            {
                //Logger.Log(String.Format(CultureInfo.CurrentCulture,
                //                        "Type {0} already registered with container",
                //                        fromType.Name), Category.Debug, Priority.Low);
            }
            else
            {
                ContainerBuilder containerUpdater = new ContainerBuilder();
                if (registerAsSingleton)
                {
                    containerUpdater.RegisterType(type).Named<Page>(name).SingleInstance();
                }
                else
                {
                    containerUpdater.RegisterType(type).Named<Page>(name);
                }
                containerUpdater.Update(container);
            }
        }
    }
}
