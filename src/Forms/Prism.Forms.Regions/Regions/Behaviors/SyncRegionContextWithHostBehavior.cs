using System;
using Prism.Common;
using Prism.Properties;
using Xamarin.Forms;

namespace Prism.Regions.Behaviors
{
    /// <summary>
    /// Behavior that synchronizes the <see cref="IRegion.Context"/> property of a <see cref="IRegion"/> with 
    /// the control that hosts the Region. It does this by setting the <see cref="Xaml.RegionManager.RegionContextProperty"/> 
    /// Dependency Property on the host control.
    /// 
    /// This behavior allows the usage of two way databinding of the RegionContext from XAML. 
    /// </summary>
    public class SyncRegionContextWithHostBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        private const string RegionContextPropertyName = "Context";
        private VisualElement _hostControl;

        /// <summary>
        /// Name that identifies the SyncRegionContextWithHostBehavior behavior in a collection of RegionsBehaviors. 
        /// </summary>
        public static readonly string BehaviorKey = "SyncRegionContextWithHost";

        private ObservableObject<object> HostControlRegionContext => RegionContext.GetObservableContext(_hostControl);

        /// <summary>
        /// Gets or sets the <see cref="VisualElement"/> that the <see cref="IRegion"/> is attached to.
        /// </summary>
        /// <value>
        /// A <see cref="VisualElement"/> that the <see cref="IRegion"/> is attached to.
        /// This is usually a <see cref="VisualElement"/> that is part of the tree.
        /// </value>
        public VisualElement HostControl
        {
            get
            {
                return _hostControl;
            }
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
        /// Override this method to perform the logic after the behavior has been attached.
        /// </summary>
        protected override void OnAttach()
        {
            if (HostControl != null)
            {
                // Sync values initially. 
                SynchronizeRegionContext();

                // Now register for events to keep them in sync
                HostControlRegionContext.PropertyChanged += RegionContextObservableObject_PropertyChanged;
                Region.PropertyChanged += Region_PropertyChanged;
            }
        }

        void Region_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RegionContextPropertyName)
            {
                if (Xaml.RegionManager.GetRegionContext(HostControl) != Region.Context)
                {
                    // Setting this Dependency Property will automatically also change the HostControlRegionContext.Value
                    // (see RegionManager.OnRegionContextChanged())
                    Xaml.RegionManager.SetRegionContext(_hostControl, Region.Context);
                }
            }
        }

        void RegionContextObservableObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                SynchronizeRegionContext();
            }
        }

        private void SynchronizeRegionContext()
        {
            // Forward this value to the Region
            if (Region.Context != HostControlRegionContext.Value)
            {
                Region.Context = HostControlRegionContext.Value;
            }

            // Also make sure the region's DependencyProperty was changed (this can occur if the value
            // was changed only on the HostControlRegionContext)
            if (Xaml.RegionManager.GetRegionContext(HostControl) != HostControlRegionContext.Value)
            {
                Xaml.RegionManager.SetRegionContext(HostControl, HostControlRegionContext.Value);
            }
        }
    }
}
