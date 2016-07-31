using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class NavigationContextFixture
    {
        [TestMethod]
        public void WhenCreatingANewContextForAUriWithAQuery_ThenNewContextInitializesPropertiesAndExtractsTheQuery()
        {
            var uri = new Uri("test?name=value", UriKind.Relative);

            var navigationJournalMock = new Mock<IRegionNavigationJournal>();
            var navigationServiceMock = new Mock<IRegionNavigationService>();

            IRegion region = new Region();
            navigationServiceMock.SetupGet(n => n.Region).Returns(region);
            navigationServiceMock.SetupGet(x => x.Journal).Returns(navigationJournalMock.Object);

            var context = new NavigationContext(navigationServiceMock.Object, uri);

            Assert.AreSame(navigationServiceMock.Object, context.NavigationService);
            Assert.AreEqual(uri, context.Uri);
            Assert.AreEqual(1, context.Parameters.Count());
            Assert.AreEqual("value", context.Parameters["name"]);
        }

        [TestMethod]
        public void WhenCreatingANewContextForAUriWithNoQuery_ThenNewContextInitializesPropertiesGetsEmptyQuery()
        {
            var uri = new Uri("test", UriKind.Relative);

            var navigationJournalMock = new Mock<IRegionNavigationJournal>();

            var navigationServiceMock = new Mock<IRegionNavigationService>();
            navigationServiceMock.SetupGet(x => x.Journal).Returns(navigationJournalMock.Object);

            var context = new NavigationContext(navigationServiceMock.Object, uri);

            Assert.AreSame(navigationServiceMock.Object, context.NavigationService);
            Assert.AreEqual(uri, context.Uri);
            Assert.AreEqual(0, context.Parameters.Count());
        }
    }
}
