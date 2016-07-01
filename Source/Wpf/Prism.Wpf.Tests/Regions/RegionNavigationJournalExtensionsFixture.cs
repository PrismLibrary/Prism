using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class RegionNavigationJournalExtensionsFixture
    {
        [TestMethod]
        public void ThrowsWhenJournalIsNull()
        {
            RegionNavigationJournal journal = null;

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                journal.GoBack();
            });

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                journal.GoForward();
            });
        }

        [TestMethod]
        public void GoBackDelegatesToGoBackAsync()
        {
            Mock<IRegionNavigationJournal> mockJournal = new Mock<IRegionNavigationJournal>();

            mockJournal.Object.GoBack();

            mockJournal.Verify(j => j.GoBackAsync());
        }

        [TestMethod]
        public void GoForwardDelegatesToGoForwardAsync()
        {
            Mock<IRegionNavigationJournal> mockJournal = new Mock<IRegionNavigationJournal>();

            mockJournal.Object.GoForward();

            mockJournal.Verify(j => j.GoForwardAsync());
        }
    }
}
