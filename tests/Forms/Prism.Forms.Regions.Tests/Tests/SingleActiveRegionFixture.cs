using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Regions.Tests
{
    public class SingleActiveRegionFixture
    {
        [Fact]
        public void ActivatingNewViewDeactivatesCurrent()
        {
            IRegion region = new SingleActiveRegion();
            var view = new ContentView();
            region.Add(view);
            region.Activate(view);

            Assert.True(region.ActiveViews.Contains(view));

            var view2 = new StackLayout();
            region.Add(view2);
            region.Activate(view2);

            Assert.False(region.ActiveViews.Contains(view));
            Assert.True(region.ActiveViews.Contains(view2));
        }
    }
}
