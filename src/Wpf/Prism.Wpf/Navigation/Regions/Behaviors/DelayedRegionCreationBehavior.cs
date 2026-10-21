using System.Collections.ObjectModel;
using System.Globalization;
using Prism.Properties;

namespace Prism.Navigation.Regions.Behaviors
{
    /// <summary>
    /// Behavior that creates a new <see cref="IRegion"/>, when the control that will host the <see cref="IRegion"/> (see <see cref="TargetElement"/>)
    /// is added to the VisualTree. This behavior will use the <see cref="RegionAdapterMappings"/> class to find the right type of adapter to create
    /// the region. After the region is created, this behavior will detach.
    /// </summary>
    /// <remarks>
    /// Region registration locates the applicable region manager by walking the visual tree.
    /// The <see cref="RegionManagerRegistrationBehavior"/> is now responsible for walking up the Tree.
    /// </remarks>
    public class DelayedRegionCreationBehavior
    {
        private readonly RegionAdapterMappings _regionAdapterMappings;
        private WeakReference elementWeakReference;
        private bool regionCreated;
#if WINUI && !UNO_WINUI
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<DependencyObject, DelayedRegionCreationBehavior> Pending = new();

        internal DelayedRegionCreationBehavior() : this(null)
        {
        }

        internal static void DetachFrom(DependencyObject element)
        {
            if (Pending.TryGetValue(element, out var behavior))
                behavior.Detach();
        }
#endif

        private static ICollection<DelayedRegionCreationBehavior> _instanceTracker = new Collection<DelayedRegionCreationBehavior>();
        private static readonly object _trackerLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedRegionCreationBehavior"/> class.
        /// </summary>
        /// <param name="regionAdapterMappings">
        /// The region adapter mappings, that are used to find the correct adapter for
        /// a given control type. The control type is determined by the <see name="TargetElement"/> value.
        /// </param>
        public DelayedRegionCreationBehavior(RegionAdapterMappings regionAdapterMappings)
        {
            _regionAdapterMappings = regionAdapterMappings;
            RegionManagerAccessor = new DefaultRegionManagerAccessor();
        }

        /// <summary>
        /// Sets a class that interfaces between the <see cref="RegionManager"/> 's static properties/events and this behavior,
        /// so this behavior can be tested in isolation.
        /// </summary>
        /// <value>The region manager accessor.</value>
        public IRegionManagerAccessor RegionManagerAccessor { get; set; }

        /// <summary>
        /// The element that will host the Region.
        /// </summary>
        /// <value>The target element.</value>
        public DependencyObject TargetElement
        {
            get { return elementWeakReference != null ? elementWeakReference.Target as DependencyObject : null; }
            set { elementWeakReference = new WeakReference(value); }
        }

        /// <summary>
        /// Start monitoring the <see cref="RegionManager"/> and the <see cref="TargetElement"/> to detect when the <see cref="TargetElement"/> becomes
        /// part of the Visual Tree. When that happens, the Region will be created and the behavior will <see cref="Detach"/>.
        /// </summary>
        public void Attach()
        {
#if WINUI && !UNO_WINUI
            DetachFrom(TargetElement);
            Pending.Add(TargetElement, this);
#endif
            RegionManagerAccessor.UpdatingRegions += OnUpdatingRegions;
            WireUpTargetElement();
        }

        /// <summary>
        /// Stop monitoring the <see cref="RegionManager"/> and the  <see cref="TargetElement"/>, so that this behavior can be garbage collected.
        /// </summary>
        public void Detach()
        {
#if WINUI && !UNO_WINUI
            if (TargetElement is DependencyObject element)
                Pending.Remove(element);
#endif
            RegionManagerAccessor.UpdatingRegions -= OnUpdatingRegions;
            UnWireTargetElement();
        }

        /// <summary>
        /// Called when the <see cref="RegionManager"/> is updating it's <see cref="RegionManager.Regions"/> collection.
        /// </summary>
        /// <remarks>
        /// This method is public to support weak event subscriptions.
        /// </remarks>
        /// <param name="sender">The <see cref="RegionManager"/>. </param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This has to be public in order to work with weak references in partial trust or Silverlight environments.")]
        public void OnUpdatingRegions(object sender, EventArgs e)
        {
            TryCreateRegion();
        }

