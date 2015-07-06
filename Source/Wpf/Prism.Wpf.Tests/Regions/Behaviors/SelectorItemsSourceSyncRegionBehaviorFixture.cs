

using System;
using System.Collections;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    [TestClass]
    public class SelectorItemsSourceSyncRegionBehaviorFixture
    {
        [TestMethod]
        public void CanAttachToSelector()
        {
            SelectorItemsSourceSyncBehavior behavior = CreateBehavior();
            behavior.Attach();

            Assert.IsTrue(behavior.IsAttached);
        }

        [TestMethod]
        public void AttachSetsItemsSourceOfSelector()
        {
            SelectorItemsSourceSyncBehavior behavior = CreateBehavior();

            var v1 = new Button();
            var v2 = new Button();

            behavior.Region.Add(v1);
            behavior.Region.Add(v2);

            behavior.Attach();

            Assert.AreEqual(2, (behavior.HostControl as Selector).Items.Count);
        }

        [TestMethod]
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
            
            Assert.AreEqual(3, (behavior.HostControl as Selector).Items.Count);

            Assert.AreSame(v1, (behavior.HostControl as Selector).Items[0]);
            Assert.AreSame(v2, (behavior.HostControl as Selector).Items[1]);
            Assert.AreSame(v3, (behavior.HostControl as Selector).Items[2]);
        }


        [TestMethod]
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

            Assert.AreEqual(1, activeViews.Count());
            Assert.AreEqual(v1, activeViews.First());

            (behavior.HostControl as Selector).SelectedItem = v2;

            Assert.AreEqual(1, activeViews.Count());
            Assert.AreEqual(v2, activeViews.First());
        }

        [TestMethod]
        public void ActiveViewChangedShouldChangeSelectedItem()
        {
            SelectorItemsSourceSyncBehavior behavior = CreateBehavior();

            var v1 = new Button();
            var v2 = new Button();

            behavior.Region.Add(v1);
            behavior.Region.Add(v2);

            behavior.Attach();

            behavior.Region.Activate(v1);
            Assert.AreEqual(v1, (behavior.HostControl as Selector).SelectedItem);

            behavior.Region.Activate(v2);
            Assert.AreEqual(v2, (behavior.HostControl as Selector).SelectedItem);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItemsSourceSetThrows()
        {
            SelectorItemsSourceSyncBehavior behavior = CreateBehavior();

            (behavior.HostControl as Selector).ItemsSource = new[] {new Button()};

            behavior.Attach();
        }

        [TestMethod]
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
                Assert.IsInstanceOfType(ex, typeof(InvalidOperationException));
                StringAssert.Contains(ex.Message, "ItemsControl's ItemsSource property is not empty.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddingViewToTwoRegionsThrows()
        {
            var behavior1 = CreateBehavior();
            var behavior2 = CreateBehavior();

            behavior1.Attach();
            behavior2.Attach();
            var v1 = new Button();

            behavior1.Region.Add(v1);
            behavior2.Region.Add(v1);
        }

        [TestMethod]
        public void ReactivatingViewAddsViewToTab()
        {
            var behavior1 = CreateBehavior();
            behavior1.Attach();

            var v1 = new Button();
            var v2 = new Button();

            behavior1.Region.Add(v1);
            behavior1.Region.Add(v2);

            behavior1.Region.Activate(v1);
            Assert.IsTrue(behavior1.Region.ActiveViews.First() == v1);

            behavior1.Region.Activate(v2);
            Assert.IsTrue(behavior1.Region.ActiveViews.First() == v2);

            behavior1.Region.Activate(v1);
            Assert.IsTrue(behavior1.Region.ActiveViews.First() == v1);
        }

        [TestMethod]
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

            Assert.IsTrue(behavior1.Region.ActiveViews.Contains(v1));
            Assert.IsTrue(behavior1.Region.ActiveViews.Contains(v2));

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
