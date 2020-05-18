using System;
using System.Collections.Generic;
using System.Text;
using Prism.Navigation;
using Prism.Regions;
using Prism.Regions.Navigation;

namespace Prism.Ioc
{
    internal class RegionResolverOverrides : IResolverOverridesHelper, IActiveRegionHelper
    {
        public IRegion ActiveRegion { get; set; }

        public IEnumerable<(Type Type, object Instance)> GetOverrides()
        {
            if (ActiveRegion == null)
            {
                return Array.Empty<(Type Type, object Instance)>();
            }

            var overrides = new List<(Type Type, object Instance)>
            {
                (typeof(IRegionNavigationService), ActiveRegion.NavigationService)
            };

            if (ActiveRegion is INavigationServiceAware nsa && nsa.NavigationService != null)
            {
                overrides.Add((typeof(INavigationService), nsa.NavigationService));
            }

            return overrides;
        }
    }
}
