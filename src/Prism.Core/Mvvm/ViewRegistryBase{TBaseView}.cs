using System.Text.RegularExpressions;

#nullable enable
namespace Prism.Mvvm;

/// <summary>
/// Base class for registering and creating views based on a specified view type.
/// </summary>
/// <typeparam name="TBaseView">The base type of all view classes managed by this registry.</typeparam>
public abstract class ViewRegistryBase<TBaseView> : IViewRegistry
    where TBaseView : class
{
    private readonly IEnumerable<ViewRegistration> _registrations;
    private readonly ViewType _registryType;

    /// <summary>
    /// Initializes a new instance of the ViewRegistryBase class.
    /// </summary>
    /// <param name="registryType">The type of view this registry manages (Page, Region, or Dialog).</param>
    /// <param name="registrations">The collection of view registrations.</param>
    protected ViewRegistryBase(ViewType registryType, IEnumerable<ViewRegistration> registrations)
    {
        _registrations = registrations;
        _registryType = registryType;
    }

    /// <summary>
    /// Gets a read-only collection of registered views filtered by the current registry type.
    /// </summary>
    public IEnumerable<ViewRegistration> Registrations =>
        _registrations.Where(viewRegistration => viewRegistration.Type == _registryType);

    /// <summary>
    /// Gets the view type associated with the specified name, or null if not found.
    /// </summary>
    /// <param name="name">The name of the view to retrieve.</param>
    /// <returns>The type of the view, or null if not found.</returns>
    public Type? GetViewType(string name) =>
        GetRegistration(name)?.View;

    /// <summary>
    /// Creates an instance of the specified view using the provided container.
    /// </summary>
    /// <param name="container">The container used to resolve dependencies.</param>
    /// <param name="name">The name of the view to create.</param>
    /// <returns>An instance of the created view.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified view is not registered.</exception>
    /// <exception cref="ViewModelCreationException">Thrown if an error occurs while creating the view model.</exception>
    /// <exception cref="ViewCreationException">Thrown if an error occurs while creating the view.</exception>
    public object? CreateView(IContainerProvider container, string name)
    {
        try
        {
            var registration = GetRegistration(name) ?? throw new KeyNotFoundException($"No view with the name '{name}' has been registered");
            var view = container.Resolve(registration.View) as TBaseView;
            SetNavigationNameProperty(view, registration.Name);

            SetContainerProvider(view, container);
            ConfigureView(view, container);

            if (registration.ViewModel is not null)
                SetViewModelProperty(view, registration.ViewModel);

            Autowire(view);

            return view;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (ViewModelCreationException)
        {
            throw;
        }
        catch (Exception? ex)
        {
            throw new ViewCreationException(name, _registryType, ex);
        }
    }

    private IEnumerable<Type> GetCandidates(Type viewModelType)
    {
        var names = new List<string>
        {
            Regex.Replace(viewModelType.Name, @"ViewModel$", string.Empty),
            Regex.Replace(viewModelType.Name, @"Model$", string.Empty),
        };

        if (_registryType == ViewType.Page)
            names.Add(Regex.Replace(viewModelType.Name, @"ViewModel$", "Page"));
        else if (_registryType == ViewType.Region)
            names.Add(Regex.Replace(viewModelType.Name, @"ViewModel$", "Region"));
        else if (_registryType == ViewType.Dialog)
            names.Add(Regex.Replace(viewModelType.Name, @"ViewModel$", "Dialog"));

        names = names.Where(x => !x.EndsWith("PagePage")).ToList();

        if (viewModelType.Namespace != null)
        {
            string[] namespaces = _registryType switch
            {
                ViewType.Page =>
                [
                    viewModelType.Namespace.Replace("ViewModels", "Views"),
                    viewModelType.Namespace.Replace("ViewModels", "Pages")
                ],
                ViewType.Region =>
                [
                    viewModelType.Namespace.Replace("ViewModels", "Views"),
                    viewModelType.Namespace.Replace("ViewModels", "Regions")
                ],
                ViewType.Dialog =>
                [
                    viewModelType.Namespace.Replace("ViewModels", "Views"),
                    viewModelType.Namespace.Replace("ViewModels", "Dialogs")
                ],
                _ =>
                [
                    viewModelType.Namespace.Replace("ViewModels", "Views"),
                ]
            };

            var candidates = namespaces.Select(@namespace => names.Select(name => $"{@namespace}.{name}"))
                .SelectMany(x => x)
                .Select(x =>
                    viewModelType.AssemblyQualifiedName?.Replace(viewModelType.FullName ?? string.Empty, x) ??
                    string.Empty);

            return candidates
                .Select(x => Type.GetType(x, false))
                .Where(x => x is not null);
        }

        return [];
    }

    /// <summary>
    /// Gets the navigation key associated with the specified view model type, or throws an exception if not found.
    /// </summary>
    /// <param name="viewModelType">The type of the view model.</param>
    /// <returns>The navigation key for the view associated with the view model.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no view is registered for the specified view model.</exception>
    public string GetViewModelNavigationKey(Type viewModelType)
    {
        var registration = Registrations.LastOrDefault(x => x.ViewModel == viewModelType);
        if (registration is not null)
            return registration.Name;

        var candidates = GetCandidates(viewModelType);
        registration = Registrations.LastOrDefault(x => candidates.Any(c => c == x.View));
        if (registration is not null)
        {
            return registration.Name;
        }

        throw new KeyNotFoundException($"No View with the ViewModel '{viewModelType.Name}' has been registered");
    }

    /// <summary>
    /// Gets a collection of registered views that inherit from or implement the specified base type.
    /// </summary>
    /// <param name="baseType">The base type to filter by.</param>
    /// <returns>A collection of matching view registrations.</returns>
    public IEnumerable<ViewRegistration> ViewsOfType(Type baseType) =>
        Registrations.Where(viewRegistration => viewRegistration.View == baseType || baseType.IsAssignableFrom(viewRegistration.View));

    /// <summary>
    /// Checks if a view is registered with the specified name.
    /// </summary>
    /// <param name="name">The name of the view to check.</param>
    /// <returns>True if the view is registered, false otherwise.</returns>
    public bool IsRegistered(string name) =>
        GetRegistration(name) is not null;

    /// <summary>
    /// Gets the registration information for a view with the specified name, or null if not found.
    /// </summary>
    /// <param name="name">The name of the view to look up.</param>
    /// <returns>The view registration object, or null if not found.</returns>
    protected ViewRegistration? GetRegistration(string name) =>
        Registrations.LastOrDefault(viewRegistration => viewRegistration.Name == name);

    /// <summary>
    /// Allows subclasses to perform custom configuration on the created view instance.
    /// </summary>
    /// <param name="view">The created view instance.</param>
    /// <param name="container">The container used to resolve dependencies.</param>
    protected abstract void ConfigureView(TBaseView? view, IContainerProvider container);

    /// <summary>
    /// Calls the platform code to Autowire the View if it does not have a ViewModel already
    /// </summary>
    /// <param name="view"></param>
    protected abstract void Autowire(TBaseView? view);

    /// <summary>
    /// Sets the specified navigation name that was used to Navigate. This can be useful for back navigation
    /// </summary>
    /// <param name="view"></param>
    /// <param name="name"></param>
    protected abstract void SetNavigationNameProperty(TBaseView? view, string name);

    /// <summary>
    /// Sets the ViewModel Type to resolve
    /// </summary>
    /// <param name="view"></param>
    /// <param name="viewModelType"></param>
    protected abstract void SetViewModelProperty(TBaseView? view, Type viewModelType);

    /// <summary>
    /// Sets the IContainerProvider making it easier to access on the given View
    /// </summary>
    /// <param name="view"></param>
    /// <param name="container"></param>
    protected abstract void SetContainerProvider(TBaseView? view, IContainerProvider container);
}
