using System;
using Moq;
using Prism.Ioc;
using Xunit;

namespace Prism.Tests.Ioc
{
    public class ContainerRegistryExtensionsFixture
    {
        private interface ITestService { }

        private interface ITest2Service { }

        private class TestService : ITestService, ITest2Service { }

        [Fact]
        public void RegisterInstanceOfT()
        {
            var testService = new TestService();
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterInstance(typeof(TestService), testService))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterInstance(testService);
            Assert.Same(mock.Object, cr);

            mock.Verify(x => x.RegisterInstance(typeof(TestService), testService));
        }

        [Fact]
        public void RegisterNamedInstanceOfT()
        {
            var testService = new TestService();
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterInstance(typeof(TestService), testService, "Test"))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterInstance(testService, "Test");
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterInstance(typeof(TestService), testService, "Test"));
        }

        [Fact]
        public void RegisterSingletonFromConcreteType()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterSingleton(typeof(TestService), typeof(TestService)))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterSingleton(typeof(TestService));
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterSingleton(typeof(TestService), typeof(TestService)));
        }

        [Fact]
        public void RegisterSingletonFromGenericConcreteType()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterSingleton(typeof(TestService), typeof(TestService)))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterSingleton<TestService>();
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterSingleton(typeof(TestService), typeof(TestService)));
        }

        [Fact]
        public void RegisterSingletonWithService()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterSingleton(typeof(ITestService), typeof(TestService)))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterSingleton<ITestService, TestService>();
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterSingleton(typeof(ITestService), typeof(TestService)));
        }

        [Fact]
        public void RegisterSingletonNamedWithService()
        {

            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterSingleton(typeof(ITestService), typeof(TestService), "Test"))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterSingleton<ITestService, TestService>("Test");
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterSingleton(typeof(ITestService), typeof(TestService), "Test"));
        }

        [Fact]
        public void RegisterSingletonFromDelegateFunction()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterSingleton(typeof(ITestService), TestDelegate))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterSingleton<ITestService>(TestDelegate);
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterSingleton(typeof(ITestService), TestDelegate));
        }

        [Fact]
        public void RegisterSingletonFromDelegateFunctionWithContainerProvider()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterSingleton(typeof(ITestService), TestDelegate))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterSingleton<ITestService>(TestDelegate);
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterSingleton(typeof(ITestService), TestDelegate));
        }

        [Fact]
        public void RegisterSingletonWithManyInterfaces()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterManySingleton(It.IsAny<Type>(), It.IsAny<Type[]>()))
                .Returns(mock.Object);
            var services = new[] { typeof(ITestService), typeof(ITest2Service) };
            var cr = mock.Object.RegisterManySingleton<TestService>(services);
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterManySingleton(typeof(TestService), services));
        }

        [Fact]
        public void RegisterFromConcreteType()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.Register(typeof(TestService), typeof(TestService)))
                .Returns(mock.Object);

            var cr = mock.Object.Register(typeof(TestService));
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.Register(typeof(TestService), typeof(TestService)));
        }

        [Fact]
        public void RegisterFromGenericConcreteType()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.Register(typeof(TestService), typeof(TestService)))
                .Returns(mock.Object);

            var cr = mock.Object.Register<TestService>();
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.Register(typeof(TestService), typeof(TestService)));
        }

        [Fact]
        public void RegisterWithService()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.Register(typeof(ITestService), typeof(TestService)))
                .Returns(mock.Object);

            var cr = mock.Object.Register<ITestService, TestService>();
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.Register(typeof(ITestService), typeof(TestService)));
        }

        [Fact]
        public void RegisterNamedWithService()
        {

            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.Register(typeof(ITestService), typeof(TestService), "Test"))
                .Returns(mock.Object);

            var cr = mock.Object.Register<ITestService, TestService>("Test");
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.Register(typeof(ITestService), typeof(TestService), "Test"));
        }

        [Fact]
        public void RegisterFromDelegateFunction()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.Register(typeof(ITestService), TestDelegate))
                .Returns(mock.Object);

            var cr = mock.Object.Register<ITestService>(TestDelegate);
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.Register(typeof(ITestService), TestDelegate));
        }

        [Fact]
        public void RegisterFromDelegateFunctionWithContainerProvider()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.Register(typeof(ITestService), TestDelegate))
                .Returns(mock.Object);

            var cr = mock.Object.Register<ITestService>(TestDelegate);
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.Register(typeof(ITestService), TestDelegate));
        }

        [Fact]
        public void RegisterWithManyInterfaces()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterMany(It.IsAny<Type>(), It.IsAny<Type[]>()))
                .Returns(mock.Object);
            var services = new[] { typeof(ITestService), typeof(ITest2Service) };
            var cr = mock.Object.RegisterMany<TestService>(services);
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterMany(typeof(TestService), services));
        }

        [Fact]
        public void RegisterScopedFromConcreteType()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterScoped(typeof(TestService), typeof(TestService)))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterScoped(typeof(TestService));
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterScoped(typeof(TestService), typeof(TestService)));
        }

        [Fact]
        public void RegisterScopedFromGenericConcreteType()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterScoped(typeof(TestService), typeof(TestService)))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterScoped<TestService>();
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterScoped(typeof(TestService), typeof(TestService)));
        }

        [Fact]
        public void RegisterScopedWithService()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterScoped(typeof(ITestService), typeof(TestService)))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterScoped<ITestService, TestService>();
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterScoped(typeof(ITestService), typeof(TestService)));
        }

        [Fact]
        public void RegisterScopedFromDelegateFunction()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterScoped(typeof(ITestService), TestDelegate))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterScoped<ITestService>(TestDelegate);
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterScoped(typeof(ITestService), TestDelegate));
        }

        [Fact]
        public void RegisterScopedFromDelegateFunctionWithContainerProvider()
        {
            var mock = new Mock<IContainerRegistry>();
            mock.Setup(x => x.RegisterScoped(typeof(ITestService), TestDelegate))
                .Returns(mock.Object);

            var cr = mock.Object.RegisterScoped<ITestService>(TestDelegate);
            Assert.Same(mock.Object, cr);
            mock.Verify(x => x.RegisterScoped(typeof(ITestService), TestDelegate));
        }




        private static ITestService TestDelegate() =>
            new TestService();

        private static ITestService TestDelegateFactory(IContainerProvider containerProvider) =>
            containerProvider.Resolve<ITestService>("Named");
    }
}
