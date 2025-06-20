using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Prism.Common;
using Prism.Events;
using Prism.Ioc.Internals;
using Prism.Navigation.Regions.Behaviors;
using Prism.Properties;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// This class is responsible for maintaining a collection of regions and attaching regions to controls.
    /// </summary>
    /// <remarks>
    /// This class supplies the attached properties that can be used for simple region creation from XAML.
    /// </remarks>
    public class RegionManager : IRegionManager
    {
        #region Static members (for XAML support)

        private static readonly WeakDelegatesManager updatingRegionsListeners = new WeakDelegatesManager();

        /// <summary>
        /// Identifies the RegionName attached property.
        /// </summary>
        /// <remarks>
        /// When a control has both the <see cref="RegionNameProperty"/> and
        /// <see cref="RegionManagerProperty"/> attached properties set to
        /// a value different than <see langword="null" /> and there is a
        /// <see cref="IRegionAdapter"/> mapping registered for the control, it
        /// will create and adapt a new region for that control, and register it
        /// in the <see cref="IRegionManager"/> with the specified region name.
        /// </remarks>
#if !AVALONIA
        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached(
            "RegionName",
            typeof(string),
            typeof(RegionManager),
            new PropertyMetadata(defaultValue: null, propertyChangedCallback: OnSetRegionNameCallback));
#else
        public static readonly AvaloniaProperty RegionNameProperty = AvaloniaProperty.RegisterAttached<AvaloniaObject, string>(
            "RegionName",
            typeof(RegionManager));
#endif

        /// <summary>
        /// Sets the <see cref="RegionNameProperty"/> attached property.
        /// </summary>
        /// <param name="regionTarget">The object to adapt. This is typically a container (i.e a control).</param>
        /// <param name="regionName">The name of the region to register.</param>
        public static void SetRegionName(DependencyObject regionTarget, string regionName)
        {
            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            regionTarget.SetValue(RegionNameProperty, regionName);
        }

        /// <summary>
        /// Gets the value for the <see cref="RegionNameProperty"/> attached property.
        /// </summary>
        /// <param name="regionTarget">The object to adapt. This is typically a container (i.e a control).</param>
        /// <returns>The name of the region that should be created when
        /// <see cref="RegionManagerProperty"/> is also set in this element.</returns>
        public static string GetRegionName(DependencyObject regionTarget)
        {
            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            return regionTarget.GetValue(RegionNameProperty) as string;
        }

#if !AVALONIA
        private static readonly DependencyProperty ObservableRegionProperty =
        DependencyProperty.RegisterAttached("ObservableRegion", typeof(ObservableObject<IRegion>), typeof(RegionManager), null);
#else
        private static readonly AvaloniaProperty ObservableRegionProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, ObservableObject<IRegion>>("ObservableRegion", typeof(RegionManager));
#endif

        /// <summary>
        /// Returns an <see cref="ObservableObject{T}"/> wrapper that can hold an <see cref="IRegion"/>. Using this wrapper
        /// you can detect when an <see cref="IRegion"/> has been created by the <see cref="RegionAdapterBase{T}"/>.
        ///
        /// If the <see cref="ObservableObject{T}"/> wrapper does not yet exist, a new wrapper will be created. When the region
        /// gets created and assigned to the wrapper, you can use the <see cref="ObservableObject{T}.PropertyChanged"/> event
        /// to get notified of that change.
        /// </summary>
        /// <param name="view">The view that will host the region. </param>
        /// <returns>Wrapper that can hold an <see cref="IRegion"/> value and can notify when the <see cref="IRegion"/> value changes. </returns>
        public static ObservableObject<IRegion> GetObservableRegion(DependencyObject view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            ObservableObject<IRegion> regionWrapper = view.GetValue(ObservableRegionProperty) as ObservableObject<IRegion>;

            if (regionWrapper == null)
            {
                regionWrapper = new ObservableObject<IRegion>();
                view.SetValue(ObservableRegionProperty, regionWrapper);
            }

            return regionWrapper;
        }

        private static void OnSetRegionNameCallback(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            if (!IsInDesignMode(element))
            {
                CreateRegion(element);
            }
        }

        private static void CreateRegion(DependencyObject element)
        {
            var container = ContainerLocator.Container;
            DelayedRegionCreationBehavior regionCreationBehavior = container.Resolve<DelayedRegionCreationBehavior>();
            regionCreationBehavior.TargetElement = element;
            regionCreationBehavior.Attach();
        }

        /// <summary>
        /// Identifies the RegionManager attached property.
        /// </summary>
        /// <remarks>
        /// When a control has both the <see cref="RegionNameProperty"/> and
        /// <see cref="RegionManagerProperty"/> attached properties set to
        /// a value different than <see langword="null" /> and there is a
        /// <see cref="IRegionAdapter"/> mapping registered for the control, it
        /// will create and adapt a new region for that control, and register it
        /// in the <see cref="IRegionManager"/> with the specified region name.
        /// </remarks>
#if !AVALONIA
        public static readonly DependencyProperty RegionManagerProperty =
            DependencyProperty.RegisterAttached("RegionManager", typeof(IRegionManager), typeof(RegionManager), null);
#else
        public static readonly AvaloniaProperty RegionManagerProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, IRegionManager>("RegionManager", typeof(RegionManager));
#endif

        /// <summary>
        /// Gets the value of the <see cref="RegionNameProperty"/> attached property.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <returns>The <see cref="IRegionManager"/> attached to the <paramref name="target"/> element.</returns>
        public static IRegionManager GetRegionManager(DependencyObject target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return (IRegionManager)target.GetValue(RegionManagerProperty);
        }

        /// <summary>
        /// Sets the <see cref="RegionManagerProperty"/> attached property.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <param name="value">The value.</param>
        public static void SetRegionManager(DependencyObject target, IRegionManager value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.SetValue(RegionManagerProperty, value);
        }

        /// <summary>
        /// Identifies the RegionContext attached property.
        /// </summary>
#if !AVALONIA
        public static readonly DependencyProperty RegionContextProperty =
        DependencyProperty.RegisterAttached("RegionContext", typeof(object), typeof(RegionManager), new PropertyMetadata(defaultValue: null, propertyChangedCallback: OnRegionContextChanged));
#else
        public static readonly AvaloniaProperty RegionContextProperty =
                    AvaloniaProperty.RegisterAttached<AvaloniaObject, object>("RegionContext", typeof(RegionManager));
#endif

        private static void OnRegionContextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (RegionContext.GetObservableContext(depObj).Value != e.NewValue)
            {
                RegionContext.GetObservableContext(depObj).Value = e.NewValue;
            }
        }

        /// <summary>
        /// Gets the value of the <see cref="RegionContextProperty"/> attached property.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <returns>The region context to pass to the contained views.</returns>
        public static object GetRegionContext(DependencyObject target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return target.GetValue(RegionContextProperty);
        }

        /// <summary>
        /// Sets the <see cref="RegionContextProperty"/> attached property.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <param name="value">The value.</param>
        public static void SetRegionContext(DependencyObject target, object value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.SetValue(RegionContextProperty, value);
        }

        /// <summary>
        /// Notification used by attached behaviors to update the region managers appropriately if needed to.
        /// </summary>
        /// <remarks>This event uses weak references to the event handler to prevent this static event of keeping the
        /// target element longer than expected.</remarks>
        public static event EventHandler UpdatingRegions
        {
            add { updatingRegionsListeners.AddListener(value); }
            remove { updatingRegionsListeners.RemoveListener(value); }
        }

        /// <summary>
        /// Notifies attached behaviors to update the region managers appropriately if needed to.
        /// </summary>
        /// <remarks>
        /// This method is normally called internally, and there is usually no need to call this from user code.
        /// </remarks>
        public static void UpdateRegions()
        {

            try
            {
                updatingRegionsListeners.Raise(null, EventArgs.Empty);
            }
            catch (TargetInvocationException ex)
            {
                Exception rootException = ex.GetRootException();

                throw new UpdateRegionsException(string.Format(CultureInfo.CurrentCulture,
                    Resources.UpdateRegionException, rootException), ex.InnerException);
            }
        }

        private static bool IsInDesignMode(DependencyObject element)
        {
#if !AVALONIA
            return DesignerProperties.GetIsInDesignMode(element);
#else
            return Design.IsDesignMode;
#endif
        }

        #endregion

        private readonly RegionCollection regionCollection;

        /// <summary>
        /// Initializes a new instance of <see cref="RegionManager"/>.
        /// </summary>
        public RegionManager()
        {
            regionCollection = new RegionCollection(this);
        }

