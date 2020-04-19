namespace Prism.Regions
{
    public partial class RegionManager : IRegionManager
    {
        public IRegionCollection Regions { get; }

        public IRegionManager CreateRegionManager()
        {
            throw new System.NotImplementedException();
        }
    }
}
