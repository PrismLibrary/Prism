using System;

namespace Prism.Ioc
{
    internal static class AutoRegistrationViewNameProvider
    {
        private static Func<Type, string> _defaultProvider = DefaultProvider;

        public static void SetDefaultProvider(Func<Type, string> navigationSegmentNameProvider) =>
            _defaultProvider = navigationSegmentNameProvider;

        public static string GetNavigationSegmentName(Type viewType)
        {
            if (_defaultProvider is null)
                _defaultProvider = DefaultProvider;

            return _defaultProvider(viewType);
        }

        private static string DefaultProvider(Type type) =>
            type.Name;
    }
}
