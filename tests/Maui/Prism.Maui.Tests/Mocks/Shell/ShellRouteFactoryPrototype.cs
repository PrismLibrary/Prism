using Prism.Common;
using Prism.Navigation;
using Prism.Navigation.Xaml;

namespace Prism.Maui.Tests.Mocks.Shell;

// Construction probe only. A Shell coordinator must still own initialization,
// confirmation, committed navigation callbacks and removal of page scopes.
internal sealed class ShellRouteFactoryPrototype : RouteFactory
{
    private readonly string _navigationKey;

    public ShellRouteFactoryPrototype(string navigationKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(navigationKey);
        _navigationKey = navigationKey;
    }

    public override Element GetOrCreate()
    {
        throw new InvalidOperationException("Shell route construction requires the owning host's services.");
    }

    public override Element GetOrCreate(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        var container = services.GetService(typeof(IContainerProvider)) as IContainerProvider
            ?? throw new InvalidOperationException("Shell services must provide a Prism container.");
        var registry = container.Resolve<INavigationRegistry>();
        var viewType = registry.GetViewType(_navigationKey)
            ?? throw new KeyNotFoundException($"No Prism page is registered for '{_navigationKey}'.");

        // Shell owns its own navigation and tab containers.
        if (!typeof(ContentPage).IsAssignableFrom(viewType))
            throw new NotSupportedException("The Shell construction probe supports ContentPage routes only.");

        var scope = container.CreateScope();
        IPageAccessor accessor = null;
        try
        {
            accessor = scope.Resolve<IPageAccessor>();
            return (ContentPage)registry.CreateView(scope, _navigationKey);
        }
        catch (Exception constructionError)
        {
            try
            {
                if (accessor?.Page is { } page && ReferenceEquals(page.GetContainerProvider(), scope))
                    page.SetContainerProvider(null);
                else
                    scope.Dispose();
            }
            catch (Exception cleanupError)
            {
                // Keep the construction failure visible even if disposal also fails.
                constructionError.Data["ShellScopeCleanupException"] = cleanupError;
            }

            throw;
        }
    }
}
