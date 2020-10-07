using Prism.Forms.Tests.Mocks.Views;
using Prism.Navigation;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    public class PageNavigationInfoFixture
    {
        [Fact]
        public void PageNavigationInfoNameIsSet()
        {
            var info = new PageNavigationInfo();

            Assert.Null(info.Name);

            info.Name = "MainPage";

            Assert.NotNull(info.Name);
        }

        [Fact]
        public void PageNavigationInfoTypeIsSet()
        {
            var info = new PageNavigationInfo();

            Assert.Null(info.Type);

            var type = typeof(PageMock);
            info.Type = type;

            Assert.Equal(info.Type, type);
        }
    }
}
