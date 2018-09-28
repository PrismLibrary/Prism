using System.Threading;

namespace Prism.Navigation
{
    public static class INavigationParametersExtensions
    {
        internal static void SetNavigationMode(this INavigationParameters parameters, NavigationMode mode)
        {
            (parameters as INavigationParametersInternal).Add(nameof(NavigationMode), mode);
        }

        internal static void SetNavigationService(this INavigationParameters parameters, IPlatformNavigationService service)
        {
            (parameters as INavigationParametersInternal).Add(nameof(NavigationService), service);
        }

        internal static void SetSyncronizationContext(this INavigationParameters parameters, SynchronizationContext context)
        {
            (parameters as INavigationParametersInternal).Add(nameof(SynchronizationContext), context);
        }

        public static NavigationMode GetNavigationMode(this INavigationParameters parameters)
        {
            if (!(parameters as INavigationParametersInternal).ContainsKey(nameof(NavigationMode)))
            {
                return default(NavigationMode);
            }

            return (parameters as INavigationParametersInternal).GetValue<NavigationMode>(nameof(NavigationMode));
        }

        public static IPlatformNavigationService GetNavigationService(this INavigationParameters parameters)
        {
            if (!(parameters as INavigationParametersInternal).ContainsKey(nameof(NavigationService)))
            {
                return null;
            }

            return (parameters as INavigationParametersInternal).GetValue<IPlatformNavigationService>(nameof(NavigationService));
        }

        public static SynchronizationContext GetSynchronizationContext(this INavigationParameters parameters)
        {
            if (!(parameters as INavigationParametersInternal).ContainsKey(nameof(SynchronizationContext)))
            {
                return null;
            }

            return (parameters as INavigationParametersInternal).GetValue<SynchronizationContext>(nameof(SynchronizationContext));
        }
    }
}
