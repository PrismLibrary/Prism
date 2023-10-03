using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Xunit;

namespace Prism.Wpf.Tests.Regions
{
    public class SingleActiveRegionFixture
    {
        [Fact]
        public void ActivatingNewViewDeactivatesCurrent()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
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
