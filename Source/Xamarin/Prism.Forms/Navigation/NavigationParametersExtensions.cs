namespace Prism.Navigation
{
    public static class NavigationParametersExtensions
    {
        public static NavigationMode GetNavigationMode(this INavigationParameters parameters)
        {
            var internalParams = (IInternalNavigationParameters)parameters;
            if (internalParams.InternalParameters.ContainsKey(KnownInternalParameters.NavigationMode))
                return (NavigationMode)internalParams.InternalParameters[KnownInternalParameters.NavigationMode];

            throw new System.ArgumentNullException("NavigationMode is not available");
        }

        internal static IInternalNavigationParameters GetInternalNavigationParameters(this INavigationParameters parameters)
        {
            return (IInternalNavigationParameters)parameters;
        }
    }
}
