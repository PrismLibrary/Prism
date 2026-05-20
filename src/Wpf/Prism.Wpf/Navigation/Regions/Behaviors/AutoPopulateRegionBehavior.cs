using System;
using System.Collections.Generic;
using Prism.Common;
using Prism.Ioc;
using Prism.Properties;

namespace Prism.Navigation.Regions.Behaviors
{
    /// <summary>
    /// Populates the target region with the views registered to it in the <see cref="IRegionViewRegistry"/>.
    /// </summary>
    public class AutoPopulateRegionBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        /// <summary>
        /// The key of this behavior.
        /// </summary>
        public const string BehaviorKey = "AutoPopulate";

        private readonly IRegionViewRegistry regionViewRegistry;
        private DependencyObject hostControl;
        private bool isAttached;

        /// <summary>
        /// Creates a new instance of the AutoPopulateRegionBehavior
        /// associated with the <see cref="IRegionViewRegistry"/> received.
        /// </summary>
        /// <param name="regionViewRegistry"><see cref="IRegionViewRegistry"/> that the behavior will monitor for views to populate the region.</param>
        public AutoPopulateRegionBehavior(IRegionViewRegistry regionViewRegistry)
        {
            this.regionViewRegistry = regionViewRegistry;
        }

        /// <summary>
        /// Gets or sets the <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// </summary>
        public DependencyObject HostControl
        {
            get => hostControl;
            set
            {
                if (isAttached)
                {
                    throw new InvalidOperationException(Resources.HostControlCannotBeSetAfterAttach);
                }

                hostControl = value;
            }
        }

        /// <summary>
        /// Attaches the AutoPopulateRegionBehavior to the Region.
        /// </summary>
        protected override void OnAttach()
        {
            if (string.IsNullOrEmpty(Region.Name))
            {
                Region.PropertyChanged += Region_PropertyChanged;
            }
            else
            {
                StartPopulatingContent();
            }
        }

        private void StartPopulatingContent()
        {
            isAttached = true;

            foreach (object view in CreateViewsToAutoPopulate())
            {
                AddViewIntoRegion(view);
            }

            TryAddDefaultView();

            regionViewRegistry.ContentRegistered += OnViewRegistered;
        }

        /// <summary>
        /// Returns a collection of views that will be added to the
        /// View collection.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<object> CreateViewsToAutoPopulate()
        {
            return regionViewRegistry.GetContents(Region.Name, ContainerLocator.Container);
        }

        /// <summary>
        /// Adds a view into the views collection of this region.
        /// </summary>
        /// <param name="viewToAdd"></param>
        protected virtual void AddViewIntoRegion(object viewToAdd)
        {
            Region.Add(viewToAdd);
        }

        private void TryAddDefaultView()
        {
            if (HostControl == null)
            {
                return;
            }

            var defaultView = RegionManager.GetDefaultView(HostControl);
            if (defaultView == null)
            {
                return;
            }

            if (defaultView is string targetName)
            {
                if (Region.GetView(targetName) != null)
                {
                    return;
                }

                var view = ContainerLocator.Container.Resolve(typeof(object), targetName);
                if (!Region.Views.Contains(view))
                {
                    Region.Add(view, targetName);
                }
            }
            else if (defaultView is Type viewType)
            {
                if (!RegionContainsViewOfType(viewType))
                {
                    var view = ContainerLocator.Container.Resolve(viewType);
                    MvvmHelpers.AutowireViewModel(view);
                    Region.Add(view);
                }
            }
            else
            {
                if (!Region.Views.Contains(defaultView))
                {
                    Region.Add(defaultView);
                }
            }
        }

        private bool RegionContainsViewOfType(Type viewType)
        {
            foreach (object view in Region.Views)
            {
                if (view != null && viewType.IsInstanceOfType(view))
                {
                    return true;
                }
            }

            return false;
        }

        private void Region_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && !string.IsNullOrEmpty(Region.Name))
            {
                Region.PropertyChanged -= Region_PropertyChanged;
                StartPopulatingContent();
            }
        }

        /// <summary>
        /// Handler of the event that fires when a new viewtype is registered to the registry.
        /// </summary>
        /// <remarks>Although this is a public method to support Weak Delegates in Silverlight, it should not be called by the user.</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This has to be public in order to work with weak references in partial trust or Silverlight environments.")]
        public virtual void OnViewRegistered(object sender, ViewRegisteredEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.RegionName == Region.Name)
            {
                AddViewIntoRegion(e.GetView(ContainerLocator.Container));
            }
        }
    }
}
