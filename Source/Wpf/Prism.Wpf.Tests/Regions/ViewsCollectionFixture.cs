

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using Xunit;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    
    public class ViewsCollectionFixture
    {
        [Fact]
        public void CanWrapCollectionCollection()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, x => true);

            Assert.Empty(viewsCollection);

            var item = new object();
            originalCollection.Add(new ItemMetadata(item));
            Assert.Single(viewsCollection);
            Assert.Same(item, viewsCollection.First());
        }

        [Fact]
        public void CanFilterCollection()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, x => x.Name == "Possible");

            originalCollection.Add(new ItemMetadata(new object()));

            Assert.Empty(viewsCollection);

            var item = new object();
            originalCollection.Add(new ItemMetadata(item) {Name = "Possible"});
            Assert.Single(viewsCollection);

            Assert.Same(item, viewsCollection.First());
        }

        [Fact]
        public void RaisesCollectionChangedWhenFilteredCollectionChanges()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, x => x.IsActive);
            bool collectionChanged = false;
            viewsCollection.CollectionChanged += (s, e) => collectionChanged = true;

            originalCollection.Add(new ItemMetadata(new object()) {IsActive = true});

            Assert.True(collectionChanged);
        }

        [Fact]
        public void RaisesCollectionChangedWithAddAndRemoveWhenFilteredCollectionChanges()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, x => x.IsActive);
            bool addedToCollection = false;
            bool removedFromCollection = false;
            viewsCollection.CollectionChanged += (s, e) =>
                                                     {
                                                         if (e.Action == NotifyCollectionChangedAction.Add)
                                                         {
                                                             addedToCollection = true;
                                                         }
                                                         else if (e.Action == NotifyCollectionChangedAction.Remove)
                                                         {
                                                             removedFromCollection = true;
                                                         }
                                                     };
            var filteredInObject = new ItemMetadata(new object()) {IsActive = true};

            originalCollection.Add(filteredInObject);

            Assert.True(addedToCollection);
            Assert.False(removedFromCollection);

            originalCollection.Remove(filteredInObject);

            Assert.True(removedFromCollection);
        }

        [Fact]
        public void DoesNotRaiseCollectionChangedWhenAddingOrRemovingFilteredOutObject()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, x => x.IsActive);
            bool collectionChanged = false;
            viewsCollection.CollectionChanged += (s, e) => collectionChanged = true;
            var filteredOutObject = new ItemMetadata(new object()) {IsActive = false};

            originalCollection.Add(filteredOutObject);
            originalCollection.Remove(filteredOutObject);

            Assert.False(collectionChanged);
        }

        [Fact]
        public void CollectionChangedPassesWrappedItemInArgumentsWhenAdding()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            var filteredInObject = new ItemMetadata(new object());
            originalCollection.Add(filteredInObject);

            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, x => true);
            IList oldItemsPassed = null;
            viewsCollection.CollectionChanged += (s, e) => { oldItemsPassed = e.OldItems; };
            originalCollection.Remove(filteredInObject);

            Assert.NotNull(oldItemsPassed);
            Assert.Equal(1, oldItemsPassed.Count);
            Assert.Same(filteredInObject.Item, oldItemsPassed[0]);
        }

        [Fact]
        public void CollectionChangedPassesWrappedItemInArgumentsWhenRemoving()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, x => true);
            IList newItemsPassed = null;
            viewsCollection.CollectionChanged += (s, e) => { newItemsPassed = e.NewItems; };
            var filteredInObject = new ItemMetadata(new object());

            originalCollection.Add(filteredInObject);

            Assert.NotNull(newItemsPassed);
            Assert.Equal(1, newItemsPassed.Count);
            Assert.Same(filteredInObject.Item, newItemsPassed[0]);
        }

        [Fact]
        public void EnumeratesWrappedItems()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>()
                                         {
                                             new ItemMetadata(new object()),
                                             new ItemMetadata(new object())
                                         };
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, x => true);
            Assert.Equal(2, viewsCollection.Count());

            Assert.Same(originalCollection[0].Item, viewsCollection.ElementAt(0));
            Assert.Same(originalCollection[1].Item, viewsCollection.ElementAt(1));
        }

        [Fact]
        public void ChangingMetadataOnItemAddsOrRemovesItFromTheFilteredCollection()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, x => x.IsActive);
            bool addedToCollection = false;
            bool removedFromCollection = false;
            viewsCollection.CollectionChanged += (s, e) =>
                                                     {
                                                         if (e.Action == NotifyCollectionChangedAction.Add)
                                                         {
                                                             addedToCollection = true;
                                                         }
                                                         else if (e.Action == NotifyCollectionChangedAction.Remove)
                                                         {
                                                             removedFromCollection = true;
                                                         }
                                                     };

            originalCollection.Add(new ItemMetadata(new object()) {IsActive = true});
            Assert.True(addedToCollection);
            Assert.False(removedFromCollection);
            addedToCollection = false;

            originalCollection[0].IsActive = false;

            Assert.Empty(viewsCollection);
            Assert.True(removedFromCollection);
            Assert.False(addedToCollection);
            Assert.Empty(viewsCollection);
            addedToCollection = false;
            removedFromCollection = false;

            originalCollection[0].IsActive = true;

            Assert.Single(viewsCollection);
            Assert.True(addedToCollection);
            Assert.False(removedFromCollection);
        }

        [Fact]
        public void AddingToOriginalCollectionFiresAddCollectionChangeEvent()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, (i) => true);

            var eventTracker = new CollectionChangedTracker(viewsCollection);

            originalCollection.Add(new ItemMetadata(new object()));

            Assert.Contains(NotifyCollectionChangedAction.Add, eventTracker.ActionsFired);
        }

        [Fact]
        public void AddingToOriginalCollectionFiresResetNotificationIfSortComparisonSet()
        {
            // Reset is fired to support the need to resort after updating the collection
            var originalCollection = new ObservableCollection<ItemMetadata>();
            var viewsCollection = new ViewsCollection(originalCollection, (i) => true);
            viewsCollection.SortComparison = (a, b) => { return 0; };

            var eventTracker = new CollectionChangedTracker(viewsCollection);

            originalCollection.Add(new ItemMetadata(new object()));

            Assert.Contains(NotifyCollectionChangedAction.Add, eventTracker.ActionsFired);
            Assert.Equal(
                1,
                eventTracker.ActionsFired.Count(a => a == NotifyCollectionChangedAction.Reset));
        }

        [Fact]
        public void OnAddNotifyCollectionChangedThenIndexProvided()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, (i) => true);

            var eventTracker = new CollectionChangedTracker(viewsCollection);

            originalCollection.Add(new ItemMetadata("a"));

            var addEvent = eventTracker.NotifyEvents.Single(e => e.Action == NotifyCollectionChangedAction.Add);
            Assert.Equal(0, addEvent.NewStartingIndex);
        }

        [Fact]
        public void OnRemoveNotifyCollectionChangedThenIndexProvided()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            originalCollection.Add(new ItemMetadata("a"));
            originalCollection.Add(new ItemMetadata("b"));
            originalCollection.Add(new ItemMetadata("c"));
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, (i) => true);

            var eventTracker = new CollectionChangedTracker(viewsCollection);
            originalCollection.RemoveAt(1);

            var removeEvent = eventTracker.NotifyEvents.Single(e => e.Action == NotifyCollectionChangedAction.Remove);
            Assert.NotNull(removeEvent);
            Assert.Equal(1, removeEvent.OldStartingIndex);
        }

        [Fact]
        public void OnRemoveOfFilterMatchingItemThenViewCollectionRelativeIndexProvided()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            originalCollection.Add(new ItemMetadata("a"));
            originalCollection.Add(new ItemMetadata("b"));
            originalCollection.Add(new ItemMetadata("c"));
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, (i) => !"b".Equals(i.Item));

            var eventTracker = new CollectionChangedTracker(viewsCollection);
            originalCollection.RemoveAt(2);

            var removeEvent = eventTracker.NotifyEvents.Single(e => e.Action == NotifyCollectionChangedAction.Remove);
            Assert.NotNull(removeEvent);
            Assert.Equal(1, removeEvent.OldStartingIndex);
        }


        [Fact]
        public void RemovingFromFilteredCollectionDoesNotThrow()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            originalCollection.Add(new ItemMetadata("a"));
            originalCollection.Add(new ItemMetadata("b"));
            originalCollection.Add(new ItemMetadata("c"));
            IViewsCollection viewsCollection = new ViewsCollection(originalCollection, (i) => true);

            CollectionViewSource cvs = new CollectionViewSource {Source = viewsCollection};

            var view = cvs.View;
            //try
            //{
                originalCollection.RemoveAt(1);
            //}
            //catch (Exception ex)
            //{
                //Assert.Fail(ex.Message);
            //}
        }

        [Fact]
        public void ViewsCollectionSortedAfterAddingItemToOriginalCollection()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            ViewsCollection viewsCollection = new ViewsCollection(originalCollection, (i) => true);
            viewsCollection.SortComparison = Region.DefaultSortComparison;

            var view1 = new MockSortableView1();
            var view2 = new MockSortableView2();
            var view3 = new MockSortableView3();

            originalCollection.Add(new ItemMetadata(view2));
            originalCollection.Add(new ItemMetadata(view3));
            originalCollection.Add(new ItemMetadata(view1));

            Assert.Same(view1, viewsCollection.ElementAt(0));
            Assert.Same(view2, viewsCollection.ElementAt(1));
            Assert.Same(view3, viewsCollection.ElementAt(2));
        }

        [Fact]
        public void ChangingSortComparisonCausesResortingOfCollection()
        {
            var originalCollection = new ObservableCollection<ItemMetadata>();
            ViewsCollection viewsCollection = new ViewsCollection(originalCollection, (i) => true);

            var view1 = new MockSortableView1();
            var view2 = new MockSortableView2();
            var view3 = new MockSortableView3();

            originalCollection.Add(new ItemMetadata(view2));
            originalCollection.Add(new ItemMetadata(view3));
            originalCollection.Add(new ItemMetadata(view1));

            // ensure items are in original order
            Assert.Same(view2, viewsCollection.ElementAt(0));
            Assert.Same(view3, viewsCollection.ElementAt(1));
            Assert.Same(view1, viewsCollection.ElementAt(2));

            // change sort comparison
            viewsCollection.SortComparison = Region.DefaultSortComparison;

            // ensure items are properly sorted
            Assert.Same(view1, viewsCollection.ElementAt(0));
            Assert.Same(view2, viewsCollection.ElementAt(1));
            Assert.Same(view3, viewsCollection.ElementAt(2));
        }
    }
}