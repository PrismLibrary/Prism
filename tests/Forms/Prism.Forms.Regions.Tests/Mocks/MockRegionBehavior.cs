using System;
using Prism.Regions;
using Prism.Regions.Behaviors;

namespace Prism.Forms.Regions.Mocks
{
    public class MockRegionBehavior : IRegionBehavior
    {
        public IRegion Region { get; set; }

        public Func<object> OnAttach;

        public void Attach()
        {
            if (OnAttach != null)
                OnAttach();
        }
    }
}
