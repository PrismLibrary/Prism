

using Prism.Properties;
using Prism.Regions.Behaviors;
using System;
using System.Globalization;
using System.Windows;

namespace Prism.Regions
{
    /// <summary>
    /// Base class to facilitate the creation of <see cref="IRegionAdapter"/> implementations.
    /// </summary>
    /// <typeparam name="T">Type of object to adapt.</typeparam>
    public abstract class RegionAdapterBase<T> : IRegionAdapter where T : class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RegionAdapterBase{T}"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        protected RegionAdapterBase(IRegionBehaviorFactory regionBehaviorFactory)
        {
            this.RegionBehaviorFactory = regionBehaviorFactory;
        }

        /// <summary>
        /// Gets or sets the factory used to create the region behaviors to attach to the created regions.
        /// </summary>
        protected IRegionBehaviorFactory RegionBehaviorFactory { get; set; }

        /// <summary>
        /// Adapts an object and binds it to a new <see cref="IRegion"/>.
        /// </summary>
        /// <param name="regionTarget">The object to adapt.</param>
        /// <param name="regionName">The name of the region to be created.</param>
        /// <returns>The new instance of <see cref="IRegion"/> that the <paramref name="regionTarget"/> is bound to.</returns>
        public IRegion Initialize(T regionTarget, string regionName)
        {
            if (regionName == null)
                throw new ArgumentNullException(nameof(regionName));

            IRegion region = this.CreateRegion();
            region.Name = regionName;

            SetObservableRegionOnHostingControl(region, regionTarget);

            this.Adapt(region, regionTarget);
            this.AttachBehaviors(region, regionTarget);
            this.AttachDefaultBehaviors(region, regionTarget);
            return region;
        }

        /// <summary>
        /// Adapts an object and binds it to a new <see cref="IRegion"/>.
        /// </summary>
        /// <param name="regionTarget">The object to adapt.</param>
        /// <param name="regionName">The name of the region to be created.</param>
        /// <returns>The new instance of <see cref="IRegion"/> that the <paramref name="regionTarget"/> is bound to.</returns>
        /// <remarks>This methods performs validation to check that <paramref name="regionTarget"/>
        /// is of type <typeparamref name="T"/>.</remarks>
        /// <exception cref="ArgumentNullException">When <paramref name="regionTarget"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">When <paramref name="regionTarget"/> is not of type <typeparamref name="T"/>.</exception>
        IRegion IRegionAdapter.Initialize(object regionTarget, string regionName)
        {
            return this.Initialize(GetCastedObject(regionTarget), regionName);
        }

        /// <summary>
        /// This method adds the default behaviors by using the <see cref="IRegionBehaviorFactory"/> object.
        /// </summary>
        /// <param name="region">The region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected virtual void AttachDefaultBehaviors(IRegion region, T regionTarget)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            IRegionBehaviorFactory behaviorFactory = this.RegionBehaviorFactory;
            if (behaviorFactory != null)
            {
                DependencyObject dependencyObjectRegionTarget = regionTarget as DependencyObject;

                foreach (string behaviorKey in behaviorFactory)
                {
                    if (!region.Behaviors.ContainsKey(behaviorKey))
                    {
                        IRegionBehavior behavior = behaviorFactory.CreateFromKey(behaviorKey);

                        if (dependencyObjectRegionTarget != null)
                        {
                            IHostAwareRegionBehavior hostAwareRegionBehavior = behavior as IHostAwareRegionBehavior;
                            if (hostAwareRegionBehavior != null)
                            {
                                hostAwareRegionBehavior.HostControl = dependencyObjectRegionTarget;
                            }
                        }

                        region.Behaviors.Add(behaviorKey, behavior);
                    }
                }
            }
        }

        /// <summary>
        /// Template method to attach new behaviors.
        /// </summary>
        /// <param name="region">The region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected virtual void AttachBehaviors(IRegion region, T regionTarget)
        {
        }

        /// <summary>
        /// Template method to adapt the object to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected abstract void Adapt(IRegion region, T regionTarget);

        /// <summary>
        /// Template method to create a new instance of <see cref="IRegion"/>
        /// that will be used to adapt the object.
        /// </summary>
        /// <returns>A new instance of <see cref="IRegion"/>.</returns>
        protected abstract IRegion CreateRegion();

        private static T GetCastedObject(object regionTarget)
        {
            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            T castedObject = regionTarget as T;

            if (castedObject == null)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.AdapterInvalidTypeException, typeof(T).Name));

            return castedObject;
        }

        private static void SetObservableRegionOnHostingControl(IRegion region, T regionTarget)
        {
            DependencyObject targetElement = regionTarget as DependencyObject;

            if (targetElement != null)
            {
                // Set the region as a dependency property on the control hosting the region
                // Because we are using an observable region, the hosting control can detect that the
                // region has actually been created. This is an ideal moment to hook up custom behaviors
                RegionManager.GetObservableRegion(targetElement).Value = region;
            }
        }
    }
}
