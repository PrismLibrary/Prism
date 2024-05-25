namespace Prism.Mvvm;

/// <summary>
/// Provides an abstraction layer for ViewRegistration that can be mocked
/// </summary>
public interface IViewRegistry
{
    /// <summary>
    /// The existing ViewRegistrations
    /// </summary>
    IEnumerable<ViewRegistration> Registrations { get; }

    /// <summary>
    /// Creates a view given a specified instance of the Container and a navigation name
    /// </summary>
    /// <param name="container"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    object CreateView(IContainerProvider container, string name);

    /// <summary>
    /// Gets the ViewType for a specified navigation name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Type GetViewType(string name);

    /// <summary>
    /// Gets the navigation name for a specified ViewModelType
    /// </summary>
    /// <param name="viewModelType"></param>
    /// <returns></returns>
    string GetViewModelNavigationKey(Type viewModelType);

    /// <summary>
    /// Gets the Registrations where the View is of a given base type
    /// </summary>
    /// <param name="baseType"></param>
    /// <returns></returns>
    IEnumerable<ViewRegistration> ViewsOfType(Type baseType);

    /// <summary>
    /// Confirms whether the given navigation name has been registered
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool IsRegistered(string name);
}
