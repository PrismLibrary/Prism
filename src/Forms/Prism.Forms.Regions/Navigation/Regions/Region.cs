using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Properties;
using Xamarin.Forms;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Implementation of <see cref="IRegion"/> that allows multiple active views.
    /// </summary>
    public class Region : BindableBase, IRegion, INavigationServiceAware
    {
        private ObservableCollection<ItemMetadata> _itemMetadataCollection;
        private IRegionManager _regionManager;
        private IRegionNavigationService _regionNavigationService;

        private Comparison<object> _sort;

        /// <summary>
        /// Initializes a new instance of <see cref="Region"/>.
        /// </summary>
        public Region()
        {
            Behaviors = new RegionBehaviorCollection(this);

            _sort = DefaultSortComparison;
        }

        private WeakReference<INavigationService> _weakNavigationService;
        INavigationService INavigationServiceAware.NavigationService
        {
            get => _weakNavigationService.TryGetTarget(out var target) ? target : null;
            set => _weakNavigationService = new WeakReference<INavigationService>(value);
        }

        private ViewsCollection _views;
        /// <summary>
        /// Gets a readonly view of the collection of views in the region.
        /// </summary>
        /// <value>An <see cref="IViewsCollection"/> of all the added views.</value>
        public virtual IViewsCollection Views
        {
            get
            {
                if (_views == null)
                {
                    _views = new ViewsCollection(ItemMetadataCollection, x => true)
                    {
                        SortComparison = _sort
                    };
                }

                return _views;
            }
        }

        private ViewsCollection _activeViews;
        /// <summary>
        /// Gets a readonly view of the collection of all the active views in the region.
        /// </summary>
        /// <value>An <see cref="IViewsCollection"/> of all the active views.</value>
        public virtual IViewsCollection ActiveViews
        {
            get
            {
                if (_views == null)
                {
                    _views = new ViewsCollection(ItemMetadataCollection, x => true)
                    {
                        SortComparison = _sort
                    };
                }

                if (_activeViews == null)
                {
                    _activeViews = new ViewsCollection(ItemMetadataCollection, x => x.IsActive)
                    {
                        SortComparison = _sort
                    };
                }

                return _activeViews;
            }
        }

        private object _context;
        /// <summary>
        /// Gets or sets a context for the region. This value can be used by the user to share context with the views.
        /// </summary>
        /// <value>The context value to be shared.</value>
        public object Context
        {
            get => _context;
            set => SetProperty(ref _context, value);
        }

        private string _name;
        /// <summary>
        /// Gets the name of the region that uniquely identifies the region within a <see cref="IRegionManager"/>.
        /// </summary>
        /// <value>The name of the region.</value>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != null && _name != value)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CannotChangeRegionNameException, _name));
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.RegionNameCannotBeEmptyException);
                }

                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Gets or sets the comparison used to sort the views.
        /// </summary>
        /// <value>The comparison to use.</value>
        public Comparison<object> SortComparison
        {
            get => _sort;
            set
            {
                _sort = value;

                if (_activeViews != null)
                {
                    _activeViews.SortComparison = _sort;
                }

                if (_views != null)
                {
                    _views.SortComparison = _sort;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IRegionManager"/> that will be passed to the views when adding them to the region, unless the view is added by specifying createRegionManagerScope as <see langword="true" />.
        /// </summary>
        /// <value>The <see cref="IRegionManager"/> where this <see cref="IRegion"/> is registered.</value>
        /// <remarks>This is usually used by implementations of <see cref="IRegionManager"/> and should not be
        /// used by the developer explicitly.</remarks>
        public IRegionManager RegionManager
        {
            get => _regionManager;
            set => SetProperty(ref _regionManager, value);
        }

        /// <summary>
        /// Gets the collection of <see cref="IRegionBehavior"/>s that can extend the behavior of regions.
        /// </summary>
        public IRegionBehaviorCollection Behaviors { get; }

        /// <summary>
        /// Gets the navigation service.
        /// </summary>
        /// <value>The navigation service.</value>
        public IRegionNavigationService NavigationService
        {
            get
            {
                if (_regionNavigationService == null)
                {
                    _regionNavigationService = ContainerLocator.Container.Resolve<IRegionNavigationService>();
                    _regionNavigationService.Region = this;
                }

                return _regionNavigationService;
            }

            set => _regionNavigationService = value;
        }

        /// <summary>
        /// Gets the collection with all the views along with their metadata.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}"/> of <see cref="ItemMetadata"/> with all the added views.</value>
        protected virtual ObservableCollection<ItemMetadata> ItemMetadataCollection
        {
            get
            {
                if (_itemMetadataCollection == null)
                {
                    _itemMetadataCollection = new ObservableCollection<ItemMetadata>();
                }

                return _itemMetadataCollection;
            }
        }

        /// <summary>
        /// Marks the specified view as active.
        /// </summary>
        /// <param name="view">The view to activate.</param>
        public virtual void Activate(object view)
        {
            var itemMetadata = GetItemMetadataOrThrow(view);

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
            var itemMetadata = GetItemMetadataOrThrow(view);

            if (itemMetadata.IsActive)
            {
                itemMetadata.IsActive = false;
            }
        }

        /// <summary>
        /// Add a View by name.
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        /// <remarks>Xamarin.Forms is in maintenance mode. This feature is not being supported.</remarks>
        /// <exception cref="NotImplementedException"></exception>
        public IRegionManager Add(string viewName) => throw new NotImplementedException();

        /// <overloads>Adds a new view to the region.</overloads>
        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view if it is a <see cref="VisualElement"/>. It will be the current region manager when using this overload.</returns>
        public IRegionManager Add(object view)
        {
            return Add(view, null, false);
        }

        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <param name="viewName">The name of the view. This can be used to retrieve it later by calling <see cref="IRegion.GetView"/>.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view if it is a <see cref="VisualElement"/>. It will be the current region manager when using this overload.</returns>
        public IRegionManager Add(object view, string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, nameof(viewName)));
            }

            return Add(view, viewName, false);
        }

        /// <summary>
        /// Adds a new view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <param name="viewName">The name of the view. This can be used to retrieve it later by calling <see cref="IRegion.GetView"/>.</param>
        /// <param name="createRegionManagerScope">When <see langword="true"/>, the added view will receive a new instance of <see cref="IRegionManager"/>, otherwise it will use the current region manager for this region.</param>
        /// <returns>The <see cref="IRegionManager"/> that is set on the view if it is a <see cref="VisualElement"/>.</returns>
        public virtual IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            IRegionManager manager = createRegionManagerScope ? RegionManager.CreateRegionManager() : RegionManager;
            InnerAdd(view, viewName, manager);
            return manager;
        }

        private void InnerAdd(object view, string viewName, IRegionManager scopedRegionManager)
        {
            if (ItemMetadataCollection.FirstOrDefault(x => x.Item == view) != null)
            {
                throw new RegionViewException(Resources.RegionViewExistsException);
            }

            if (view is not VisualElement visualElement)
            {
                throw new RegionViewException(Resources.RegionViewIsNotVisualElementException);
            }

            var itemMetadata = new ItemMetadata(visualElement);
            if (!string.IsNullOrEmpty(viewName))
            {
                if (ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName) != null)
                {
                    throw new RegionViewException(string.Format(CultureInfo.InvariantCulture, Resources.RegionViewNameExistsException, viewName));
                }
                itemMetadata.Name = viewName;
            }

            Xaml.RegionManager.SetRegionManager(visualElement, scopedRegionManager);

            ItemMetadataCollection.Add(itemMetadata);
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
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, nameof(viewName)));
            }

            var metadata = ItemMetadataCollection.FirstOrDefault(x => x.Name == viewName);

            if (metadata != null)
            {
                return metadata.Item;
            }

            return null;
        }

        /// <summary>
        /// Removes the specified view from the region.
        /// </summary>
        /// <param name="view">The view to remove.</param>
        public void Remove(object view)
        {
            if (view is not VisualElement visualElement)
            {
                throw new Exception("View is not a Visual Element");
            }

            var itemMetadata = GetItemMetadataOrThrow(view);

            ItemMetadataCollection.Remove(itemMetadata);

            if (Xaml.RegionManager.GetRegionManager(visualElement) == RegionManager)
            {
                visualElement.ClearValue(Xaml.RegionManager.RegionManagerProperty);
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

        private ItemMetadata GetItemMetadataOrThrow(object view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            var itemMetadata = ItemMetadataCollection.FirstOrDefault(x => x.Item == view);

            if (itemMetadata == null)
                throw new ArgumentException(Resources.ViewNotInRegionException, nameof(view));

            return itemMetadata;
        }

        internal static int DefaultSortComparison(object x, object y)
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

        /// <inheritDoc />
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, INavigationParameters navigationParameters)
        {
            _regionNavigationService.RequestNavigate(target, navigationCallback, navigationParameters);
        }
    }
}