#if AVALONIA
        static RegionManager()
        {
            // TODO: Could this go into the default constructor?
            RegionNameProperty.Changed.Subscribe(args => OnSetRegionNameCallback(args?.Sender, args));
            RegionContextProperty.Changed.Subscribe(args => OnRegionContextChanged(args?.Sender, args));
        }
#endif

        /// <summary>
        /// Gets a collection of <see cref="IRegion"/> that identify each region by name. You can use this collection to add or remove regions to the current region manager.
        /// </summary>
        /// <value>A <see cref="IRegionCollection"/> with all the registered regions.</value>
        public IRegionCollection Regions
        {
            get { return regionCollection; }
        }

        /// <summary>
        /// Creates a new region manager.
        /// </summary>
        /// <returns>A new region manager that can be used as a different scope from the current region manager.</returns>
        public IRegionManager CreateRegionManager()
        {
            return new RegionManager();
        }

        /// <summary>
        ///     Add a view to the Views collection of a Region. Note that the region must already exist in this <see cref="IRegionManager"/>.
        /// </summary>
        /// <param name="regionName">The name of the region to add a view to</param>
        /// <param name="view">The view to add to the views collection</param>
        /// <returns>The RegionManager, to easily add several views. </returns>
        public IRegionManager AddToRegion(string regionName, object view)
        {
            if (!Regions.ContainsRegionWithName(regionName))
                throw new ArgumentException(string.Format(Thread.CurrentThread.CurrentCulture, Resources.RegionNotFound, regionName), nameof(regionName));

            return Regions[regionName].Add(view);
        }

        /// <summary>
        /// Add a view to the Views collection of a Region. Note that the region must already exist in this <see cref="IRegionManager"/>.
        /// </summary>
        /// <param name="regionName">The name of the region to add a view to</param>
        /// <param name="targetName">The view to add to the views collection</param>
        /// <returns>The RegionManager, to easily add several views. </returns>
        public IRegionManager AddToRegion(string regionName, string targetName)
        {
            if (!Regions.ContainsRegionWithName(regionName))
                throw new ArgumentException(string.Format(Thread.CurrentThread.CurrentCulture, Resources.RegionNotFound, regionName), nameof(regionName));

            var view = CreateNewRegionItem(targetName);

            return Regions[regionName].Add(view);
        }

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="viewType">The type of the view to register with the </param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
        {
            var regionViewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();

            regionViewRegistry.RegisterViewWithRegion(regionName, viewType);

            return this;
        }

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="targetName">The type of the view to register with the </param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        public IRegionManager RegisterViewWithRegion(string regionName, string targetName)
        {
            var viewType = ContainerLocator.Current.GetRegistrationType(targetName);

            return RegisterViewWithRegion(regionName, viewType);
        }

        /// <summary>
        /// Associate a view with a region, using a delegate to resolve a concrete instance of the view.
        /// When the region get's displayed, this delegate will be called and the result will be added to the
        /// views collection of the region.
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="getContentDelegate">The delegate used to resolve a concrete instance of the view.</param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        public IRegionManager RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> getContentDelegate)
        {
            var regionViewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();

            regionViewRegistry.RegisterViewWithRegion(regionName, getContentDelegate);

            return this;
        }

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        /// <param name="navigationCallback">The navigation callback.</param>
        public void RequestNavigate(string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            if (Regions.ContainsRegionWithName(regionName))
            {
                Regions[regionName].RequestNavigate(source, navigationCallback);
            }
            else
            {
                navigationCallback(new NavigationResult(new NavigationContext(null, source), false));
            }
        }

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target Uri, passing a navigation callback and an instance of <see cref="NavigationParameters"/>, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationCallback">The navigation callback that will be executed after the navigation is completed.</param>
        /// <param name="navigationParameters">An instance of <see cref="NavigationParameters"/>, which holds a collection of object parameters.</param>
        public void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback, INavigationParameters navigationParameters)
        {
            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            if (Regions.ContainsRegionWithName(regionName))
            {
                Regions[regionName].RequestNavigate(target, navigationCallback, navigationParameters);
            }
            else
            {
                navigationCallback(new NavigationResult(new NavigationContext(null, target, navigationParameters), false));
            }
        }

        /// <summary>
        /// Provides a new item for the region based on the supplied candidate target contract name.
        /// </summary>
        /// <param name="candidateTargetContract">The target contract to build.</param>
        /// <returns>An instance of an item to put into the <see cref="IRegion"/>.</returns>
        protected virtual object CreateNewRegionItem(string candidateTargetContract)
        {
            try
            {
                var view = ContainerLocator.Container.Resolve<object>(candidateTargetContract);

                MvvmHelpers.AutowireViewModel(view);

                return view;
            }
            catch (ContainerResolutionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.CannotCreateNavigationTarget, candidateTargetContract),
                    e);
            }
        }

        private class RegionCollection : IRegionCollection
        {
            private readonly IRegionManager _regionManager;
            private readonly List<IRegion> _regions;

            public RegionCollection(IRegionManager regionManager)
            {
                _regionManager = regionManager;
                _regions = new List<IRegion>();
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public IEnumerator<IRegion> GetEnumerator()
            {
                UpdateRegions();

                return _regions.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IRegion this[string regionName]
            {
                get
                {
                    UpdateRegions();

                    IRegion region = GetRegionByName(regionName);
                    if (region == null)
                    {
                        throw new KeyNotFoundException(string.Format(CultureInfo.CurrentUICulture, Resources.RegionNotInRegionManagerException, regionName));
                    }

                    return region;
                }
            }

            public void Add(IRegion region)
            {
                if (region == null)
                    throw new ArgumentNullException(nameof(region));

                UpdateRegions();

                if (region.Name == null)
                {
                    throw new InvalidOperationException(Resources.RegionNameCannotBeEmptyException);
                }

                if (GetRegionByName(region.Name) != null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                                                              Resources.RegionNameExistsException, region.Name));
                }

                _regions.Add(region);
                region.RegionManager = _regionManager;

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, region, 0));
            }

            public bool Remove(string regionName)
            {
                UpdateRegions();

                bool removed = false;

                IRegion region = GetRegionByName(regionName);
                if (region != null)
                {
                    removed = true;
                    _regions.Remove(region);
                    region.RegionManager = null;

                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, region, 0));
                }

                return removed;
            }

            public bool ContainsRegionWithName(string regionName)
            {
                UpdateRegions();

                return GetRegionByName(regionName) != null;
            }

            /// <summary>
            /// Adds a region to the <see cref="RegionManager"/> with the name received as argument.
            /// </summary>
            /// <param name="regionName">The name to be given to the region.</param>
            /// <param name="region">The region to be added to the <see cref="RegionManager"/>.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="region"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException">Thrown if <paramref name="regionName"/> and <paramref name="region"/>'s name do not match and the <paramref name="region"/> <see cref="IRegion.Name"/> is not <see langword="null"/>.</exception>
            public void Add(string regionName, IRegion region)
            {
                if (region == null)
                    throw new ArgumentNullException(nameof(region));

                if (region.Name != null && region.Name != regionName)
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.RegionManagerWithDifferentNameException, region.Name, regionName), nameof(regionName));

                region.Name ??= regionName;

                Add(region);
            }

            private IRegion GetRegionByName(string regionName)
            {
                return _regions.FirstOrDefault(r => r.Name == regionName);
            }

            private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
            {
                CollectionChanged?.Invoke(this, notifyCollectionChangedEventArgs);
            }
        }
    }
}
