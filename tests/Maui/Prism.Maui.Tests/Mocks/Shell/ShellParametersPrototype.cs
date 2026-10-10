using Prism.Navigation;
using NavigationMode = Prism.Navigation.NavigationMode;

namespace Prism.Maui.Tests.Mocks.Shell;

// Use Shell's single-use object transfer. Encoding belongs to the URI boundary;
// strings in an object dictionary must not be decoded or serialized implicitly.
internal static class ShellParametersPrototype
{
    public static ShellNavigationQueryParameters ToShellQuery(INavigationParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        var query = new ShellNavigationQueryParameters();
        foreach (var parameter in parameters)
        {
            if (!query.TryAdd(parameter.Key, parameter.Value))
                throw new ArgumentException($"Shell cannot transfer repeated parameter '{parameter.Key}'.", nameof(parameters));
        }

        return query;
    }

    public static INavigationParameters FromShellQuery(IDictionary<string, object> query, NavigationMode mode)
    {
        ArgumentNullException.ThrowIfNull(query);
        var parameters = new NavigationParameters();
        foreach (var parameter in query)
            parameters.Add(parameter.Key, parameter.Value);

        parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, mode);
        return parameters;
    }
}
