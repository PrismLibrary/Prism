using System;
using System.ComponentModel;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Defines a model that can be used to compose views.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IRegion"/> is a key component of Prism's Region-based composition pattern. Regions act as named placeholders
    /// where views can be dynamically added, removed, or swapped at runtime. This allows for flexible UI composition without
    /// tightly coupling different parts of an application.
    /// </para>
    /// <para>
    /// Views in a region can be managed individually or as a group. Only one view can be "Active" at a time (in most region adapters),
    /// allowing for tab-like or carousel-like UI patterns. Each region has a <see cref="Context"/> that can be used to share data
    /// between the region and its views.
    /// </para>
    /// </remarks>
    public interface IRegion : INavigateAsync, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a readonly view of the collection of views in the region.
        /// </summary>
        /// <value>An <see cref="IViewsCollection"/> of all the added views.</value>
        /// <remarks>
        /// This collection includes all views that have been added to the region, regardless of whether they are currently active.
        /// </remarks>
        IViewsCollection Views { get; }

        /// <summary>
        /// Gets a readonly view of the collection of all the active views in the region.
        /// </summary>
        /// <value>An <see cref="IViewsCollection"/> of all the active views.</value>
        /// <remarks>
        /// In most region adapters, this collection will contain 0 or 1 items, as only one view is typically active at a time.
        /// </remarks>
        IViewsCollection ActiveViews { get; }

        /// <summary>
        /// Gets or sets a context for the region. This value can be used by the user to share context with the views.
        /// </summary>
        /// <value>The context value to be shared.</value>
        /// <remarks>
        /// The context is typically used to pass data to views or to allow views to communicate back through the region.
        /// </remarks>
        object Context { get; set; }

        /// <summary>
        /// Gets the name of the region that uniquely identifies the region within a <see cref="IRegionManager"/>.
        /// </summary>
        /// <value>The name of the region.</value>
        /// <remarks>
        /// Region names are case-sensitive and must be unique within a region manager.
        /// </remarks>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the comparison used to sort the views.
        /// </summary>
        /// <value>The comparison to use.</value>
        /// <remarks>
        /// Setting this property allows you to control the order in which views appear in the Views collection.
        /// </remarks>
        Comparison<object> SortComparison { get; set; }

        ///<overloads>Adds a new view to the region.</overloads>
        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="viewName">The view to add.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view. It will be the current region manager when using this overload.</returns>
        /// <remarks>
        /// The view is resolved from the container using the provided name.
        /// </remarks>
        IRegionManager Add(string viewName);

        ///<overloads>Adds a new view to the region.</overloads>
        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view. It will be the current region manager when using this overload.</returns>
        /// <remarks>
        /// The view instance is directly added to the region without resolving from the container.
        /// </remarks>
        IRegionManager Add(object view);

        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <param name="viewName">The name of the view. This can be used to retrieve it later by calling <see cref="GetView"/>.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view. It will be the current region manager when using this overload.</returns>
        /// <remarks>
        /// The view is associated with the specified name and can be retrieved later using GetView(viewName).
        /// </remarks>
        IRegionManager Add(object view, string viewName);

        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <param name="viewName">The name of the view. This can be used to retrieve it later by calling <see cref="GetView"/>.</param>
        /// <param name="createRegionManagerScope">When <see langword="true"/>, the added view will receive a new instance of <see cref="IRegionManager"/>, otherwise it will use the current region manager for this region.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view.</returns>
        /// <remarks>
        /// Creating a new scope is useful when you want the view and its child regions to have a hierarchical region management structure.
        /// </remarks>
        IRegionManager Add(object view, string viewName, bool createRegionManagerScope);

        /// <summary>
        /// Removes the specified view from the region.
        /// </summary>
        /// <param name="view">The view to remove.</param>
        /// <remarks>
        /// If the view is currently active, it will be deactivated before removal.
        /// </remarks>
        void Remove(object view);

        /// <summary>
        /// Removes all views from the region.
        /// </summary>
        /// <remarks>
        /// This clears the region of all views. Active views are deactivated before removal.
        /// </remarks>
        void RemoveAll();

        /// <summary>
        /// Marks the specified view as active.
        /// </summary>
        /// <param name="view">The view to activate.</param>
        /// <remarks>
        /// In most region adapters, only one view can be active at a time. Activating a new view deactivates the previously active view.
        /// </remarks>
        void Activate(object view);

        /// <summary>
        /// Marks the specified view as inactive.
        /// </summary>
        /// <param name="view">The view to deactivate.</param>
        /// <remarks>
        /// Deactivating a view removes it from the ActiveViews collection.
        /// </remarks>
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
        /// used by the developer explicitly.</remarks>
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
