using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace Prism.Common
{
    public static class PageNavigationRegistry
    {
        //public static Dictionary<string, Type> PageRegistrationCache = new Dictionary<string, Type>();

        static Dictionary<string, PageNavigationInfo> _pageRegistrationCache = new Dictionary<string, PageNavigationInfo>();
        static Dictionary<Type, INavigationPageProvider> _navigationProviderCache = new Dictionary<Type, INavigationPageProvider>();
        static Dictionary<Type, PageNavigationParametersAttribute> _pageNavigationParametersAttributeCache = new Dictionary<Type, PageNavigationParametersAttribute>();

        public static void Register(string name, Type pageType)
        {
            var info = new PageNavigationInfo();
            info.PageName = name;
            info.PageType = pageType;
            info.NavigationPageProvider = GetNavigationPageProvider(pageType);
            info.PageNavigationParameters = GetPageNavigationParameters(pageType);

            _pageRegistrationCache.Add(name, info);
        }

        public static PageNavigationInfo GetPageNavigationInfo(string name)
        {
            if (_pageRegistrationCache.ContainsKey(name))
                return _pageRegistrationCache[name];

            return null;
        }

        public static PageNavigationParametersAttribute GetPageNavigationParameters(string name)
        {
            return GetPageNavigationInfo(name)?.PageNavigationParameters;
        }

        public static Type GetPageType(string name)
        {
            return GetPageNavigationInfo(name)?.PageType;
        }

        public static INavigationPageProvider GetNavigatinPageProvider(string name)
        {
            return GetPageNavigationInfo(name)?.NavigationPageProvider;
        }

        static INavigationPageProvider GetNavigationPageProvider(Type pageType)
        {
            INavigationPageProvider provider = null;

            if (_navigationProviderCache.ContainsKey(pageType))
            {
                provider = _navigationProviderCache[pageType];
            }
            else
            {
                var navigationPageProvider = pageType.GetTypeInfo().GetCustomAttribute<NavigationPageProviderAttribute>(true);
                if (navigationPageProvider != null)
                {
                    provider = ServiceLocator.Current.GetInstance(navigationPageProvider.Type) as INavigationPageProvider;
                    if (provider == null)
                        throw new InvalidCastException("Could not create the navigation page provider.  Please make sure the navigation page provider implements the INavigationPageProvider interface.");
                }
            }

            if (!_navigationProviderCache.ContainsKey(pageType))
                _navigationProviderCache.Add(pageType, provider);

            return provider;
        }

        static PageNavigationParametersAttribute GetPageNavigationParameters(Type pageType)
        {
            PageNavigationParametersAttribute attributes = null;

            if (_pageNavigationParametersAttributeCache.ContainsKey(pageType))
                attributes = _pageNavigationParametersAttributeCache[pageType];
            else
                attributes = pageType.GetTypeInfo().GetCustomAttribute<PageNavigationParametersAttribute>();

            if (!_pageNavigationParametersAttributeCache.ContainsKey(pageType))
                _pageNavigationParametersAttributeCache.Add(pageType, attributes);

            return attributes;
        }
    }

    public class PageNavigationInfo
    {
        public INavigationPageProvider NavigationPageProvider { get; set; }

        public PageNavigationParametersAttribute PageNavigationParameters { get; set; }

        public string PageName { get; set; }

        public Type PageType { get; set; }
    }
}
