

using System;
using System.ComponentModel;
using System.Windows;

namespace Prism.Regions
{
    /// <summary>
    /// Defines a model that can be used to compose views.
    /// </summary>
    public interface IRegion : INavigateAsync, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a readonly view of the collection of views in the region.
        /// </summary>
        /// <value>An <see cref="IViewsCollection"/> of all the added views.</value>
        IViewsCollection Views { get; }

        /// <summary>
        /// Gets a readonly view of the collection of all the active views in the region.
        /// </summary>
        /// <value>An <see cref="IViewsCollection"/> of all the active views.</value>
        IViewsCollection ActiveViews { get; }

        /// <summary>
        /// Gets or sets a context for the region. This value can be used by the user to share context with the views.
        /// </summary>
        /// <value>The context value to be shared.</value>
        object Context { get; set; }

        /// <summary>
        /// Gets the name of the region that uniequely identifies the region within a <see cref="IRegionManager"/>.
        /// </summary>
        /// <value>The name of the region.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the comparison used to sort the views.
        /// </summary>
        /// <value>The comparison to use.</value>
        Comparison<object> SortComparison { get; set; }

        ///<overloads>Adds a new view to the region.</overloads>
        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view if it is a <see cref="DependencyObject"/>. It will be the current region manager when using this overload.</returns>
        IRegionManager Add(object view);

        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <param name="viewName">The name of the view. This can be used to retrieve it later by calling <see cref="GetView"/>.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view if it is a <see cref="DependencyObject"/>. It will be the current region manager when using this overload.</returns>
        IRegionManager Add(object view, string viewName);

        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <param name="viewName">The name of the view. This can be used to retrieve it later by calling <see cref="GetView"/>.</param>
        /// <param name="createRegionManagerScope">When <see langword="true"/>, the added view will receive a new instance of <see cref="IRegionManager"/>, otherwise it will use the current region manager for this region.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view if it is a <see cref="DependencyObject"/>.</returns>
        IRegionManager Add(object view, string viewName, bool createRegionManagerScope);

        /// <summary>
        /// Removes the specified view from the region.
        /// </summary>
        /// <param name="view">The view to remove.</param>
        void Remove(object view);

        /// <summary>
        /// Removes all views from the region.
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// Marks the specified view as active. 
        /// </summary>
        /// <param name="view">The view to activate.</param>
        void Activate(object view);

        /// <summary>
        /// Marks the specified view as inactive. 
        /// </summary>
        /// <param name="view">The view to deactivate.</param>
        void Deactivate(object view);

        /// <summary>
        /// Returns the view instance that was added to the region using a specific name.
        /// </summary>
        /// <param name="viewName">The name used when adding the view to the region.</param>
        /// <returns>Returns the named view or <see langword="null"/> if the view with <paramref name="viewName"/> does not exist in the current region.</returns>
        object GetView(string viewName);

        /// <summary>
        /// Gets or sets the <see cref="IRegionManager"/> that will be passed to the views when adding them to the region, unless the view is added by specifying createRegionManagerScope as <see langword="true" />.
        /// </summary>
        /// <value>The <see cref="IRegionManager"/> where this <see cref="IRegion"/> is registered.</value>
        /// <remarks>This is usually used by implementations of <see cref="IRegionManager"/> and should not be
        /// used by the developer explicitely.</remarks>
        IRegionManager RegionManager { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="IRegionBehavior"/>s that can extend the behavior of regions. 
        /// </summary>
        IRegionBehaviorCollection Behaviors { get; }

        /// <summary>
        /// Gets or sets the navigation service.
        /// </summary>
        /// <value>The navigation service.</value>
        IRegionNavigationService NavigationService { get; set; }
    }
}