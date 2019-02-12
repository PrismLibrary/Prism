using System.Threading;

namespace Prism.Navigation
{
    public static class INavigationParametersExtensions
    {
        internal static void SetNavigationMode(this INavigationParameters parameters, NavigationMode mode)
        {
            if ((parameters as INavigationParametersInternal).ContainsKey(nameof(NavigationMode)))
            {
                (parameters as INavigationParametersInternal).Remove(nameof(NavigationMode));
            }
            (parameters as INavigationParametersInternal).Add(nameof(NavigationMode), mode);
        }

        internal static void SetNavigationService(this INavigationParameters parameters, IPlatformNavigationService service)
        {
            if ((parameters as INavigationParametersInternal).ContainsKey(nameof(NavigationService)))
            {
                (parameters as INavigationParametersInternal).Remove(nameof(NavigationService));
            }
            (parameters as INavigationParametersInternal).Add(nameof(NavigationService), service);
        }

        internal static void SetSyncronizationContext(this INavigationParameters parameters, SynchronizationContext context)
        {
            if ((parameters as INavigationParametersInternal).ContainsKey(nameof(SynchronizationContext)))
            {
                (parameters as INavigationParametersInternal).Remove(nameof(SynchronizationContext));
            }
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
