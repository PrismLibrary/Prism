

using System;
using System.Windows;
using Prism.Properties;
using Prism.Common;

namespace Prism.Regions.Behaviors
{
    /// <summary>
    /// Behavior that synchronizes the <see cref="IRegion.Context"/> property of a <see cref="IRegion"/> with 
    /// the control that hosts the Region. It does this by setting the <see cref="RegionManager.RegionContextProperty"/> 
    /// Dependency Property on the host control.
    /// 
    /// This behavior allows the usage of two way databinding of the RegionContext from XAML. 
    /// </summary>
    public class SyncRegionContextWithHostBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        private const string RegionContextPropertyName = "Context";
        private DependencyObject hostControl;

        /// <summary>
        /// Name that identifies the SyncRegionContextWithHostBehavior behavior in a collection of RegionsBehaviors. 
        /// </summary>
        public static readonly string BehaviorKey = "SyncRegionContextWithHost";

        private ObservableObject<object> HostControlRegionContext
        {
            get
            {
                return RegionContext.GetObservableContext(this.hostControl);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// </summary>
        /// <value>
        /// A <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// This is usually a <see cref="FrameworkElement"/> that is part of the tree.
        /// </value>
        public DependencyObject HostControl
        {
            get
            {
                return hostControl;
            }
            set
            {
                if (IsAttached)
                {
                    throw new InvalidOperationException(Resources.HostControlCannotBeSetAfterAttach);
                }
                this.hostControl = value;
            }
        }

        /// <summary>
        /// Override this method to perform the logic after the behavior has been attached.
        /// </summary>
        protected override void OnAttach()
        {
            if (this.HostControl != null)
            {
                // Sync values initially. 
                SynchronizeRegionContext();

                // Now register for events to keep them in sync
                this.HostControlRegionContext.PropertyChanged += this.RegionContextObservableObject_PropertyChanged;
                this.Region.PropertyChanged += this.Region_PropertyChanged;
            }
        }

        void Region_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RegionContextPropertyName)
            {
                if (RegionManager.GetRegionContext(this.HostControl) != this.Region.Context)
                {
                    // Setting this Dependency Property will automatically also change the HostControlRegionContext.Value
                    // (see RegionManager.OnRegionContextChanged())
                    RegionManager.SetRegionContext(this.hostControl, this.Region.Context);
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
            if (this.Region.Context != this.HostControlRegionContext.Value)
            {
                this.Region.Context = this.HostControlRegionContext.Value;
            }

            // Also make sure the region's DependencyProperty was changed (this can occur if the value
            // was changed only on the HostControlRegionContext)
            if (RegionManager.GetRegionContext(this.HostControl) != this.HostControlRegionContext.Value)
            {
                RegionManager.SetRegionContext(this.HostControl, this.HostControlRegionContext.Value);
            }
        }
    }
}