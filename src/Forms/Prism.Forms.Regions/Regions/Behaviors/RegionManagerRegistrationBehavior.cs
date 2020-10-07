using System;
using System.ComponentModel;
using Prism.Properties;
using Xamarin.Forms;

namespace Prism.Regions.Behaviors
{
    /// <summary>
    /// Subscribes to a static event from the <see cref="RegionManager"/> in order to register the target <see cref="IRegion"/>
    /// in a <see cref="IRegionManager"/> when one is available on the host control by walking up the tree and finding
    /// a control whose <see cref="Xaml.RegionManager.RegionManagerProperty"/> property is not <see langword="null"/>.
    /// </summary>
    public class RegionManagerRegistrationBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        /// <summary>
        /// The key of this behavior.
        /// </summary>
        public static readonly string BehaviorKey = "RegionManagerRegistration";

        private WeakReference _attachedRegionManagerWeakReference;
        private VisualElement _hostControl;

        /// <summary>
        /// Initializes a new instance of <see cref="RegionManagerRegistrationBehavior"/>.
        /// </summary>
        public RegionManagerRegistrationBehavior()
        {
            RegionManagerAccessor = new DefaultRegionManagerAccessor();
        }

        /// <summary>
        /// Provides an abstraction on top of the RegionManager static members.
        /// </summary>
        public IRegionManagerAccessor RegionManagerAccessor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="VisualElement"/> that the <see cref="IRegion"/> is attached to.
        /// </summary>
        /// <value>A <see cref="VisualElement"/> that the <see cref="IRegion"/> is attached to.
        /// This is usually a <see cref="View"/> that is part of the tree.</value>
        /// <exception cref="InvalidOperationException">When this member is set after the <see cref="IRegionBehavior.Attach"/> method has being called.</exception>
        public VisualElement HostControl
        {
            get => _hostControl;
            set
            {
                if (IsAttached)
                {
                    throw new InvalidOperationException(Resources.HostControlCannotBeSetAfterAttach);
                }
                _hostControl = value;
            }
        }

        /// <summary>
        /// When the <see cref="IRegion"/> has a name assigned, the behavior will start monitoring the ancestor controls in the element tree
        /// to look for an <see cref="IRegionManager"/> where to register the region in.
        /// </summary>
        protected override void OnAttach()
        {
            if (string.IsNullOrEmpty(Region.Name))
            {
                Region.PropertyChanged += Region_PropertyChanged;
            }
            else
            {
                StartMonitoringRegionManager();
            }
        }

        private void Region_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && !string.IsNullOrEmpty(Region.Name))
            {
                Region.PropertyChanged -= Region_PropertyChanged;
                StartMonitoringRegionManager();
            }
        }

        private void StartMonitoringRegionManager()
        {
            RegionManagerAccessor.UpdatingRegions += OnUpdatingRegions;
            TryRegisterRegion();
        }

        private void TryRegisterRegion()
        {
            var targetElement = HostControl;
            //if (targetElement.CheckAccess())
            {
                var regionManager = FindRegionManager(targetElement);
                var attachedRegionManager = GetAttachedRegionManager();

                if (regionManager != attachedRegionManager)
                {
                    if (attachedRegionManager != null)
                    {
                        _attachedRegionManagerWeakReference = null;
                        attachedRegionManager.Regions.Remove(Region.Name);
                    }

                    if (regionManager != null)
                    {
                        _attachedRegionManagerWeakReference = new WeakReference(regionManager);
                        regionManager.Regions.Add(Region);
                    }
                }
            }
        }

        /// <summary>
        /// This event handler gets called when a RegionManager is requering the instances of a region to be registered if they are not already.
        /// <remarks>Although this is a public method to support Weak Delegates in Silverlight, it should not be called by the user.</remarks>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The arguments.</param>
        public void OnUpdatingRegions(object sender, EventArgs e)
        {
            TryRegisterRegion();
        }

        private IRegionManager FindRegionManager(Element element)
        {
            if (element is VisualElement visualElement)
            {
                var regionmanager = RegionManagerAccessor.GetRegionManager(visualElement);
                if (regionmanager != null)
                {
                    return regionmanager;
                }
            }

            if (element.Parent != null)
            {
                return FindRegionManager(element.Parent);
            }

            return null;
        }

        private IRegionManager GetAttachedRegionManager()
        {
            if (_attachedRegionManagerWeakReference != null)
            {
                return _attachedRegionManagerWeakReference.Target as IRegionManager;
            }

            return null;
        }
    }
}
