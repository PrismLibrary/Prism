using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Prism.Ioc;

namespace Prism.Mvvm;

public abstract class ViewRegistryBase<TBaseView> : IViewRegistry
    where TBaseView : class
{
    private readonly IEnumerable<ViewRegistration> _registrations;
    private readonly ViewType _registryType;

    protected ViewRegistryBase(ViewType registryType, IEnumerable<ViewRegistration> registrations)
    {
        _registrations = registrations;
        _registryType = registryType;
    }

    public IEnumerable<ViewRegistration> Registrations => 
        _registrations.Where(x => x.Type == _registryType);

    public Type GetViewType(string name) =>
        GetRegistration(name)?.View;

    public object CreateView(IContainerProvider container, string name)
    {
        try
        {
            var registration = GetRegistration(name);
            if (registration is null)
                throw new KeyNotFoundException($"No view with the name '{name}' has been registered");

            var view = container.Resolve(registration.View) as TBaseView;
            SetNavigationNameProperty(view, registration.Name);
            //;

            //;
            SetContainerProvider(view, container);
            ConfigureView(view, container);

            if (registration.ViewModel is not null)
                SetViewModelProperty(view, registration.ViewModel);
                //

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
        catch (Exception ex)
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

        var namespaces = _registryType switch
        {
            ViewType.Page => new[]
            {
                viewModelType.Namespace.Replace("ViewModels", "Views"),
                viewModelType.Namespace.Replace("ViewModels", "Pages")
            },
            ViewType.Region => new[]
            {
                viewModelType.Namespace.Replace("ViewModels", "Views"),
                viewModelType.Namespace.Replace("ViewModels", "Regions")
            },
            ViewType.Dialog => new[]
            {
                viewModelType.Namespace.Replace("ViewModels", "Views"),
                viewModelType.Namespace.Replace("ViewModels", "Dialogs")
            },
            _ => new[]
            {
                viewModelType.Namespace.Replace("ViewModels", "Views"),
            }
        };

        var candidates = namespaces.Select(@namespace => names.Select(name => $"{@namespace}.{name}"))
            .SelectMany(x => x)
            .Select(x => viewModelType.AssemblyQualifiedName.Replace(viewModelType.FullName, x));
        return candidates
            .Select(x => Type.GetType(x, false))
            .Where(x => x is not null);
    }

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

    public IEnumerable<ViewRegistration> ViewsOfType(Type baseType) =>
        Registrations.Where(x => x.View == baseType || baseType.IsAssignableFrom(x.View));

    public bool IsRegistered(string name) =>
        GetRegistration(name) is not null;

    protected ViewRegistration GetRegistration(string name) =>
        Registrations.LastOrDefault(x => x.Name == name);

    protected abstract void ConfigureView(TBaseView view, IContainerProvider container);

    /// <summary>
    /// Calls the platform code to Autowire the View if it does not have a ViewModel already
    /// </summary>
    /// <param name="view"></param>
    protected abstract void Autowire(TBaseView view);

    /// <summary>
    /// Sets the specified navigation name that was used to Navigate. This can be useful for back navigation
    /// </summary>
    /// <param name="view"></param>
    /// <param name="name"></param>
    protected abstract void SetNavigationNameProperty(TBaseView view, string name);

    /// <summary>
    /// Sets the ViewModel Type to resolve
    /// </summary>
    /// <param name="view"></param>
    /// <param name="viewModelType"></param>
    protected abstract void SetViewModelProperty(TBaseView view, Type viewModelType);

    /// <summary>
    /// Sets the IContainerProvider making it easier to access on the given View
    /// </summary>
    /// <param name="view"></param>
    /// <param name="container"></param>
    protected abstract void SetContainerProvider(TBaseView view, IContainerProvider container);

    //public static Type GetPageType(string name)
    //{
    //    var registrations = _registrations.Where(x => x.Name == name);
    //    if (!registrations.Any())
    //        throw new KeyNotFoundException(name);
    //    if (registrations.Count() > 1)
    //        throw new InvalidOperationException(string.Format(Resources.MultipleViewsRegisteredForNavigationKey, name, string.Join(", ", registrations.Select(x => x.View.FullName))));

    //    return registrations.First().View;
    //}
}
