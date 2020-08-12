using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using Xamarin.Forms;

namespace HelloRegions.ViewModels
{
    public class FrameDemoRegionViewModel : RegionDemoBase
    {
        public FrameDemoRegionViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
        }

        protected override string RegionName => "FrameRegion";
    }

    public class ScrollViewDemoRegionViewModel : RegionDemoBase
    {
        public ScrollViewDemoRegionViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
        }

        protected override string RegionName => "ScrollRegion";
    }
}
