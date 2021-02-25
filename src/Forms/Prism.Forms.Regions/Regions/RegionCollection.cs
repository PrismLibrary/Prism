using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Prism.Properties;

namespace Prism.Regions
{
    internal class RegionCollection : IRegionCollection
    {
        private readonly IRegionManager regionManager;
        private readonly List<IRegion> _regions;

        public RegionCollection(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            _regions = new List<IRegion>();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<IRegion> GetEnumerator()
        {
            Xaml.RegionManager.UpdateRegions();

            return _regions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IRegion this[string regionName]
        {
            get
            {
                Xaml.RegionManager.UpdateRegions();

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

            Xaml.RegionManager.UpdateRegions();

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
            region.RegionManager = this.regionManager;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, region, 0));
        }

        public bool Remove(string regionName)
        {
            Xaml.RegionManager.UpdateRegions();

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
            Xaml.RegionManager.UpdateRegions();

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

            if (region.Name == null)
                region.Name = regionName;

            Add(region);
        }

        private IRegion GetRegionByName(string regionName) =>
            _regions.FirstOrDefault(r => r.Name == regionName);

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs) =>
            CollectionChanged?.Invoke(this, notifyCollectionChangedEventArgs);
    }
}
