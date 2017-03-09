namespace Prism.Navigation
{
    public static class NavigationParametersExtensions
    {
        public static NavigationMode GetNavigationMode(this NavigationParameters parameters)
        {
            if (parameters.ContainsKey(KnownNavigationParameters.NavigationMode))
                return (NavigationMode)parameters[KnownNavigationParameters.NavigationMode];

            throw new System.ArgumentNullException("NavigationMode is not available");
        }
    }
}
