using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Prism.Navigation;
using Prism.Regions.Navigation;
using Xunit;

namespace Prism.Forms.Regions.Tests
{
    public class RegionNavigationJournalFixture
    {
        [Fact]
        public void ConstructingJournalInitializesValues()
        {
            // Act
            var target = new RegionNavigationJournal();

            // Verify
            Assert.False(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Null(target.CurrentEntry);
            Assert.Null(target.NavigationTarget);
        }

        [Fact]
        public void SettingNavigationServiceUpdatesValue()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var mockINavigate = new Mock<INavigateAsync>();

            // Act
            target.NavigationTarget = mockINavigate.Object;

            // Verify
            Assert.Same(mockINavigate.Object, target.NavigationTarget);
        }

        [Fact]
        public void RecordingNavigationUpdatesNavigationState()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var uri = new Uri("Uri", UriKind.Relative);
            var entry = new RegionNavigationJournalEntry() { Uri = uri };

            // Act
            target.RecordNavigation(entry, true);

            // Verify
            Assert.False(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Same(entry, target.CurrentEntry);
        }

        [Fact]
        public void RecordingNavigationMultipleTimesUpdatesNavigationState()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            // Act
            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            // Verify
            Assert.True(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Same(entry3, target.CurrentEntry);
        }

        [Fact]
        public void ClearUpdatesNavigationState()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            // Act
            target.Clear();

            // Verify
            Assert.False(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Null(target.CurrentEntry);
        }

        [Fact]
        public void GoBackNavigatesBack()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);


            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));

            // Act
            target.GoBack();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry2, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
        }

        [Fact]
        public void GoBackDoesNotChangeStateWhenNavigationFails()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);


            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, false)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));

            // Act
            target.GoBack();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Same(entry3, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
        }

        [Fact]
        public void GoBackMultipleTimesNavigatesBack()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);


            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));

            // Act
            target.GoBack();
            target.GoBack();

            // Verify
            Assert.False(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry1, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
        }

        [Fact]
        public void GoForwardNavigatesForward()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));

            target.GoBack();
            target.GoBack();

            // Act
            target.GoForward();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry2, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Exactly(2));
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
        }

        [Fact]
        public void GoForwardDoesNotChangeStateWhenNavigationFails()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, false)));

            target.GoBack();

            // Act
            target.GoForward();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry2, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
        }

        [Fact]
        public void GoForwardMultipleTimesNavigatesForward()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));

            target.GoBack();
            target.GoBack();

            // Act
            target.GoForward();
            target.GoForward();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Same(entry3, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Exactly(2));
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
        }

        [Fact]
        public void WhenNavigationToNewUri_ThenCanNoLongerNavigateForward()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            var uri4 = new Uri("Uri4", UriKind.Relative);
            var entry4 = new RegionNavigationJournalEntry() { Uri = uri4 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);

            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));

            target.GoBack();

            Assert.True(target.CanGoForward);

            // Act
            target.RecordNavigation(entry3, true);

            // Verify
            Assert.False(target.CanGoForward);
            Assert.Equal(entry3, target.CurrentEntry);
        }

        [Fact]
        public void WhenSavePreviousFalseDoNotRecordEntry()
        {
            // Prepare
            var target = new RegionNavigationJournal();

            var mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            var uri1 = new Uri("Uri1", UriKind.Relative);
            var entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            var uri2 = new Uri("Uri2", UriKind.Relative);
            var entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            var uri3 = new Uri("Uri3", UriKind.Relative);
            var entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            var uri4 = new Uri("Uri4", UriKind.Relative);
            var entry4 = new RegionNavigationJournalEntry() { Uri = uri4 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, false);
            target.RecordNavigation(entry4, true);

            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri4, It.IsAny<Action<IRegionNavigationResult>>(), null))
                .Callback<Uri, Action<IRegionNavigationResult>, INavigationParameters>((u, c, n) => c(new RegionNavigationResult(null, true)));

            Assert.Equal(entry4, target.CurrentEntry);

            target.GoBack();

            Assert.True(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry2, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri4, It.IsAny<Action<IRegionNavigationResult>>(), null), Times.Never());
        }
    }
}
