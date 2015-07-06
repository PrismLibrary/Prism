

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using Prism.Events;
using Prism.Properties;
using Prism.Regions.Behaviors;
using Microsoft.Practices.ServiceLocation;
using Prism.Common;

namespace Prism.Regions
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
        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached(
            "RegionName",
            typeof(string),
            typeof(RegionManager),
            new PropertyMetadata(OnSetRegionNameCallback));

        /// <summary>
        /// Sets the <see cref="RegionNameProperty"/> attached property.
        /// </summary>
        /// <param name="regionTarget">The object to adapt. This is typically a container (i.e a control).</param>
        /// <param name="regionName">The name of the region to register.</param>
        public static void SetRegionName(DependencyObject regionTarget, string regionName)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
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
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            return regionTarget.GetValue(RegionNameProperty) as string;
        }

        private static readonly DependencyProperty ObservableRegionProperty =
            DependencyProperty.RegisterAttached("ObservableRegion", typeof(ObservableObject<IRegion>), typeof(RegionManager), null);


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
            if (view == null) throw new ArgumentNullException("view");

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
            IServiceLocator locator = ServiceLocator.Current;
            DelayedRegionCreationBehavior regionCreationBehavior = locator.GetInstance<DelayedRegionCreationBehavior>();
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
        public static readonly DependencyProperty RegionManagerProperty =
            DependencyProperty.RegisterAttached("RegionManager", typeof(IRegionManager), typeof(RegionManager), null);

        /// <summary>
        /// Gets the value of the <see cref="RegionNameProperty"/> attached property.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <returns>The <see cref="IRegionManager"/> attached to the <paramref name="target"/> element.</returns>
        public static IRegionManager GetRegionManager(DependencyObject target)
        {
            if (target == null) throw new ArgumentNullException("target");
            return (IRegionManager)target.GetValue(RegionManagerProperty);
        }

        /// <summary>
        /// Sets the <see cref="RegionManagerProperty"/> attached property.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <param name="value">The value.</param>
        public static void SetRegionManager(DependencyObject target, IRegionManager value)
        {
            if (target == null) throw new ArgumentNullException("target");
            target.SetValue(RegionManagerProperty, value);
        }

        /// <summary>
        /// Identifies the RegionContext attached property.
        /// </summary>
        public static readonly DependencyProperty RegionContextProperty =
            DependencyProperty.RegisterAttached("RegionContext", typeof(object), typeof(RegionManager), new PropertyMetadata(OnRegionContextChanged));

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
            if (target == null) throw new ArgumentNullException("target");
            return target.GetValue(RegionContextProperty);
        }

        /// <summary>
        /// Sets the <see cref="RegionContextProperty"/> attached property.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <param name="value">The value.</param>
        public static void SetRegionContext(DependencyObject target, object value)
        {
            if (target == null) throw new ArgumentNullException("target");
            target.SetValue(RegionContextProperty, value);
        }

        /// <summary>
        /// Notification used by attached behaviors to update the region managers appropriatelly if needed to.
        /// </summary>
        /// <remarks>This event uses weak references to the event handler to prevent this static event of keeping the
        /// target element longer than expected.</remarks>
        public static event EventHandler UpdatingRegions
        {
            add { updatingRegionsListeners.AddListener(value); }
            remove { updatingRegionsListeners.RemoveListener(value); }
        }

        /// <summary>
        /// Notifies attached behaviors to update the region managers appropriatelly if needed to. 
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
            return DesignerProperties.GetIsInDesignMode(element);
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

        private class RegionCollection : IRegionCollection
        {
            private readonly IRegionManager regionManager;
            private readonly List<IRegion> regions;

            public RegionCollection(IRegionManager regionManager)
            {
                this.regionManager = regionManager;
                this.regions = new List<IRegion>();
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public IEnumerator<IRegion> GetEnumerator()
            {
                UpdateRegions();

                return this.regions.GetEnumerator();
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
                if (region == null) throw new ArgumentNullException("region");
                UpdateRegions();

                if (region.Name == null)
                {
                    throw new InvalidOperationException(Resources.RegionNameCannotBeEmptyException);
                }

                if (this.GetRegionByName(region.Name) != null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                                                              Resources.RegionNameExistsException, region.Name));
                }

                this.regions.Add(region);
                region.RegionManager = this.regionManager;

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, region, 0));
            }

            public bool Remove(string regionName)
            {
                UpdateRegions();

                bool removed = false;

                IRegion region = GetRegionByName(regionName);
                if (region != null)
                {
                    removed = true;
                    this.regions.Remove(region);
                    region.RegionManager = null;

                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, region, 0));
                }

                return removed;
            }

            public bool ContainsRegionWithName(string regionName)
            {
                UpdateRegions();

                return GetRegionByName(regionName) != null;
            }

            private IRegion GetRegionByName(string regionName)
            {
                return this.regions.FirstOrDefault(r => r.Name == regionName);
            }

            private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
            {
                var handler = this.CollectionChanged;

                if (handler != null)
                {
                    handler(this, notifyCollectionChangedEventArgs);
                }
            }
        }
    }
}
