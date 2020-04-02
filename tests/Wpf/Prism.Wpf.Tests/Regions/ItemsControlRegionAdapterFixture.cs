

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Xunit;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    
    public class ItemsControlRegionAdapterFixture
    {
        [StaFact]
        public void AdapterAssociatesItemsControlWithRegion()
        {
            var control = new ItemsControl();
            IRegionAdapter adapter = new TestableItemsControlRegionAdapter();

            IRegion region = adapter.Initialize(control, "Region1");
            Assert.NotNull(region);

            Assert.Same(control.ItemsSource, region.Views);
        }

        [StaFact]
        public void AdapterAssignsARegionThatHasAllViewsActive()
        {
            var control = new ItemsControl();
            IRegionAdapter adapter = new ItemsControlRegionAdapter(null);

            IRegion region = adapter.Initialize(control, "Region1");
            Assert.NotNull(region);
            Assert.IsType<AllActiveRegion>(region);
        }


        [StaFact]
        public void ShouldMoveAlreadyExistingContentInControlToRegion()
        {
            var control = new ItemsControl();
            var view = new object();
            control.Items.Add(view);
            IRegionAdapter adapter = new TestableItemsControlRegionAdapter();

            var region = (MockPresentationRegion)adapter.Initialize(control, "Region1");

            Assert.Single(region.MockViews);
            Assert.Same(view, region.MockViews.ElementAt(0));
            Assert.Same(view, control.Items[0]);
        }

        [StaFact]
        public void ControlWithExistingItemSourceThrows()
        {
            var control = new ItemsControl() { ItemsSource = new List<string>() };

            IRegionAdapter adapter = new TestableItemsControlRegionAdapter();

            try
            {
                var region = (MockPresentationRegion)adapter.Initialize(control, "Region1");
                //Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("ItemsControl's ItemsSource property is not empty.", ex.Message);
            }
        }

        [StaFact]
        public void ControlWithExistingBindingOnItemsSourceWithNullValueThrows()
        {
            var control = new ItemsControl();
            Binding binding = new Binding("Enumerable");
            binding.Source = new SimpleModel() { Enumerable = null };
            control.SetBinding(ItemsControl.ItemsSourceProperty, binding);

            IRegionAdapter adapter = new TestableItemsControlRegionAdapter();

            try
            {
                var region = (MockPresentationRegion)adapter.Initialize(control, "Region1");
                //Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("ItemsControl's ItemsSource property is not empty.", ex.Message);
            }
        }

        class SimpleModel
        {
            public IEnumerable Enumerable { get; set; }
        }

        private class TestableItemsControlRegionAdapter : ItemsControlRegionAdapter
        {
            public TestableItemsControlRegionAdapter() : base (null)
            {
            }

            private MockPresentationRegion region = new MockPresentationRegion();

            protected override IRegion CreateRegion()
            {
                return region;
            }
        }
    }
}