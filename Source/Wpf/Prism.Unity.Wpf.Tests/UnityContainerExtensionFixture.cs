

using System.Collections.Generic;
using Unity;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;

namespace Prism.Unity.Wpf.Tests
{
    
    public class UnityContainerExtensionFixture
    {
        [Fact]
        public void ExtensionReturnsTrueIfThereIsAPolicyForType()
        {
            UnityContainer container = new UnityContainer();
            container.AddNewExtension<UnityBootstrapperExtension>();

            container.RegisterType<object, string>();
            Assert.True(container.IsTypeRegistered(typeof(object)));
            Assert.False(container.IsTypeRegistered(typeof(int)));

            container.RegisterType<IList<int>, List<int>>();

            Assert.True(container.IsTypeRegistered(typeof(IList<int>)));
            Assert.False(container.IsTypeRegistered(typeof(IList<string>)));

            container.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>));
            Assert.True(container.IsTypeRegistered(typeof(IDictionary<,>)));
        }

        [Fact]
        public void TryResolveShouldResolveTheElementIfElementExist()
        {
            var container = new UnityContainer();
            container.RegisterType<IService, MockService>();

            object dependantA = container.TryResolve<IService>();
            Assert.NotNull(dependantA);
        }

        [Fact]
        public void TryResolveShouldReturnNullIfElementNotExist()
        {
            var container = new UnityContainer();

            object dependantA = container.TryResolve<IDependantA>();
            Assert.Null(dependantA);
        }

        [Fact]
        public void TryResolveWorksWithValueTypes()
        {
            var container = new UnityContainer();

            int valueType = container.TryResolve<int>();
            Assert.Equal(default(int), valueType);
        }

    }
}