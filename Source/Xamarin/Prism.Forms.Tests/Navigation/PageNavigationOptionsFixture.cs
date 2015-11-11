using Prism.Forms.Tests.Mocks.Views;
using System.Reflection;
using Xunit;
using Prism.Navigation;

namespace Prism.Forms.Tests.Navigation
{
    public class PageNavigationOptionsFixture
    {
        [Fact]
        public void UseModalNavigationDefaultIsTrue()
        {
            var attribute = typeof(PageWithDefaultNavigationOptionsMock).GetTypeInfo().GetCustomAttribute<PageNavigationOptionsAttribute>();
            Assert.NotNull(attribute);
            Assert.True(attribute.UseModalNavigation);
        }

        [Fact]
        public void AnimatedDefaultIsTrue()
        {
            var attribute = typeof(PageWithDefaultNavigationOptionsMock).GetTypeInfo().GetCustomAttribute<PageNavigationOptionsAttribute>();
            Assert.NotNull(attribute);
            Assert.True(attribute.Animated);
        }

        [Fact]
        public void PageNavigationParameterTypeDefaultIsNull()
        {
            var attribute = typeof(PageWithDefaultNavigationOptionsMock).GetTypeInfo().GetCustomAttribute<PageNavigationOptionsAttribute>();
            Assert.NotNull(attribute);
            Assert.Null(attribute.PageNavigationProviderType);
        }
    }
}
