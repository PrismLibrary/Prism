using System;
using System.Collections.Generic;
using Prism.Navigation;
using Prism.Regions;

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

            if (ActiveRegion is INavigationServiceAware nsa && nsa.NavigationService != null)
            {
                return new List<(Type Type, object Instance)>
                {
                    (typeof(INavigationService), nsa.NavigationService)
                };
            }

            return Array.Empty<(Type Type, object Instance)>(); ;
        }
    }
}
