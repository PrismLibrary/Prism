

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class SingleActiveRegionFixture
    {
        [TestMethod]
        public void ActivatingNewViewDeactivatesCurrent()
        {
            IRegion region = new SingleActiveRegion();
            var view = new object();
            region.Add(view);
            region.Activate(view);

            Assert.IsTrue(region.ActiveViews.Contains(view));

            var view2 = new object();
            region.Add(view2);
            region.Activate(view2);

            Assert.IsFalse(region.ActiveViews.Contains(view));
            Assert.IsTrue(region.ActiveViews.Contains(view2));
        }
    }
}