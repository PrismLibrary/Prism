using System.Globalization;
using System.Reflection;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Properties;

namespace Prism.Navigation.Regions;

/// <summary>
/// Defines a registry for the content of the regions used on View Discovery composition.
/// </summary>
public class RegionViewRegistry : IRegionViewRegistry
{
    private readonly ListDictionary<string, Func<IContainerProvider, object>> _registeredContent = new();
    private readonly WeakDelegatesManager _contentRegisteredListeners = new();

    /// <summary>
    /// Occurs whenever a new view is registered.
    /// </summary>
    public event EventHandler<ViewRegisteredEventArgs> ContentRegistered
    {
        add => _contentRegisteredListeners.AddListener(value);
        remove => _contentRegisteredListeners.RemoveListener(value);
    }

    /// <summary>
    /// Returns the contents registered for a region.
    /// </summary>
    /// <param name="container">The <see cref="IContainerProvider"/> to use.</param>
    /// <param name="regionName">Name of the region which content is being requested.</param>
    /// <returns>Collection of contents registered for the region.</returns>
    public IEnumerable<object> GetContents(string regionName, IContainerProvider container)
    {
        var items = new List<object>();
        foreach (var getContentDelegate in _registeredContent[regionName])
        {
            items.Add(getContentDelegate(container));
        }

        return items;
    }

    /// <summary>
    /// Registers a content type with a region name.
    /// </summary>
    /// <param name="regionName">Region name to which the <paramref name="viewType"/> will be registered.</param>
    /// <param name="viewType">Content type to be registered for the <paramref name="regionName"/>.</param>
    public void RegisterViewWithRegion(string regionName, Type viewType)
    {
        RegisterViewWithRegion(regionName, c =>
        {
            var registry = c.Resolve<IRegionNavigationRegistry>();
            var registration = registry.Registrations.FirstOrDefault(x => x.Type == ViewType.Region && x.View == viewType);
            if (registration is null)
                throw new KeyNotFoundException($"No registration found for the Region View '{viewType.FullName}'.");

            return registry.CreateView(c, registration.Name);
        });
    }

    /// <summary>
    /// Registers a content type with a region name.
    /// </summary>
    /// <param name="regionName">Region name to which the <paramref name="targetName"/> will be registered.</param>
    /// <param name="targetName">Content type to be registered for the <paramref name="regionName"/>.</param>
    public void RegisterViewWithRegion(string regionName, string targetName)
    {
        RegisterViewWithRegion(regionName, c =>
        {
            var registry = c.Resolve<IRegionNavigationRegistry>();
            return registry.CreateView(c, targetName);
        });
    }

    /// <summary>
    /// Registers a delegate that can be used to retrieve the content associated with a region name.
    /// </summary>
    /// <param name="regionName">Region name to which the <paramref name="getContentDelegate"/> will be registered.</param>
    /// <param name="getContentDelegate">Delegate used to retrieve the content associated with the <paramref name="regionName"/>.</param>
    public void RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> getContentDelegate)
    {
        _registeredContent.Add(regionName, getContentDelegate);
        OnContentRegistered(new ViewRegisteredEventArgs(regionName, getContentDelegate));
    }

    private void OnContentRegistered(ViewRegisteredEventArgs e)
    {
        try
        {
            _contentRegisteredListeners.Raise(this, e);
        }
        catch (TargetInvocationException ex)
        {
            Exception rootException;
            if (ex.InnerException != null)
            {
                rootException = ex.InnerException.GetRootException();
            }
            else
            {
                rootException = ex.GetRootException();
            }

            throw new ViewRegistrationException(string.Format(CultureInfo.CurrentCulture,
                Resources.OnViewRegisteredException, e.RegionName, rootException), ex.InnerException);
        }
    }
}
