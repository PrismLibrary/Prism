using System;

namespace Prism.Navigation
{
    internal static partial class FrameFacadeExtensions
    {
        internal static INavigationResult NavigationSuccess(this FrameFacade result)
            => new NavigationResult { Success = true, Exception = null };

        internal static INavigationResult NavigationFailure(this FrameFacade result, Exception exception)
            => new NavigationResult { Success = false, Exception = exception };

        internal static INavigationResult NavigationFailure(this FrameFacade result, string message)
            => new NavigationResult { Success = false, Exception = new Exception(message) };
    }

}
