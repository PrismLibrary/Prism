using System.Collections.Generic;
using Prism.Ioc;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Xunit;
using static Prism.Container.Wpf.Tests.ContainerHelper;

namespace Prism.Container.Wpf.Tests.Ioc
{
    public class ContainerExtensionFixture
    {
        [Fact]
        public void ExtensionReturnsTrueIfThereIsAPolicyForType()
        {
            var container = CreateContainerExtension();

            container.Register<object, string>();
            Assert.True(container.IsRegistered(typeof(object)));
            Assert.False(container.IsRegistered(typeof(int)));

            container.Register<IList<int>, List<int>>();

            Assert.True(container.IsRegistered(typeof(IList<int>)));
            Assert.False(container.IsRegistered(typeof(IList<string>)));

            container.Register(typeof(IDictionary<,>), typeof(Dictionary<,>));
            Assert.True(container.IsRegistered(typeof(IDictionary<,>)));
        }

        [Fact]
        public void TryResolveShouldResolveTheElementIfElementExist()
        {
            var container = CreateContainerExtension();
            container.Register<IService, MockService>();

            object dependantA = null;
            var ex = Record.Exception(() => dependantA = container.Resolve<IService>());
            Assert.Null(ex);
            Assert.NotNull(dependantA);
        }

        [Fact]
        public void TryResolveShouldReturnNullIfElementNotExist()
        {
            var container = CreateContainerExtension();

            object dependantA = null;
            var ex = Record.Exception(() => dependantA = container.Resolve<IDependantA>());
            Assert.NotNull(ex);
            Assert.Null(dependantA);
        }
    }
}
