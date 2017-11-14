

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class RegionNavigationJournalFixture
    {
        [TestMethod]
        public void ConstructingJournalInitializesValues()
        {
            // Act
            RegionNavigationJournal target = new RegionNavigationJournal();

            // Verify
            Assert.IsFalse(target.CanGoBack);
            Assert.IsFalse(target.CanGoForward);
            Assert.IsNull(target.CurrentEntry);
            Assert.IsNull(target.NavigationTarget);
        }

        [TestMethod]
        public void SettingNavigationServiceUpdatesValue()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Mock<INavigateAsync> mockINavigate = new Mock<INavigateAsync>();

            // Act
            target.NavigationTarget = mockINavigate.Object;

            // Verify
            Assert.AreSame(mockINavigate.Object, target.NavigationTarget);
        }

        [TestMethod]
        public void RecordingNavigationUpdatesNavigationState()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Uri uri = new Uri("Uri", UriKind.Relative);
            RegionNavigationJournalEntry entry = new RegionNavigationJournalEntry() { Uri = uri };

            // Act
            target.RecordNavigation(entry, true);

            // Verify
            Assert.IsFalse(target.CanGoBack);
            Assert.IsFalse(target.CanGoForward);
            Assert.AreSame(entry, target.CurrentEntry);
        }

        [TestMethod]
        public void RecordingNavigationMultipleTimesUpdatesNavigationState()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            // Act
            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            // Verify
            Assert.IsTrue(target.CanGoBack);
            Assert.IsFalse(target.CanGoForward);
            Assert.AreSame(entry3, target.CurrentEntry);
        }

        [TestMethod]
        public void ClearUpdatesNavigationState()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            // Act
            target.Clear();

            // Verify
            Assert.IsFalse(target.CanGoBack);
            Assert.IsFalse(target.CanGoForward);
            Assert.IsNull(target.CurrentEntry);
        }

        [TestMethod]
        public void GoBackNavigatesBack()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Mock<INavigateAsync> mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);


            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));

            // Act
            target.GoBack();

            // Verify
            Assert.IsTrue(target.CanGoBack);
            Assert.IsTrue(target.CanGoForward);
            Assert.AreSame(entry2, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null), Times.Never());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null), Times.Never());
        }

        [TestMethod]
        public void GoBackDoesNotChangeStateWhenNavigationFails()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Mock<INavigateAsync> mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);


            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, false)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));

            // Act
            target.GoBack();

            // Verify
            Assert.IsTrue(target.CanGoBack);
            Assert.IsFalse(target.CanGoForward);
            Assert.AreSame(entry3, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null), Times.Never());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null), Times.Never());
        }

        [TestMethod]
        public void GoBackMultipleTimesNavigatesBack()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Mock<INavigateAsync> mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);


            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));

            // Act
            target.GoBack();
            target.GoBack();

            // Verify
            Assert.IsFalse(target.CanGoBack);
            Assert.IsTrue(target.CanGoForward);
            Assert.AreSame(entry1, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null), Times.Never());
        }

        [TestMethod]
        public void GoForwardNavigatesForward()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Mock<INavigateAsync> mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));

            target.GoBack();
            target.GoBack();

            // Act
            target.GoForward();

            // Verify
            Assert.IsTrue(target.CanGoBack);
            Assert.IsTrue(target.CanGoForward);
            Assert.AreSame(entry2, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null), Times.Exactly(2));
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null), Times.Never());
        }

        [TestMethod]
        public void GoForwardDoesNotChangeStateWhenNavigationFails()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Mock<INavigateAsync> mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, false)));

            target.GoBack();

            // Act
            target.GoForward();

            // Verify
            Assert.IsTrue(target.CanGoBack);
            Assert.IsTrue(target.CanGoForward);
            Assert.AreSame(entry2, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null), Times.Never());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
        }

        [TestMethod]
        public void GoForwardMultipleTimesNavigatesForward()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Mock<INavigateAsync> mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));

            target.GoBack();
            target.GoBack();

            // Act
            target.GoForward();
            target.GoForward();

            // Verify
            Assert.IsTrue(target.CanGoBack);
            Assert.IsFalse(target.CanGoForward);
            Assert.AreSame(entry3, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null), Times.Exactly(2));
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
        }

        [TestMethod]
        public void WhenNavigationToNewUri_ThenCanNoLongerNavigateForward()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Mock<INavigateAsync> mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>()))
                .Callback<Uri, Action<NavigationResult>>((u, c) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>()))
                .Callback<Uri, Action<NavigationResult>>((u, c) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>()))
                .Callback<Uri, Action<NavigationResult>>((u, c) => c(new NavigationResult(null, true)));

            target.GoBack();

            // Act
            target.RecordNavigation(new RegionNavigationJournalEntry() { Uri = new Uri("Uri4", UriKind.Relative) }, true);


            // Verify
            Assert.IsFalse(target.CanGoForward);
        }

        [TestMethod]
        public void WhenSavePreviousFalseDoNotRecordEntry()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Mock<INavigateAsync> mockNavigationTarget = new Mock<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget.Object;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, false);


            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));
            mockNavigationTarget
                .Setup(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null))
                .Callback<Uri, Action<NavigationResult>, NavigationParameters>((u, c, n) => c(new NavigationResult(null, true)));

            // Act
            target.GoBack();

            // Verify
            Assert.IsFalse(target.CanGoBack);
            Assert.IsTrue(target.CanGoForward);
            Assert.AreSame(entry1, target.CurrentEntry);

            mockNavigationTarget.Verify(x => x.RequestNavigate(uri1, It.IsAny<Action<NavigationResult>>(), null), Times.Once());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri2, It.IsAny<Action<NavigationResult>>(), null), Times.Never());
            mockNavigationTarget.Verify(x => x.RequestNavigate(uri3, It.IsAny<Action<NavigationResult>>(), null), Times.Never());
        }
    }
}
