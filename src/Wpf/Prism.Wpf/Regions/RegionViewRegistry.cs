using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Properties;

namespace Prism.Regions
{
    /// <summary>
    /// Defines a registry for the content of the regions used on View Discovery composition.
    /// </summary>
    public class RegionViewRegistry : IRegionViewRegistry
    {
        private readonly IContainerProvider _container;
        private readonly ListDictionary<string, Func<object>> _registeredContent = new ListDictionary<string, Func<object>>();
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
        /// <returns>Collection of contents registered for the region.</returns>
        public IEnumerable<object> GetContents(string regionName)
        {
            List<object> items = new List<object>();
            foreach (Func<object> getContentDelegate in _registeredContent[regionName])
            {
                items.Add(getContentDelegate());
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
            RegisterViewWithRegion(regionName, () => CreateInstance(viewType));
        }

        /// <summary>
        /// Registers a delegate that can be used to retrieve the content associated with a region name. 
        /// </summary>
        /// <param name="regionName">Region name to which the <paramref name="getContentDelegate"/> will be registered.</param>
        /// <param name="getContentDelegate">Delegate used to retrieve the content associated with the <paramref name="regionName"/>.</param>
        public void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            _registeredContent.Add(regionName, getContentDelegate);
            OnContentRegistered(new ViewRegisteredEventArgs(regionName, getContentDelegate));
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
