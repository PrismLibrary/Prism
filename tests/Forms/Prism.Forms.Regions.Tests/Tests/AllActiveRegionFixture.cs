using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Regions.Tests
{
    public class AllActiveRegionFixture
    {
        [Fact]
        public void AddingViewsToRegionMarksThemAsActive()
        {
            IRegion region = new AllActiveRegion();
            var view = new ContentView();

            region.Add(view);

            Assert.True(region.ActiveViews.Contains(view));
        }

        [Fact]
        public void DeactivateThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                IRegion region = new AllActiveRegion();
                var view = new ContentView();
                region.Add(view);

                region.Deactivate(view);
            });
        }
    }
}
