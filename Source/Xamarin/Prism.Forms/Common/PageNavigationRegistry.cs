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
        static Dictionary<Type, PageNavigationParametersAttribute> _pageNavigationParametersAttributeCache = new Dictionary<Type, PageNavigationParametersAttribute>();

        public static void Register(string name, Type pageType)
        {
            var info = new PageNavigationInfo();
            info.PageName = name;
            info.PageType = pageType;
            info.PageNavigationProvider = GetPageNavigationProvider(pageType);  //TODO: Not sure I want to create the provider now, or wait until it is requested during navigation
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

        public static IPageNavigationProvider GetPageNavigationProvider(string name)
        {
            return GetPageNavigationInfo(name)?.PageNavigationProvider;
        }

        static PageNavigationParametersAttribute GetPageNavigationParameters(Type pageType)
        {
            PageNavigationParametersAttribute attribute = null;

            if (_pageNavigationParametersAttributeCache.ContainsKey(pageType))
                attribute = _pageNavigationParametersAttributeCache[pageType];
            else
                attribute = pageType.GetTypeInfo().GetCustomAttribute<PageNavigationParametersAttribute>();

            if (!_pageNavigationParametersAttributeCache.ContainsKey(pageType))
                _pageNavigationParametersAttributeCache.Add(pageType, attribute);

            return attribute;
        }

        static PageNavigationProviderAttribute GetNavigationPageProviderAttribute(Type pageType)
        {
            return pageType.GetTypeInfo().GetCustomAttribute<PageNavigationProviderAttribute>(true);
        }

        static IPageNavigationProvider GetPageNavigationProvider(Type pageType)
        {
            IPageNavigationProvider provider = null;

            if (_navigationProviderCache.ContainsKey(pageType))
            {
                provider = _navigationProviderCache[pageType];
            }
            else
            {
                var navigationPageProvider = pageType.GetTypeInfo().GetCustomAttribute<PageNavigationProviderAttribute>();
                if (navigationPageProvider != null)
                {
                    provider = ServiceLocator.Current.GetInstance(navigationPageProvider.Type) as IPageNavigationProvider;
                    if (provider == null)
                        throw new InvalidCastException("Could not create the navigation page provider.  Please make sure the page navigation provider implements the IPageNavigationProvider interface.");
                }
            }

            if (!_navigationProviderCache.ContainsKey(pageType))
                _navigationProviderCache.Add(pageType, provider);

            return provider;
        }
    }

    public class PageNavigationInfo
    {
        public IPageNavigationProvider PageNavigationProvider { get; set; }

        public PageNavigationParametersAttribute PageNavigationParameters { get; set; }

        public string PageName { get; set; }

        public Type PageType { get; set; }
    }
}
