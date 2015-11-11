using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace Prism.Common
{
    public static class PageNavigationRegistry
    {
        static Dictionary<string, PageNavigationInfo> _pageRegistrationCache = new Dictionary<string, PageNavigationInfo>();
        static Dictionary<Type, IPageNavigationProvider> _navigationProviderCache = new Dictionary<Type, IPageNavigationProvider>();
        static Dictionary<Type, PageNavigationOptionsAttribute> _pageNavigationOptionsAttributeCache = new Dictionary<Type, PageNavigationOptionsAttribute>();

        public static void Register(string name, Type pageType)
        {
            var info = new PageNavigationInfo();
            info.Name = name;
            info.Type = pageType;
            info.NavigationOptions = GetPageNavigationOptions(pageType);

            if (!_pageRegistrationCache.ContainsKey(name))
                _pageRegistrationCache.Add(name, info);
        }

        public static PageNavigationInfo GetPageNavigationInfo(string name)
        {
            if (_pageRegistrationCache.ContainsKey(name))
                return _pageRegistrationCache[name];

            return null;
        }

        public static PageNavigationOptionsAttribute GetPageNavigationOptions(string name)
        {
            return GetPageNavigationInfo(name)?.NavigationOptions;
        }

        public static Type GetPageType(string name)
        {
            return GetPageNavigationInfo(name)?.Type;
        }

        public static IPageNavigationProvider GetPageNavigationProvider(string name)
        {
            IPageNavigationProvider provider = null;

            var providerType = GetPageNavigationInfo(name)?.NavigationOptions?.PageNavigationProviderType;

            if (providerType == null)
                return null;

            if (_navigationProviderCache.ContainsKey(providerType))
            {
                provider = _navigationProviderCache[providerType];
            }
            else
            {
                if (providerType != null)
                {
                    provider = ServiceLocator.Current.GetInstance(providerType) as IPageNavigationProvider;
                    if (provider == null)
                        throw new InvalidCastException("Could not create the page navigation provider.  Please make sure the page navigation provider implements the IPageNavigationProvider interface.");
                }
            }

            if (!_navigationProviderCache.ContainsKey(providerType))
                _navigationProviderCache.Add(providerType, provider);

            return provider;
        }

        static PageNavigationOptionsAttribute GetPageNavigationOptions(Type pageType)
        {
            PageNavigationOptionsAttribute attribute = null;

            if (_pageNavigationOptionsAttributeCache.ContainsKey(pageType))
                attribute = _pageNavigationOptionsAttributeCache[pageType];
            else
                attribute = pageType.GetTypeInfo().GetCustomAttribute<PageNavigationOptionsAttribute>();

            if (!_pageNavigationOptionsAttributeCache.ContainsKey(pageType))
                _pageNavigationOptionsAttributeCache.Add(pageType, attribute);

            return attribute;
        }
    }
}
