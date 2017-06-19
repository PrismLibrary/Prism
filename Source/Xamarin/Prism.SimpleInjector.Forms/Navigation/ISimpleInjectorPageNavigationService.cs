using Prism.Navigation;

namespace Prism.SimpleInjector.Navigation
{
    /// <summary>
    /// Marker interface so we can retrieve from the Container by interface, as named instances are not supported by SimpleInjector.
    /// </summary>
    internal interface ISimpleInjectorPageNavigationService : INavigationService { }
}