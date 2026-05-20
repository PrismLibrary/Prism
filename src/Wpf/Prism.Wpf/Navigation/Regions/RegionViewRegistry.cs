using System.Globalization;
using System.Reflection;
using Prism.Common;
using Prism.Events;
using Prism.Properties;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Defines a registry for the content of the regions used on View Discovery composition.
    /// </summary>
    public class RegionViewRegistry : IRegionViewRegistry
    {
        private readonly IContainerProvider _container;
        private readonly ListDictionary<string, Func<IContainerProvider, object>> _registeredContent = new ListDictionary<string, Func<IContainerProvider, object>>();
        private readonly Dictionary<string, HashSet<Type>> _registeredViewTypes = new Dictionary<string, HashSet<Type>>();
        private readonly Dictionary<string, HashSet<string>> _registeredTargetNames = new Dictionary<string, HashSet<string>>();
        private readonly WeakDelegatesManager _contentRegisteredListeners = new WeakDelegatesManager();

        /// <summary>
        /// Creates a new instance of the <see cref="RegionViewRegistry"/> class.
        /// </summary>
        /// <param name="container"><see cref="IContainerExtension"/> used to create the instance of the views from its <see cref="Type"/>.</param>
        public RegionViewRegistry(IContainerExtension container)
        {
            _container = container;
        }

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
        /// <param name="regionName">Name of the region which content is being requested.</param>
        /// <param name="container">The <see cref="IContainerProvider"/> to use to get the View.</param>
        /// <returns>Collection of contents registered for the region.</returns>
        public IEnumerable<object> GetContents(string regionName, IContainerProvider container)
        {
            var items = new List<object>();
            foreach (Func<IContainerProvider, object> getContentDelegate in _registeredContent[regionName])
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
            if (!TryAddRegisteredViewType(regionName, viewType))
            {
                return;
            }

            RegisterViewWithRegion(regionName, _ => CreateInstance(viewType));
        }

        /// <summary>
        /// Registers a delegate that can be used to retrieve the content associated with a region name.
        /// </summary>
        /// <param name="regionName">Region name to which the <paramref name="getContentDelegate"/> will be registered.</param>
        /// <param name="getContentDelegate">Delegate used to retrieve the content associated with the <paramref name="regionName"/>.</param>
        public void RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> getContentDelegate)
        {
            if (IsContentDelegateRegistered(regionName, getContentDelegate))
            {
                return;
            }

            _registeredContent.Add(regionName, getContentDelegate);
            OnContentRegistered(new ViewRegisteredEventArgs(regionName, getContentDelegate));
        }

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="targetName">The type of the view to register with the </param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        public void RegisterViewWithRegion(string regionName, string targetName)
        {
            if (!TryAddRegisteredTargetName(regionName, targetName))
            {
                return;
            }

            RegisterViewWithRegion(regionName, c => c.Resolve<object>(targetName));
        }

        /// <summary>
        /// Creates an instance of a registered view <see cref="Type"/>.
        /// </summary>
        /// <param name="type">Type of the registered view.</param>
        /// <returns>Instance of the registered view.</returns>
        protected virtual object CreateInstance(Type type)
        {
            var view = _container.Resolve(type);
            MvvmHelpers.AutowireViewModel(view);
            return view;
        }

        private bool TryAddRegisteredViewType(string regionName, Type viewType)
        {
            if (!_registeredViewTypes.TryGetValue(regionName, out HashSet<Type> viewTypes))
            {
                viewTypes = new HashSet<Type>();
                _registeredViewTypes.Add(regionName, viewTypes);
            }

            return viewTypes.Add(viewType);
        }

        private bool TryAddRegisteredTargetName(string regionName, string targetName)
        {
            if (!_registeredTargetNames.TryGetValue(regionName, out HashSet<string> targetNames))
            {
                targetNames = new HashSet<string>();
                _registeredTargetNames.Add(regionName, targetNames);
            }

            return targetNames.Add(targetName);
        }

        private bool IsContentDelegateRegistered(string regionName, Func<IContainerProvider, object> getContentDelegate)
        {
            foreach (Func<IContainerProvider, object> existingDelegate in _registeredContent[regionName])
            {
                if (existingDelegate == getContentDelegate)
                {
                    return true;
                }
            }

            return false;
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
}
