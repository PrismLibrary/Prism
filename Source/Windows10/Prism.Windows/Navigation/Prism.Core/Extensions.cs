using Prism.Navigation;
using System;
using System.Threading;
using Windows.UI.Core;

namespace Prism.Navigation
{
    public static partial class IContainerRegistryExtensions
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
            if (!(parameters as INavigationParametersInternal).ContainsKey(nameof(NavigationMode))) return default(NavigationMode);
            return (parameters as INavigationParametersInternal).GetValue<NavigationMode>(nameof(NavigationMode));
        }
        public static NavigationService GetNavigationService(this INavigationParameters parameters)
        {
            if (!(parameters as INavigationParametersInternal).ContainsKey(nameof(NavigationService))) return null;
            return (parameters as INavigationParametersInternal).GetValue<NavigationService>(nameof(NavigationService));
        }
        public static SynchronizationContext GetSynchronizationContext(this INavigationParameters parameters)
        {
            if (!(parameters as INavigationParametersInternal).ContainsKey(nameof(SynchronizationContext))) return null;
            return (parameters as INavigationParametersInternal).GetValue<SynchronizationContext>(nameof(SynchronizationContext));
        }

        internal static INavigationResult Successful(this FrameFacade result)
            => new NavigationResult { Success = true, Exception = null };

        internal static INavigationResult Failure(this FrameFacade result, Exception exception)
            => new NavigationResult { Success = false, Exception = exception };

        internal static INavigationResult Failure(this FrameFacade result, string message)
            => new NavigationResult { Success = false, Exception = new Exception(message) };
    }

}
