namespace Prism.Navigation
{
    public static class NavigationParametersExtensions
    {
        public static NavigationMode GetNavigationMode(this NavigationParameters parameters)
        {
            object navigationMode;
            if (parameters.TryGetValue(KnownNavigationParameters.NavigationMode, out navigationMode))
                return (NavigationMode)navigationMode;

            throw new System.ArgumentNullException("NavigationMode is not available");
        }
    }
}
