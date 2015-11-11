using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Xunit;
using System.Reflection;
using Prism.Navigation;
using Prism.Forms.Tests.Mocks.Views;

namespace Prism.Forms.Tests.Common
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

        [Fact]
        public void PageNavigationInfoNavigationOptionsIsSet()
        {
            var info = new PageNavigationInfo();

            Assert.Null(info.NavigationOptions);

            var attribute = typeof(PageMock).GetTypeInfo().GetCustomAttribute<PageNavigationOptionsAttribute>();

            info.NavigationOptions = attribute;

            Assert.Equal(info.NavigationOptions, attribute);
        }
    }
}
