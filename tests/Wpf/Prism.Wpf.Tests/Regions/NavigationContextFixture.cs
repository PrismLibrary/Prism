using System;
using System.Linq;
using Xunit;
using Moq;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    
    public class NavigationContextFixture
    {
        [Fact]
        public void WhenCreatingANewContextForAUriWithAQuery_ThenNewContextInitializesPropertiesAndExtractsTheQuery()
        {
            var uri = new Uri("test?name=value", UriKind.Relative);

            var navigationJournalMock = new Mock<IRegionNavigationJournal>();
            var navigationServiceMock = new Mock<IRegionNavigationService>();

            IRegion region = new Region();
            navigationServiceMock.SetupGet(n => n.Region).Returns(region);
            navigationServiceMock.SetupGet(x => x.Journal).Returns(navigationJournalMock.Object);

            var context = new NavigationContext(navigationServiceMock.Object, uri);

            Assert.Same(navigationServiceMock.Object, context.NavigationService);
            Assert.Equal(uri, context.Uri);
            Assert.Single(context.Parameters);
            Assert.Equal("value", context.Parameters["name"]);
        }

        [Fact]
        public void WhenCreatingANewContextForAUriWithNoQuery_ThenNewContextInitializesPropertiesGetsEmptyQuery()
        {
            var uri = new Uri("test", UriKind.Relative);

            var navigationJournalMock = new Mock<IRegionNavigationJournal>();

            var navigationServiceMock = new Mock<IRegionNavigationService>();
            navigationServiceMock.SetupGet(x => x.Journal).Returns(navigationJournalMock.Object);

            var context = new NavigationContext(navigationServiceMock.Object, uri);

            Assert.Same(navigationServiceMock.Object, context.NavigationService);
            Assert.Equal(uri, context.Uri);
            Assert.Empty(context.Parameters);
        }
    }
}
