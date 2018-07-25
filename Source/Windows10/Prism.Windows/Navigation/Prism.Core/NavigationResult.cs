using Prism.Navigation;
using System;

namespace Prism.Navigation
{
    public class NavigationResult : INavigationResult
    {
        internal static INavigationResult Successful()
            => new NavigationResult(true, null);

        internal static INavigationResult Failure(Exception exception)
            => new NavigationResult(false, exception);

        internal static INavigationResult Failure(string message)
            => new NavigationResult(false, new Exception(message));

        NavigationResult(bool success, Exception exception)
        {
            Success = success;
            Exception = exception;
        }

        public bool Success { get; }

        public Exception Exception { get; }

        public static implicit operator bool(NavigationResult result)
        {
            return result.Success;
        }
    }
}
