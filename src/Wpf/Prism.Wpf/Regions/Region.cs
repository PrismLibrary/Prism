

using CommonServiceLocator;
using Prism.Properties;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Prism.Regions
{
    /// <summary>
    /// Implementation of <see cref="IRegion"/> that allows multiple active views.
    /// </summary>
    public class Region : IRegion
    {
        private ObservableCollection<ItemMetadata> itemMetadataCollection;
        private string name;
        private ViewsCollection views;
        private ViewsCollection activeViews;
        private object context;
        private IRegionManager regionManager;
        private IRegionNavigationService regionNavigationService;

        private Comparison<object> sort;

        /// <summary>
        /// Initializes a new instance of <see cref="Region"/>.
        /// </summary>
        public Region()
        {
            this.Behaviors = new RegionBehaviorCollection(this);

            this.sort = Region.DefaultSortComparison;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the collection of <see cref="IRegionBehavior"/>s that can extend the behavior of regions.
        /// </summary>
        public IRegionBehaviorCollection Behaviors { get; private set; }

        /// <summary>
        /// Gets or sets a context for the region. This value can be used by the user to share context with the views.
        /// </summary>
        /// <value>The context value to be shared.</value>
        public object Context
        {
            get
            {
                return this.context;
            }

            set
            {
                if (this.context != value)
                {
                    this.context = value;
                    this.OnPropertyChanged("Context");
                }
            }
        }

        /// <summary>
        /// Gets the name of the region that uniequely identifies the region within a <see cref="IRegionManager"/>.
        /// </summary>
        /// <value>The name of the region.</value>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name != null && this.name != value)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CannotChangeRegionNameException, this.name));
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.RegionNameCannotBeEmptyException);
                }

                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets a readonly view of the collection of views in the region.
        /// </summary>
        /// <value>An <see cref="IViewsCollection"/> of all the added views.</value>
        public virtual IViewsCollection Views
        {
            get
            {
                if (this.views == null)
                {
                    this.views = new ViewsCollection(ItemMetadataCollection, x => true);
                    this.views.SortComparison = this.sort;
                }

                return this.views;
            }
        }

        /// <summary>
        /// Gets a readonly view of the collection of all the active views in the region.
        /// </summary>
        /// <value>An <see cref="IViewsCollection"/> of all the active views.</value>
        public virtual IViewsCollection ActiveViews
        {
            get
            {
                if (this.views == null)
                {
                    this.views = new ViewsCollection(ItemMetadataCollection, x => true);
                    this.views.SortComparison = this.sort;
                }

                if (this.activeViews == null)
                {
                    this.activeViews = new ViewsCollection(ItemMetadataCollection, x => x.IsActive);
                    this.activeViews.SortComparison = this.sort;
                }

                return this.activeViews;
            }
        }

        /// <summary>
        /// Gets or sets the comparison used to sort the views.
        /// </summary>
        /// <value>The comparison to use.</value>
        public Comparison<object> SortComparison
        {
            get
            {
                return this.sort;
            }
            set
            {
                this.sort = value;

                if (this.activeViews != null)
                {
                    this.activeViews.SortComparison = this.sort;
                }

                if (this.views != null)
                {
                    this.views.SortComparison = this.sort;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IRegionManager"/> that will be passed to the views when adding them to the region, unless the view is added by specifying createRegionManagerScope as <see langword="true" />.
        /// </summary>
        /// <value>The <see cref="IRegionManager"/> where this <see cref="IRegion"/> is registered.</value>
        /// <remarks>This is usually used by implementations of <see cref="IRegionManager"/> and should not be
        /// used by the developer explicitely.</remarks>
        public IRegionManager RegionManager
        {
            get
            {
                return this.regionManager;
            }

            set
            {
                if (this.regionManager != value)
                {
                    this.regionManager = value;
                    this.OnPropertyChanged("RegionManager");
                }
            }
        }

        /// <summary>
        /// Gets the navigation service.
        /// </summary>
        /// <value>The navigation service.</value>
        public IRegionNavigationService NavigationService
        {
            get
            {
                if (this.regionNavigationService == null)
                {
                    this.regionNavigationService = ServiceLocator.Current.GetInstance<IRegionNavigationService>();
                    this.regionNavigationService.Region = this;
                }

                return this.regionNavigationService;
            }

            set
            {
                this.regionNavigationService = value;
            }
        }

        /// <summary>
        /// Gets the collection with all the views along with their metadata.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}"/> of <see cref="ItemMetadata"/> with all the added views.</value>
        protected virtual ObservableCollection<ItemMetadata> ItemMetadataCollection
        {
            get
            {
                if (this.itemMetadataCollection == null)
                {
                    this.itemMetadataCollection = new ObservableCollection<ItemMetadata>();
                }

                return this.itemMetadataCollection;
            }
        }

        /// <overloads>Adds a new view to the region.</overloads>
        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view if it is a <see cref="DependencyObject"/>. It will be the current region manager when using this overload.</returns>
        public IRegionManager Add(object view)
        {
            return this.Add(view, null, false);
        }

        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <param name="viewName">The name of the view. This can be used to retrieve it later by calling <see cref="IRegion.GetView"/>.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view if it is a <see cref="DependencyObject"/>. It will be the current region manager when using this overload.</returns>
        public IRegionManager Add(object view, string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, "viewName"));
            }

            return this.Add(view, viewName, false);
        }

        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <param name="viewName">The name of the view. This can be used to retrieve it later by calling <see cref="IRegion.GetView"/>.</param>
        /// <param name="createRegionManagerScope">When <see langword="true"/>, the added view will receive a new instance of <see cref="IRegionManager"/>, otherwise it will use the current region manager for this region.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view if it is a <see cref="DependencyObject"/>.</returns>
        public virtual IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            IRegionManager manager = createRegionManagerScope ? this.RegionManager.CreateRegionManager() : this.RegionManager;
            this.InnerAdd(view, viewName, manager);
            return manager;
        }

        /// <summary>
        /// Removes the specified view from the region.
        /// </summary>
        /// <param name="view">The view to remove.</param>
        public virtual void Remove(object view)
        {
            ItemMetadata itemMetadata = this.GetItemMetadataOrThrow(view);

            this.ItemMetadataCollection.Remove(itemMetadata);

            DependencyObject dependencyObject = view as DependencyObject;
            if (dependencyObject != null && Regions.RegionManager.GetRegionManager(dependencyObject) == this.RegionManager)
            {
                dependencyObject.ClearValue(Regions.RegionManager.RegionManagerProperty);
            }
        }

        /// <summary>
        /// Removes all views from the region.
        /// </summary>
        public void RemoveAll()
        {
            foreach (var view in Views)
            {
                Remove(view);
            }
        }

        /// <summary>
        /// Marks the specified view as active.
        /// </summary>
        /// <param name="view">The view to activate.</param>
        public virtual void Activate(object view)
        {
            ItemMetadata itemMetadata = this.GetItemMetadataOrThrow(view);

            if (!itemMetadata.IsActive)
            {
                itemMetadata.IsActive = true;
            }
        }

        /// <summary>
        /// Marks the specified view as inactive.
        /// </summary>
        /// <param name="view">The view to deactivate.</param>
        public virtual void Deactivate(object view)
        {
            ItemMetadata itemMetadata = this.GetItemMetadataOrThrow(view);

            if (itemMetadata.IsActive)
            {
                itemMetadata.IsActive = false;
            }
        }

        /// <summary>
        /// Returns the view instance that was added to the region using a specific name.
        /// </summary>
        /// <param name="viewName">The name used when adding the view to the region.</param>
        /// <returns>Returns the named view or <see langword="null"/> if the view with <paramref name="viewName"/> does not exist in the current region.</returns>
        public virtual object GetView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, "viewName"));
            }

            ItemMetadata metadata = this.ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName);

            if (metadata != null)
            {
                return metadata.Item;
            }

            return null;
        }

        /// <summary>
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="navigationCallback">A callback to execute when the navigation request is completed.</param>
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            this.RequestNavigate(target, navigationCallback, null);
        }

        /// <summary>
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="navigationCallback">A callback to execute when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            this.NavigationService.RequestNavigate(target, navigationCallback, navigationParameters);
        }

        private void InnerAdd(object view, string viewName, IRegionManager scopedRegionManager)
        {
            if (this.ItemMetadataCollection.FirstOrDefault(x => x.Item == view) != null)
            {
                throw new InvalidOperationException(Resources.RegionViewExistsException);
            }

            ItemMetadata itemMetadata = new ItemMetadata(view);
            if (!string.IsNullOrEmpty(viewName))
            {
                if (this.ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName) != null)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Resources.RegionViewNameExistsException, viewName));
                }
                itemMetadata.Name = viewName;
            }

            DependencyObject dependencyObject = view as DependencyObject;

            if (dependencyObject != null)
            {
                Regions.RegionManager.SetRegionManager(dependencyObject, scopedRegionManager);
            }

            this.ItemMetadataCollection.Add(itemMetadata);
        }

        private ItemMetadata GetItemMetadataOrThrow(object view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            ItemMetadata itemMetadata = this.ItemMetadataCollection.FirstOrDefault(x => x.Item == view);

            if (itemMetadata == null)
                throw new ArgumentException(Resources.ViewNotInRegionException, nameof(view));

            return itemMetadata;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The default sort algorithm.
        /// </summary>
        /// <param name="x">The first view to compare.</param>
        /// <param name="y">The second view to compare.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public static int DefaultSortComparison(object x, object y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    Type xType = x.GetType();
                    Type yType = y.GetType();

                    ViewSortHintAttribute xAttribute = xType.GetCustomAttributes(typeof(ViewSortHintAttribute), true).FirstOrDefault() as ViewSortHintAttribute;
                    ViewSortHintAttribute yAttribute = yType.GetCustomAttributes(typeof(ViewSortHintAttribute), true).FirstOrDefault() as ViewSortHintAttribute;

                    return ViewSortHintAttributeComparison(xAttribute, yAttribute);
                }
            }
        }

        private static int ViewSortHintAttributeComparison(ViewSortHintAttribute x, ViewSortHintAttribute y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    return string.Compare(x.Hint, y.Hint, StringComparison.Ordinal);
                }
            }
        }
    }
}