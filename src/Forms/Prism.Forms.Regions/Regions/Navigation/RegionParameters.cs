using Prism.Common;

namespace Prism.Regions.Navigation
{
    public class RegionParameters : ParametersBase, IRegionParameters
    {
        public RegionParameters()
        {
        }

        public RegionParameters(string query)
            : base(query)
        {
        }
    }
}
