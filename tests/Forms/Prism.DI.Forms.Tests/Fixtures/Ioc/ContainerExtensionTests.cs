using System;
using System.Collections.Generic;
using System.Text;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Ioc;
using Prism.Ioc.Internals;
using Xunit;

#if DryIoc
using ContainerExtension = Prism.DryIoc.DryIocContainerExtension;
using INativeContainer = DryIoc.IContainer;

namespace Prism.DryIoc.Forms.Tests.Fixtures
#elif Unity
using ContainerExtension = Prism.Unity.UnityContainerExtension;
using INativeContainer = Unity.IUnityContainer;

namespace Prism.Unity.Forms.Tests.Fixtures
#endif
{
    public class ContainerExtensionTests
    {
        [Fact]
        public void IContainerProviderIsRegistered()
        {
            var container = new ContainerExtension();
            object resolved = container.Resolve<IContainerProvider>();
            Assert.Same(container, resolved);
        }

        [Fact]
        public void IContainerExtensionIsRegistered()
        {
            var container = new ContainerExtension();
            object resolved = container.Resolve<IContainerExtension>();
            Assert.Same(container, resolved);
        }

        [Fact]
        public void NativeContainerIsRegistered()
        {
            var container = new ContainerExtension();
            object resolved = container.Resolve<INativeContainer>();
            Assert.Same(container.Instance, resolved);
        }

        [Fact]
        public void RegisterInstanceReturnsSameInstance()
        {
            var instance = new ServiceMock();
            var container = new ContainerExtension();
            container.RegisterInstance<object>(instance);
            object resolved = container.Resolve<object>();
            Assert.Same(instance, resolved);
        }

        [Fact]
        public void RegisterNamedInstanceReturnsSameInstance()
        {
            var instance = new ServiceMock();
            var container = new ContainerExtension();
            container.RegisterInstance<object>(instance, "Test");
            var resolved = container.Resolve<object>("Test");
            Assert.Same(instance, resolved);
        }

        [Fact]
        public void AutomaticTransientResolutionOfConcreteType()
        {
            var container = new ContainerExtension();
            var resolved1 = container.Resolve<ServiceMock>();
            var resolved2 = container.Resolve<ServiceMock>();
            Assert.NotNull(resolved1);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterConcreteTypeCreatesTransient()
        {
            var container = new ContainerExtension();
            container.Register<ServiceMock>();
            var resolved1 = container.Resolve<ServiceMock>();
            var resolved2 = container.Resolve<ServiceMock>();
            Assert.NotNull(resolved1);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterNamedConcreteTypeCreatesTransient()
        {
            var container = new ContainerExtension();
            container.Register<ServiceMock>("Test");
            var resolved1 = container.Resolve<ServiceMock>("Test");
            var resolved2 = container.Resolve<ServiceMock>("Test");
            Assert.NotNull(resolved1);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterServiceMappingCreatesTransient()
        {
            var container = new ContainerExtension();
            container.Register<IServiceMock, ServiceMock>();
            var resolved1 = container.Resolve<IServiceMock>();
            var resolved2 = container.Resolve<IServiceMock>();
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceMock>(resolved1);
            Assert.IsType<ServiceMock>(resolved2);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterNamedServiceMappingCreatesTransient()
        {
            var container = new ContainerExtension();
            container.Register<IServiceMock, ServiceMock>("Test");
            var resolved1 = container.Resolve<IServiceMock>("Test");
            var resolved2 = container.Resolve<IServiceMock>("Test");
            var ex = Record.Exception(() => container.Resolve<IServiceMock>());
            Assert.NotNull(ex);
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceMock>(resolved1);
            Assert.IsType<ServiceMock>(resolved2);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterTransientWithFactory()
        {
            var container = new ContainerExtension();
            container.Register<IServiceMock>(CreateService);
            var resolved = container.Resolve<IServiceMock>();
            Assert.NotNull(resolved);
            Assert.IsType<ServiceMock>(resolved);
            var serviceMock = resolved as ServiceMock;
            Assert.Equal("Created through a factory", serviceMock.SomeProperty);
            var resolved2 = container.Resolve<IServiceMock>();
            Assert.NotSame(resolved, resolved2);
        }

        [Fact]
        public void RegisterTransientWithFactoryAndContainerProvider()
        {
            var container = new ContainerExtension();
            container.Register<IServiceMock, ServiceMock>(nameof(ServiceMock));
            container.Register<IServiceMock>(CreateServiceWithContainerProvider);
            var resolved = container.Resolve<IServiceMock>();
            Assert.NotNull(resolved);
            Assert.IsType<ServiceMock>(resolved);
            var serviceMock = resolved as ServiceMock;

            var resolved2 = container.Resolve<IServiceMock>();
            Assert.NotSame(resolved, resolved2);
        }

        [Fact]
        public void RegisterManyRegistersAllInterfacesByDefault()
        {
            var container = new ContainerExtension();
            container.RegisterMany<AggregateService>();

            Assert.IsType<AggregateService>(container.Resolve<IServiceA>());
            Assert.IsType<AggregateService>(container.Resolve<IServiceB>());
            Assert.IsType<AggregateService>(container.Resolve<IServiceC>());
        }

        [Fact]
        public void RegisterManyOnlyRegistersSpecifiedInterfaces()
        {
            var container = new ContainerExtension();
            container.RegisterMany<AggregateService>(typeof(IServiceA), typeof(IServiceB));

            Assert.IsType<AggregateService>(container.Resolve<IServiceA>());
            Assert.IsType<AggregateService>(container.Resolve<IServiceB>());
            var ex = Record.Exception(() => container.Resolve<IServiceC>());
            Assert.NotNull(ex);
        }

        [Fact]
        public void RegisterManyRegistersServicesAsTransients()
        {
            var container = new ContainerExtension();
            container.RegisterMany<AggregateService>();
            Assert.NotSame(container.Resolve<IServiceA>(), container.Resolve<IServiceA>());
        }

        [Fact]
        public void RegisterSupportsLazyInjection()
        {
            var container = new ContainerExtension();
            container.Register<IServiceMock, ServiceMock>();
            var lazy = container.Resolve<Lazy<IServiceMock>>();
            Assert.NotNull(lazy);
            Assert.False(lazy.IsValueCreated);
            var instance = lazy.Value;
            Assert.IsType<ServiceMock>(instance);

            var lazy2 = container.Resolve<Lazy<IServiceMock>>();
            Assert.NotSame(instance, lazy2.Value);
        }

        [Fact]
        public void RegisterSupportsFuncInjection()
        {
            var container = new ContainerExtension();
            container.Register<IServiceMock, ServiceMock>();
            var func = container.Resolve<Func<IServiceMock>>();
            Assert.NotNull(func);
            var instance = func();
            Assert.IsType<ServiceMock>(instance);

            var func2 = container.Resolve<Func<IServiceMock>>();
            Assert.NotSame(instance, func2());
        }

        [Fact]
        public void RegisterSingletonConcreteTypeCreatesSingleton()
        {
            var container = new ContainerExtension();
            container.RegisterSingleton<ServiceMock>();
            var resolved1 = container.Resolve<ServiceMock>();
            var resolved2 = container.Resolve<ServiceMock>();
            Assert.NotNull(resolved1);
            Assert.Same(resolved1, resolved2);
        }

        [Fact]
        public void RegisterSingletonServiceMappingCreatesSingleton()
        {
            var container = new ContainerExtension();
            container.RegisterSingleton<IServiceMock, ServiceMock>();
            var resolved1 = container.Resolve<IServiceMock>();
            var resolved2 = container.Resolve<IServiceMock>();
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceMock>(resolved1);
            Assert.IsType<ServiceMock>(resolved2);
            Assert.Same(resolved1, resolved2);
        }

        [Fact]
        public void RegisterSingletonNamedServiceMappingCreatesSingleton()
        {
            var container = new ContainerExtension();
            container.RegisterSingleton<IServiceMock, ServiceMock>("Test");
            var resolved1 = container.Resolve<IServiceMock>("Test");
            var resolved2 = container.Resolve<IServiceMock>("Test");
            var ex = Record.Exception(() => container.Resolve<IServiceMock>());
            Assert.NotNull(ex);
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceMock>(resolved1);
            Assert.IsType<ServiceMock>(resolved2);
            Assert.Same(resolved1, resolved2);
        }

        [Fact]
        public void RegisterSingletonWithFactory()
        {
            var container = new ContainerExtension();
            container.RegisterSingleton<IServiceMock>(CreateService);
            var resolved = container.Resolve<IServiceMock>();
            Assert.NotNull(resolved);
            Assert.IsType<ServiceMock>(resolved);
            var serviceMock = resolved as ServiceMock;
            Assert.Equal("Created through a factory", serviceMock.SomeProperty);
            var resolved2 = container.Resolve<IServiceMock>();
            Assert.Same(resolved, resolved2);
        }

        [Fact]
        public void RegisterSingletonWithFactoryAndContainerProvider()
        {
            var container = new ContainerExtension();
            container.Register<IServiceMock, ServiceMock>(nameof(ServiceMock));
            container.RegisterSingleton<IServiceMock>(CreateServiceWithContainerProvider);
            var resolved = container.Resolve<IServiceMock>();
            Assert.NotNull(resolved);
            Assert.IsType<ServiceMock>(resolved);
            var serviceMock = resolved as ServiceMock;
            var resolved2 = container.Resolve<IServiceMock>();
            Assert.Same(resolved, resolved2);
        }

        [Fact]
        public void RegisterManySingletonRegistersAllInterfacesByDefault()
        {
            var container = new ContainerExtension();
            container.RegisterManySingleton<AggregateService>();

            Assert.IsType<AggregateService>(container.Resolve<IServiceA>());
            Assert.IsType<AggregateService>(container.Resolve<IServiceB>());
            Assert.IsType<AggregateService>(container.Resolve<IServiceC>());
        }

        [Fact]
        public void RegisterManySingletonOnlyRegistersSpecifiedInterfaces()
        {
            var container = new ContainerExtension();
            container.RegisterManySingleton<AggregateService>(typeof(IServiceA), typeof(IServiceB));

            Assert.IsType<AggregateService>(container.Resolve<IServiceA>());
            Assert.IsType<AggregateService>(container.Resolve<IServiceB>());
            var ex = Record.Exception(() => container.Resolve<IServiceC>());
            Assert.NotNull(ex);
        }

        [Fact]
        public void RegisterManySingletonUsesSharedInstanceForAllServices()
        {
            var container = new ContainerExtension();
            container.RegisterManySingleton<AggregateService>();

            var serviceA = container.Resolve<IServiceA>();
            var serviceB = container.Resolve<IServiceB>();
            var serviceC = container.Resolve<IServiceC>();

            Assert.Same(serviceA, serviceB);
            Assert.Same(serviceB, serviceC);
        }

        [Fact]
        public void RegisterSingletonSupportsLazyInjection()
        {
            var container = new ContainerExtension();
            container.RegisterSingleton<IServiceMock, ServiceMock>();
            var lazy = container.Resolve<Lazy<IServiceMock>>();
            Assert.NotNull(lazy);
            Assert.False(lazy.IsValueCreated);
            var instance = lazy.Value;
            Assert.IsType<ServiceMock>(instance);

            var lazy2 = container.Resolve<Lazy<IServiceMock>>();
            Assert.Same(instance, lazy2.Value);
        }

        [Fact]
        public void RegisterSingletonSupportsFuncInjection()
        {
            var container = new ContainerExtension();
            container.RegisterSingleton<IServiceMock, ServiceMock>();
            var func = container.Resolve<Func<IServiceMock>>();
            Assert.NotNull(func);
            var instance = func();
            Assert.IsType<ServiceMock>(instance);

            var func2 = container.Resolve<Func<IServiceMock>>();
            Assert.Same(instance, func2());
        }

        [Fact]
        public void RegisterScopedConcreteTypeCreatesScoped()
        {
            var container = new ContainerExtension();
            container.RegisterScoped<ServiceMock>();
            var resolved1 = container.Resolve<ServiceMock>();
            var resolved2 = container.Resolve<ServiceMock>();

            Assert.NotNull(resolved1);
            Assert.Same(resolved1, resolved2);

            container.CreateScope();
            var resolved3 = container.Resolve<ServiceMock>();
            Assert.NotSame(resolved1, resolved3);
        }

        [Fact]
        public void RegisterScopedServiceMappingCreatesScoped()
        {
            var container = new ContainerExtension();
            container.RegisterScoped<IServiceMock, ServiceMock>();
            var resolved1 = container.Resolve<IServiceMock>();
            var resolved2 = container.Resolve<IServiceMock>();
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceMock>(resolved1);
            Assert.IsType<ServiceMock>(resolved2);
            Assert.Same(resolved1, resolved2);

            container.CreateScope();
            var resolved3 = container.Resolve<IServiceMock>();
            Assert.NotSame(resolved1, resolved3);
        }

        [Fact]
        public void RegisterScopedWithFactory()
        {
            var container = new ContainerExtension();
            container.RegisterScoped<IServiceMock>(CreateService);
            var resolved1 = container.Resolve<IServiceMock>();
            Assert.NotNull(resolved1);
            Assert.IsType<ServiceMock>(resolved1);
            var serviceMock = resolved1 as ServiceMock;
            Assert.Equal("Created through a factory", serviceMock.SomeProperty);

            var resolved2 = container.Resolve<IServiceMock>();
            Assert.Same(resolved1, resolved2);
            container.CreateScope();
            var resolved3 = container.Resolve<IServiceMock>();
            Assert.NotSame(resolved2, resolved3);
        }

        [Fact]
        public void RegisterScopedWithFactoryAndContainerProvider()
        {
            var container = new ContainerExtension();
            container.Register<IServiceMock, ServiceMock>(nameof(ServiceMock));
            container.RegisterScoped<IServiceMock>(CreateServiceWithContainerProvider);
            var resolved1 = container.Resolve<IServiceMock>();
            Assert.NotNull(resolved1);
            Assert.IsType<ServiceMock>(resolved1);

            var resolved2 = container.Resolve<IServiceMock>();
            Assert.Same(resolved1, resolved2);
            container.CreateScope();
            var resolved3 = container.Resolve<IServiceMock>();
            Assert.NotSame(resolved2, resolved3);
        }

        [Fact]
        public void LocatesImplementationTypeForNamedService()
        {
            var container = new ContainerExtension();
            container.RegisterForNavigation<ViewAMock>();
            var type = container.GetRegistrationType(nameof(ViewAMock));
            Assert.NotNull(type);
            Assert.Equal(typeof(ViewAMock), type);
        }

        [Fact]
        public void LocatesImplementationType()
        {
            var container = new ContainerExtension();
            container.Register<IServiceMock, ServiceMock>();
            var type = container.GetRegistrationType(typeof(IServiceMock));
            Assert.NotNull(type);
            Assert.Equal(typeof(ServiceMock), type);
        }

        private static IServiceMock CreateService() =>
            new ServiceMock { SomeProperty = "Created through a factory" };

        private static IServiceMock CreateServiceWithContainerProvider(IContainerProvider containerProvider) =>
            containerProvider.Resolve<IServiceMock>(nameof(ServiceMock));
    }
}
