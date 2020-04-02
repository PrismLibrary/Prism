using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Prism.Ioc
{
    internal static class IContainerRegistryAutoLoadExtensions
    {
        public static void AutoRegisterViews(this Type type, IContainerRegistry containerRegistry)
        {
            if (!type.GetCustomAttributes().Any(a => a is AutoRegisterForNavigationAttribute)) return;

            var regAttr = type.GetCustomAttribute<AutoRegisterForNavigationAttribute>();
            var assembly = type.Assembly;

            var viewTypes = assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(Page)) && !t.GetTypeInfo().IsGenericTypeDefinition && !t.GetTypeInfo().ContainsGenericParameters);
            RegisterViewsAutomatically(containerRegistry, viewTypes);
        }

        private static void RegisterViewsAutomatically(IContainerRegistry containerRegistry, IEnumerable<Type> viewTypes)
        {
            foreach (var viewType in viewTypes)
            {
                RegisterView(containerRegistry, viewType);
            }

            RegisterView(containerRegistry, typeof(NavigationPage), true);
            RegisterView(containerRegistry, typeof(TabbedPage), true);
        }

        private static void RegisterView(IContainerRegistry containerRegistry, Type viewType, bool checkIfRegistered = false)
        {
            var name = AutoRegistrationViewNameProvider.GetNavigationSegmentName(viewType);

            if(!checkIfRegistered || containerRegistry.IsRegistered<object>(name))
            {
                containerRegistry.RegisterForNavigation(viewType, name);
            }
        }
    }
}
