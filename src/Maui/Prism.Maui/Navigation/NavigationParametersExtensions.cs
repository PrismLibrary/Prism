using Prism.Properties;

namespace Prism.Navigation;

/// <summary>Provides access to navigation metadata carried by Prism navigation parameters.</summary>
public static class NavigationParametersExtensions
{
    /// <summary>Gets whether the current navigation is a new navigation or a back navigation.</summary>
    /// <param name="parameters">The Prism parameters supplied to a navigation callback.</param>
    /// <returns>The navigation mode recorded by the navigation service.</returns>
    /// <exception cref="ArgumentNullException">No navigation mode has been recorded in the parameters.</exception>
    /// <remarks>The mode is internal navigation metadata; adding a user parameter with the same name does not set it.</remarks>
    public static NavigationMode GetNavigationMode(this INavigationParameters parameters)
    {
        var internalParams = (INavigationParametersInternal)parameters;
        if (internalParams.ContainsKey(KnownInternalParameters.NavigationMode))
            return internalParams.GetValue<NavigationMode>(KnownInternalParameters.NavigationMode);

        throw new System.ArgumentNullException(Resources.NavigationModeNotAvailable);
    }

    internal static INavigationParametersInternal GetNavigationParametersInternal(this INavigationParameters parameters)
    {
        return (INavigationParametersInternal)parameters;
    }
}
