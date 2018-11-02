

using System;
using System.Collections;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Xunit;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    
    public class SelectorItemsSourceSyncRegionBehaviorFixture
    {
        [StaFact]
        public void CanAttachToSelector()
        {
            SelectorItemsSourceSyncBehavior behavior = CreateBehavior();
            behavior.Attach();

            Assert.True(behavior.IsAttached);
        }

        [StaFact]
        public void AttachSetsItemsSourceOfSelector()
        {
            SelectorItemsSourceSyncBehavior behavior = CreateBehavior();

            var v1 = new Button();
            var v2 = new Button();

            behavior.Region.Add(v1);
            behavior.Region.Add(v2);

            behavior.Attach();

            Assert.Equal(2, (behavior.HostControl as Selector).Items.Count);
        }

        [StaFact]
        public void IfViewsHaveSortHintThenViewsAreProperlySorted()
        {
            SelectorItemsSourceSyncBehavior behavior = CreateBehavior();

            var v1 = new MockSortableView1();
            var v2 = new MockSortableView2();
            var v3 = new MockSortableView3();
            behavior.Attach();

            behavior.Region.Add(v3);
            behavior.Region.Add(v2);
            behavior.Region.Add(v1);
            
            Assert.Equal(3, (behavior.HostControl as Selector).Items.Count);

            Assert.Same(v1, (behavior.HostControl as Selector).Items[0]);
            Assert.Same(v2, (behavior.HostControl as Selector).Items[1]);
            Assert.Same(v3, (behavior.HostControl as Selector).Items[2]);
        }


        [StaFact]
        public void SelectionChangedShouldChangeActiveViews()
        {
            SelectorItemsSourceSyncBehavior behavior = CreateBehavior();

            var v1 = new Button();
            var v2 = new Button();

            behavior.Region.Add(v1);
            behavior.Region.Add(v2);

            behavior.Attach();

            (behavior.HostControl as Selector).SelectedItem = v1;
            var activeViews = behavior.Region.ActiveViews;

            Assert.Single(activeViews);
            Assert.Equal(v1, activeViews.First());

            (behavior.HostControl as Selector).SelectedItem = v2;

            Assert.Single(activeViews);
            Assert.Equal(v2, activeViews.First());
        }

        [StaFact]
        public void ActiveViewChangedShouldChangeSelectedItem()
        {
            SelectorItemsSourceSyncBehavior behavior = CreateBehavior();

            var v1 = new Button();
            var v2 = new Button();

            behavior.Region.Add(v1);
            behavior.Region.Add(v2);

            behavior.Attach();

            behavior.Region.Activate(v1);
            Assert.Equal(v1, (behavior.HostControl as Selector).SelectedItem);

            behavior.Region.Activate(v2);
            Assert.Equal(v2, (behavior.HostControl as Selector).SelectedItem);
        }

        [StaFact]
        public void ItemsSourceSetThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                SelectorItemsSourceSyncBehavior behavior = CreateBehavior();

                (behavior.HostControl as Selector).ItemsSource = new[] { new Button() };

                behavior.Attach();
            });

        }

        [StaFact]
        public void ControlWithExistingBindingOnItemsSourceWithNullValueThrows()
        {
            var behavor = CreateBehavior();

            Binding binding = new Binding("Enumerable");
            binding.Source = new SimpleModel() { Enumerable = null };
            (behavor.HostControl as Selector).SetBinding(ItemsControl.ItemsSourceProperty, binding);

            try
            {
                behavor.Attach();

            }
            catch (Exception ex)
            {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("ItemsControl's ItemsSource property is not empty.", ex.Message);
            }
        }

        [StaFact]
        public void AddingViewToTwoRegionsThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var behavior1 = CreateBehavior();
                var behavior2 = CreateBehavior();

                behavior1.Attach();
                behavior2.Attach();
                var v1 = new Button();

                behavior1.Region.Add(v1);
                behavior2.Region.Add(v1);
            });

        }

        [StaFact]
        public void ReactivatingViewAddsViewToTab()
        {
            var behavior1 = CreateBehavior();
            behavior1.Attach();

            var v1 = new Button();
            var v2 = new Button();

            behavior1.Region.Add(v1);
            behavior1.Region.Add(v2);

            behavior1.Region.Activate(v1);
            Assert.True(behavior1.Region.ActiveViews.First() == v1);

            behavior1.Region.Activate(v2);
            Assert.True(behavior1.Region.ActiveViews.First() == v2);

            behavior1.Region.Activate(v1);
            Assert.True(behavior1.Region.ActiveViews.First() == v1);
        }

        [StaFact]
        public void ShouldAllowMultipleSelectedItemsForListBox()
        {
            var behavior1 = CreateBehavior();
            ListBox listBox = new ListBox();
            listBox.SelectionMode = SelectionMode.Multiple;
            behavior1.HostControl = listBox;
            behavior1.Attach();

            var v1 = new Button();
            var v2 = new Button();

            behavior1.Region.Add(v1);
            behavior1.Region.Add(v2);

            listBox.SelectedItems.Add(v1);
            listBox.SelectedItems.Add(v2);

            Assert.True(behavior1.Region.ActiveViews.Contains(v1));
            Assert.True(behavior1.Region.ActiveViews.Contains(v2));

        }

        private SelectorItemsSourceSyncBehavior CreateBehavior()
        {
            Region region = new Region();
            Selector selector = new TabControl();

            var behavior = new SelectorItemsSourceSyncBehavior();
            behavior.HostControl = selector;
            behavior.Region = region;
            return behavior;
        }

        private class SimpleModel
        {
            public IEnumerable Enumerable { get; set; }
        }
    }
}
