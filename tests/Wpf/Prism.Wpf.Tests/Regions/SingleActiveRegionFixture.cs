

using Xunit;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    
    public class SingleActiveRegionFixture
    {
        [Fact]
        public void ActivatingNewViewDeactivatesCurrent()
        {
            IRegion region = new SingleActiveRegion();
            var view = new object();
            region.Add(view);
            region.Activate(view);

            Assert.True(region.ActiveViews.Contains(view));

            var view2 = new object();
            region.Add(view2);
            region.Activate(view2);

            Assert.False(region.ActiveViews.Contains(view));
            Assert.True(region.ActiveViews.Contains(view2));
        }
    }
}