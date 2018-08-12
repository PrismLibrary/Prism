namespace Prism.Navigation
{
    public static class NavigationParametersExtensions
    {
        public static NavigationMode GetNavigationMode(this INavigationParameters parameters)
        {
            var internalParams = (INavigationParametersInternal)parameters;
            if (internalParams.ContainsKey(KnownInternalParameters.NavigationMode))
                return internalParams.GetValue<NavigationMode>(KnownInternalParameters.NavigationMode);

            throw new System.ArgumentNullException("NavigationMode is not available");
        }

        internal static INavigationParametersInternal GetNavigationParametersInternal(this INavigationParameters parameters)
        {
            return (INavigationParametersInternal)parameters;
        }
    }
}
