namespace Prism.Navigation;

/// <summary>
/// 
/// </summary>
public static class NavigationLifecycleAwareFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory"></param>
    public static void SetDefaultNavigationLifecycleFactory(Func<NavigationLifecycleAware> factory) => _factory = factory;
    internal static INavigationLifecycleAware NavigationLifecycleAware => _navigationLifeCycleAware ??= _factory.Invoke();
    
    private static Func<NavigationLifecycleAware> _factory;
    private static NavigationLifecycleAware _navigationLifeCycleAware;
}
