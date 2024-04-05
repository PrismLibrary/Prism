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
    /// Attached property value inheritance is not available in Silverlight, so the current approach walks up the visual tree when requesting a region from a region manager.
    /// The <see cref="RegionManagerRegistrationBehavior"/> is now responsible for walking up the Tree.
    /// </remarks>
    public class DelayedRegionCreationBehavior
    {
        private readonly RegionAdapterMappings _regionAdapterMappings;
        private WeakReference elementWeakReference;
        private bool regionCreated;

        private static ICollection<DelayedRegionCreationBehavior> _instanceTracker = new Collection<DelayedRegionCreationBehavior>();
        private object _trackerLock = new object();

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
            RegionManagerAccessor.UpdatingRegions += OnUpdatingRegions;
            WireUpTargetElement();
        }

        /// <summary>
        /// Stop monitoring the <see cref="RegionManager"/> and the  <see cref="TargetElement"/>, so that this behavior can be garbage collected.
        /// </summary>
        public void Detach()
        {
            RegionManagerAccessor.UpdatingRegions -= OnUpdatingRegions;
            UnWireTargetElement();
        }

        /// <summary>
        /// Called when the <see cref="RegionManager"/> is updating it's <see cref="RegionManager.Regions"/> collection.
        /// </summary>
        /// <remarks>
        /// This method has to be public, because it has to be callable using weak references in silverlight and other partial trust environments.
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
                IRegionAdapter regionAdapter = _regionAdapterMappings.GetMapping(targetElement.GetType());
                IRegion region = regionAdapter.Initialize(targetElement, regionName);

                return region;
            }
            catch (Exception ex)
            {
                throw new RegionCreationException(string.Format(CultureInfo.CurrentCulture, Resources.RegionCreationException, regionName, ex), ex);
            }
        }

        private void ElementLoaded(object sender, RoutedEventArgs e)
        {
            UnWireTargetElement();
            TryCreateRegion();
        }

        private void WireUpTargetElement()
        {
            FrameworkElement element = TargetElement as FrameworkElement;
            if (element != null)
            {
                element.Loaded += ElementLoaded;
                return;
            }

#if !UNO_WINUI
            FrameworkContentElement fcElement = TargetElement as FrameworkContentElement;
            if (fcElement != null)
            {
                fcElement.Loaded += ElementLoaded;
                return;
            }
#endif

            //if the element is a dependency object, and not a FrameworkElement, nothing is holding onto the reference after the DelayedRegionCreationBehavior
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
            FrameworkElement element = TargetElement as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= ElementLoaded;
                return;
            }

#if !UNO_WINUI
            FrameworkContentElement fcElement = TargetElement as FrameworkContentElement;
            if (fcElement != null)
            {
                fcElement.Loaded -= ElementLoaded;
                return;
            }
#endif

            DependencyObject depObj = TargetElement as DependencyObject;
            if (depObj != null)
            {
                Untrack();
                return;
            }
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
