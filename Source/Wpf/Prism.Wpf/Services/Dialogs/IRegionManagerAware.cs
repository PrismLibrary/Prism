using Prism.Regions;

namespace Prism.Services.Dialogs
{
    public interface IRegionManagerAware
    {
        IRegionManager RegionManager { get; set; }
    }
}