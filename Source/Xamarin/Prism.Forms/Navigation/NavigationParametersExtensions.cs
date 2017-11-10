namespace Prism.Navigation
{
    public static class NavigationParametersExtensions
    {
        public static NavigationMode GetNavigationMode(this NavigationParameters parameters)
        {
            if (parameters.InternalParameters.ContainsKey(KnownInternalParameters.NavigationMode))
                return (NavigationMode)parameters.InternalParameters[KnownInternalParameters.NavigationMode];

            throw new System.ArgumentNullException("NavigationMode is not available");
        }
    }
}
