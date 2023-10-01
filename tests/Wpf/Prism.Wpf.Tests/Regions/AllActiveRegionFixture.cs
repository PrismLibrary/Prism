using System;
using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Xunit;

namespace Prism.Wpf.Tests.Regions
{
    public class AllActiveRegionFixture
    {
        [Fact]
        public void AddingViewsToRegionMarksThemAsActive()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new AllActiveRegion();
            var view = new object();

            region.Add(view);

            Assert.True(region.ActiveViews.Contains(view));
        }

        [Fact]
        public void DeactivateThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                IRegion region = new AllActiveRegion();
                var view = new object();
                region.Add(view);

                region.Deactivate(view);
            });
        }
    }
}