        private void TryCreateRegion()
        {
            DependencyObject targetElement = TargetElement;
            if (targetElement == null)
            {
                Detach();
                return;
            }

            if (targetElement.CheckAccess())
            {
#if WINUI && !UNO_WINUI
                // XAML sets RegionName before the control is connected to its window shell.
                // Keep listening instead of resolving from the application's root container.
                if (NativeRegionScope.FindProvider(targetElement) is null)
                    return;
#endif
                Detach();

                if (!regionCreated)
                {
                    string regionName = RegionManagerAccessor.GetRegionName(targetElement);
                    CreateRegion(targetElement, regionName);
                    regionCreated = true;
                }
            }
        }

        /// <summary>
        /// Method that will create the region, by calling the right <see cref="IRegionAdapter"/>.
        /// </summary>
        /// <param name="targetElement">The target element that will host the <see cref="IRegion"/>.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <returns>The created <see cref="IRegion"/></returns>
        protected virtual IRegion CreateRegion(DependencyObject targetElement, string regionName)
        {
            if (targetElement == null)
                throw new ArgumentNullException(nameof(targetElement));

            try
            {
                // Build the region
#if WINUI && !UNO_WINUI
                var provider = NativeRegionScope.RequireProvider(targetElement);
                var mappings = _regionAdapterMappings ?? provider.Resolve<RegionAdapterMappings>();
                IRegionAdapter regionAdapter = mappings.GetMapping(targetElement.GetType(), provider);
#else
                IRegionAdapter regionAdapter = _regionAdapterMappings.GetMapping(targetElement.GetType());
#endif
                IRegion region = regionAdapter.Initialize(targetElement, regionName);
#if WINUI && !UNO_WINUI
                NativeRegionScope.SetProvider(region, provider);
#endif

                return region;
            }
            catch (Exception ex)
            {
                throw new RegionCreationException(string.Format(CultureInfo.CurrentCulture, Resources.RegionCreationException, regionName, ex), ex);
            }
        }

#if !AVALONIA
        private void ElementLoaded(object sender, RoutedEventArgs e)
#else
        private void ElementLoaded(object sender, VisualTreeAttachmentEventArgs e)
#endif
        {
#if WINUI && !UNO_WINUI
            NativeRegionScope.RequireProvider(TargetElement);
#endif
            UnWireTargetElement();
            TryCreateRegion();
        }

        private void WireUpTargetElement()
        {
            FrameworkElement element = TargetElement as FrameworkElement;
            if (element != null)
            {
#if !AVALONIA
                element.Loaded += ElementLoaded;
#else
                element.AttachedToVisualTree += ElementLoaded;
#endif
                return;
            }

#if (!UNO_WINUI && !WINUI && !AVALONIA)
            FrameworkContentElement fcElement = TargetElement as FrameworkContentElement;
            if (fcElement != null)
            {
                fcElement.Loaded += ElementLoaded;
                return;
            }
#endif

            //if the element is a dependency/avalonia object, and not a FrameworkElement/Avalonia.Control, nothing is holding onto the reference after the DelayedRegionCreationBehavior
            //is instantiated inside RegionManager.CreateRegion(DependencyObject element). If the GC runs before RegionManager.UpdateRegions is called, the region will
            //never get registered because it is gone from the updatingRegionsListeners list inside RegionManager. So we need to hold on to it. This should be rare.
            DependencyObject depObj = TargetElement as DependencyObject;
            if (depObj != null)
            {
                Track();
                return;
            }
        }

        private void UnWireTargetElement()
        {
            // The weak target may already have been collected.
            Untrack();

            FrameworkElement element = TargetElement as FrameworkElement;
            if (element != null)
            {
#if !AVALONIA
                element.Loaded -= ElementLoaded;
#else
                element.AttachedToVisualTree -= ElementLoaded;
#endif
                return;
            }

#if (!UNO_WINUI && !WINUI && !AVALONIA)
            FrameworkContentElement fcElement = TargetElement as FrameworkContentElement;
            if (fcElement != null)
            {
                fcElement.Loaded -= ElementLoaded;
                return;
            }
#endif
        }

        /// <summary>
        /// Add the instance of this class to <see cref="_instanceTracker"/> to keep it alive
        /// </summary>
        private void Track()
        {
            lock (_trackerLock)
            {
                if (!_instanceTracker.Contains(this))
                {
                    _instanceTracker.Add(this);
                }
            }
        }

        /// <summary>
        /// Remove the instance of this class from <see cref="_instanceTracker"/>
        /// so it can eventually be garbage collected
        /// </summary>
        private void Untrack()
        {
            lock (_trackerLock)
            {
                _instanceTracker.Remove(this);
            }
        }
    }
}
