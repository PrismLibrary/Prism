using Prism.Modularity;
using Prism.Regions;
using System;

namespace $safeprojectname$
{
    public class $safeprojectname$Module : IModule
    {
        IRegionManager _regionManager;

        public $safeprojectname$Module(RegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}