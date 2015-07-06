

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Prism.Events;
using Prism.Properties;
using Microsoft.Practices.ServiceLocation;
using Prism.Common;

namespace Prism.Regions
{
    /// <summary>
    /// Defines a registry for the content of the regions used on View Discovery composition.
    /// </summary>
    public class RegionViewRegistry : IRegionViewRegistry
    {
        private readonly IServiceLocator locator;
        private readonly ListDictionary<string, Func<object>> registeredContent = new ListDictionary<string, Func<object>>();
        private readonly WeakDelegatesManager contentRegisteredListeners = new WeakDelegatesManager();

        /// <summary>
        /// Creates a new instance of the <see cref="RegionViewRegistry"/> class.
        /// </summary>
        /// <param name="locator"><see cref="IServiceLocator"/> used to create the instance of the views from its <see cref="Type"/>.</param>
        public RegionViewRegistry(IServiceLocator locator)
        {
            this.locator = locator;
        }

        /// <summary>
        /// Occurs whenever a new view is registered.
        /// </summary>
        public event EventHandler<ViewRegisteredEventArgs> ContentRegistered
        {
            add { this.contentRegisteredListeners.AddListener(value); }
            remove { this.contentRegisteredListeners.RemoveListener(value); }
        }

        /// <summary>
        /// Returns the contents registered for a region.
        /// </summary>
        /// <param name="regionName">Name of the region which content is being requested.</param>
        /// <returns>Collection of contents registered for the region.</returns>
        public IEnumerable<object> GetContents(string regionName)
        {
            List<object> items = new List<object>();
            foreach (Func<object> getContentDelegate in this.registeredContent[regionName])
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
            this.RegisterViewWithRegion(regionName, () => this.CreateInstance(viewType));
        }

        /// <summary>
        /// Registers a delegate that can be used to retrieve the content associated with a region name. 
        /// </summary>
        /// <param name="regionName">Region name to which the <paramref name="getContentDelegate"/> will be registered.</param>
        /// <param name="getContentDelegate">Delegate used to retrieve the content associated with the <paramref name="regionName"/>.</param>
        public void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            this.registeredContent.Add(regionName, getContentDelegate);
            this.OnContentRegistered(new ViewRegisteredEventArgs(regionName, getContentDelegate));
        }

        /// <summary>
        /// Creates an instance of a registered view <see cref="Type"/>. 
        /// </summary>
        /// <param name="type">Type of the registered view.</param>
        /// <returns>Instance of the registered view.</returns>
        protected virtual object CreateInstance(Type type)
        {
            return this.locator.GetInstance(type);
        }

        private void OnContentRegistered(ViewRegisteredEventArgs e)
        {
            try
            {
                this.contentRegisteredListeners.Raise(this, e);
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