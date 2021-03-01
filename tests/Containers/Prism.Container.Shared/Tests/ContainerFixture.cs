using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prism.Ioc.Mocks.Services;
using Prism.Ioc.Mocks.Views;
using Xunit;

namespace Prism.Ioc.Tests
{
    public class ContainerTests : TestBase
    {
        public ContainerTests(ContainerSetup setup)
            : base(setup)
        {
        }

        [Fact]
        public void IContainerProviderIsRegistered()
        {
            var container = Setup.CreateContainer();
            var resolved = container.Resolve<IContainerProvider>();
            Assert.Same(container, resolved);
        }

        [Fact]
        public void IContainerExtensionIsRegistered()
        {
            var container = Setup.CreateContainer();
            var resolved = container.Resolve<IContainerExtension>();
            Assert.Same(container, resolved);
        }

        [Fact]
        public void NativeContainerIsRegistered()
        {
            var container = Setup.CreateContainer();
            object resolved = null;
            var ex = Record.Exception(() => resolved = container.Resolve(Setup.NativeContainerType));
            Assert.Null(ex);
            Assert.NotNull(resolved);
            Assert.IsAssignableFrom(Setup.NativeContainerType, resolved);
        }

        [Fact]
        public void RegisterInstanceReturnsSameInstance()
        {
            var instance = new ServiceA();
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterInstance<object>(instance);
            object resolved = container.Resolve<object>();
            Assert.Same(instance, resolved);
        }

        [Fact]
        public void RegisterNamedInstanceReturnsSameInstance()
        {
            var instance = new ServiceA();
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterInstance<object>(instance, "Test");
            var resolved = container.Resolve<object>("Test");
            Assert.Same(instance, resolved);
        }

        [Fact]
        public void AutomaticTransientResolutionOfConcreteType()
        {
            var container = Setup.CreateContainer();
            var resolved1 = container.Resolve<ServiceA>();
            var resolved2 = container.Resolve<ServiceA>();
            Assert.NotNull(resolved1);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterConcreteTypeCreatesTransient()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<ServiceA>();
            var resolved1 = container.Resolve<ServiceA>();
            var resolved2 = container.Resolve<ServiceA>();
            Assert.NotNull(resolved1);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterNamedConcreteTypeCreatesTransient()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<ServiceA>("Test");
            var resolved1 = container.Resolve<ServiceA>("Test");
            var resolved2 = container.Resolve<ServiceA>("Test");
            Assert.NotNull(resolved1);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterServiceMappingCreatesTransient()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();
            var resolved1 = container.Resolve<IServiceA>();
            var resolved2 = container.Resolve<IServiceA>();
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceA>(resolved1);
            Assert.IsType<ServiceA>(resolved2);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterNamedServiceMappingCreatesTransient()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>("Test");
            var resolved1 = container.Resolve<IServiceA>("Test");
            var resolved2 = container.Resolve<IServiceA>("Test");
            var ex = Record.Exception(() => container.Resolve<IServiceA>());
            Assert.NotNull(ex);
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceA>(resolved1);
            Assert.IsType<ServiceA>(resolved2);
            Assert.NotSame(resolved1, resolved2);
        }

        [Fact]
        public void RegisterTransientWithFactory()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<IServiceA>(CreateService);
            var resolved = container.Resolve<IServiceA>();
            Assert.NotNull(resolved);
            Assert.IsType<ServiceA>(resolved);
            var ServiceA = resolved as ServiceA;
            Assert.Equal("Created through a factory", ServiceA.SomeProperty);
            var resolved2 = container.Resolve<IServiceA>();
            Assert.NotSame(resolved, resolved2);
        }

        [Fact]
        public void RegisterTransientWithFactoryAndContainerProvider()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>(nameof(ServiceA));
            Setup.Registry.Register<IServiceA>(CreateServiceWithContainerProvider);
            var resolved = container.Resolve<IServiceA>();
            Assert.NotNull(resolved);
            Assert.IsType<ServiceA>(resolved);
            var serviceA = resolved as ServiceA;

            var resolved2 = container.Resolve<IServiceA>();
            Assert.NotSame(resolved, resolved2);
        }

        [Fact]
        public void RegisterManyRegistersAllInterfacesByDefault()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterMany<CompositeService>();

            Assert.IsType<CompositeService>(container.Resolve<IServiceA>());
            Assert.IsType<CompositeService>(container.Resolve<IServiceB>());
            Assert.IsType<CompositeService>(container.Resolve<IServiceC>());
        }

        [Fact]
        public void RegisterManyOnlyRegistersSpecifiedInterfaces()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterMany<CompositeService>(typeof(IServiceA), typeof(IServiceB));

            Assert.IsType<CompositeService>(container.Resolve<IServiceA>());
            Assert.IsType<CompositeService>(container.Resolve<IServiceB>());
            var ex = Record.Exception(() => container.Resolve<IServiceC>());
            Assert.NotNull(ex);
        }

        [Fact]
        public void RegisterManyRegistersServicesAsTransients()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterMany<CompositeService>();
            Assert.NotSame(container.Resolve<IServiceA>(), container.Resolve<IServiceA>());
        }

        [Fact]
        public void RegisterSupportsLazyInjection()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();
            var lazy = container.Resolve<Lazy<IServiceA>>();
            Assert.NotNull(lazy);
            Assert.False(lazy.IsValueCreated);
            var instance = lazy.Value;
            Assert.IsType<ServiceA>(instance);

            var lazy2 = container.Resolve<Lazy<IServiceA>>();
            Assert.NotSame(instance, lazy2.Value);
        }

        [Fact]
        public void RegisterSupportsFuncInjection()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();
            var func = container.Resolve<Func<IServiceA>>();
            Assert.NotNull(func);
            var instance = func();
            Assert.IsType<ServiceA>(instance);

            var func2 = container.Resolve<Func<IServiceA>>();
            Assert.NotSame(instance, func2());
        }

        [Fact]
        public void RegisterSingletonConcreteTypeCreatesSingleton()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<ServiceA>();
            var resolved1 = container.Resolve<ServiceA>();
            var resolved2 = container.Resolve<ServiceA>();
            Assert.NotNull(resolved1);
            Assert.Same(resolved1, resolved2);
        }

        [Fact]
        public void RegisterSingletonServiceMappingCreatesSingleton()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA, ServiceA>();
            var resolved1 = container.Resolve<IServiceA>();
            var resolved2 = container.Resolve<IServiceA>();
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceA>(resolved1);
            Assert.IsType<ServiceA>(resolved2);
            Assert.Same(resolved1, resolved2);
        }

        [Fact]
        public void RegisterSingletonNamedServiceMappingCreatesSingleton()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA, ServiceA>("Test");
            var resolved1 = container.Resolve<IServiceA>("Test");
            var resolved2 = container.Resolve<IServiceA>("Test");
            var ex = Record.Exception(() => container.Resolve<IServiceA>());
            Assert.NotNull(ex);
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceA>(resolved1);
            Assert.IsType<ServiceA>(resolved2);
            Assert.Same(resolved1, resolved2);
        }

        [Fact]
        public void RegisterSingletonWithFactory()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA>(CreateService);
            var resolved = container.Resolve<IServiceA>();
            Assert.NotNull(resolved);
            Assert.IsType<ServiceA>(resolved);
            var ServiceA = resolved as ServiceA;
            Assert.Equal("Created through a factory", ServiceA.SomeProperty);
            var resolved2 = container.Resolve<IServiceA>();
            Assert.Same(resolved, resolved2);
        }

        [Fact]
        public void RegisterSingletonWithFactoryAndContainerProvider()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>(nameof(ServiceA));
            Setup.Registry.RegisterSingleton<IServiceA>(CreateServiceWithContainerProvider);
            var resolved = container.Resolve<IServiceA>();
            Assert.NotNull(resolved);
            Assert.IsType<ServiceA>(resolved);
            var serviceA = resolved as ServiceA;
            var resolved2 = container.Resolve<IServiceA>();
            Assert.Same(resolved, resolved2);
        }

        [Fact]
        public void RegisterManySingletonRegistersAllInterfacesByDefault()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterManySingleton<CompositeService>();

            Assert.IsType<CompositeService>(container.Resolve<IServiceA>());
            Assert.IsType<CompositeService>(container.Resolve<IServiceB>());
            Assert.IsType<CompositeService>(container.Resolve<IServiceC>());
        }

        [Fact]
        public void RegisterManySingletonOnlyRegistersSpecifiedInterfaces()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterManySingleton<CompositeService>(typeof(IServiceA), typeof(IServiceB));

            Assert.IsType<CompositeService>(container.Resolve<IServiceA>());
            Assert.IsType<CompositeService>(container.Resolve<IServiceB>());
            var ex = Record.Exception(() => container.Resolve<IServiceC>());
            Assert.NotNull(ex);
        }

        [Fact]
        public void RegisterManySingletonUsesSharedInstanceForAllServices()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterManySingleton<CompositeService>();

            var serviceA = container.Resolve<IServiceA>();
            var serviceB = container.Resolve<IServiceB>();
            var serviceC = container.Resolve<IServiceC>();

            Assert.Same(serviceA, serviceB);
            Assert.Same(serviceB, serviceC);
        }

        [Fact]
        public void RegisterSingletonSupportsLazyInjection()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA, ServiceA>();
            var lazy = container.Resolve<Lazy<IServiceA>>();
            Assert.NotNull(lazy);
            Assert.False(lazy.IsValueCreated);
            var instance = lazy.Value;
            Assert.IsType<ServiceA>(instance);

            var lazy2 = container.Resolve<Lazy<IServiceA>>();
            Assert.Same(instance, lazy2.Value);
        }

        [Fact]
        public void RegisterSingletonSupportsFuncInjection()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA, ServiceA>();
            var func = container.Resolve<Func<IServiceA>>();
            Assert.NotNull(func);
            var instance = func();
            Assert.IsType<ServiceA>(instance);

            var func2 = container.Resolve<Func<IServiceA>>();
            Assert.Same(instance, func2());
        }

        [Fact]
        public void RegisterScopedConcreteTypeCreatesScoped()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterScoped<ServiceA>();
            var resolved1 = container.Resolve<ServiceA>();
            var resolved2 = container.Resolve<ServiceA>();

            Assert.NotNull(resolved1);
            Assert.Same(resolved1, resolved2);

            container.CreateScope();
            var resolved3 = container.Resolve<ServiceA>();
            Assert.NotSame(resolved1, resolved3);
        }

        [Fact]
        public void RegisterScopedServiceMappingCreatesScoped()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterScoped<IServiceA, ServiceA>();
            var resolved1 = container.Resolve<IServiceA>();
            var resolved2 = container.Resolve<IServiceA>();
            Assert.NotNull(resolved1);
            Assert.NotNull(resolved2);
            Assert.IsType<ServiceA>(resolved1);
            Assert.IsType<ServiceA>(resolved2);
            Assert.Same(resolved1, resolved2);

            container.CreateScope();
            var resolved3 = container.Resolve<IServiceA>();
            Assert.NotSame(resolved1, resolved3);
        }

        [Fact]
        public void RegisterScopedWithFactory()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.RegisterScoped<IServiceA>(CreateService);
            var resolved1 = container.Resolve<IServiceA>();
            Assert.NotNull(resolved1);
            Assert.IsType<ServiceA>(resolved1);
            var ServiceA = resolved1 as ServiceA;
            Assert.Equal("Created through a factory", ServiceA.SomeProperty);

            var resolved2 = container.Resolve<IServiceA>();
            Assert.Same(resolved1, resolved2);
            container.CreateScope();
            var resolved3 = container.Resolve<IServiceA>();
            Assert.NotSame(resolved2, resolved3);
        }

        [Fact]
        public void RegisterScopedWithFactoryAndContainerProvider()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>(nameof(ServiceA));
            Setup.Registry.RegisterScoped<IServiceA>(CreateServiceWithContainerProvider);
            var resolved1 = container.Resolve<IServiceA>();
            Assert.NotNull(resolved1);
            Assert.IsType<ServiceA>(resolved1);

            var resolved2 = container.Resolve<IServiceA>();
            Assert.Same(resolved1, resolved2);
            container.CreateScope();
            var resolved3 = container.Resolve<IServiceA>();
            Assert.NotSame(resolved2, resolved3);
        }

        [Fact]
        public void LocatesImplementationTypeForNamedService()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<object, ViewA>(nameof(ViewA));
            var type = ((Internals.IContainerInfo)Setup.Extension).GetRegistrationType(nameof(ViewA));
            Assert.NotNull(type);
            Assert.Equal(typeof(ViewA), type);
        }

        [Fact]
        public void LocatesImplementationType()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();
            var type = ((Internals.IContainerInfo)Setup.Extension).GetRegistrationType(typeof(IServiceA));
            Assert.NotNull(type);
            Assert.Equal(typeof(ServiceA), type);
        }

        private static IServiceA CreateService() =>
            new ServiceA { SomeProperty = "Created through a factory" };

        private static IServiceA CreateServiceWithContainerProvider(IContainerProvider containerProvider) =>
            containerProvider.Resolve<IServiceA>(nameof(ServiceA));



































        [Fact]
        public void Register_RegistersTransientService()
        {
            Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();

            var resolve1 = Setup.Container.Resolve<IServiceA>();
            var resolve2 = Setup.Container.Resolve<IServiceA>();

            Assert.IsType<ServiceA>(resolve1);
            Assert.NotSame(resolve1, resolve2);
        }

        [Fact]
        public void Register_RegistersFunc()
        {
            Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();

            var factory = Setup.Container.Resolve<Func<IServiceA>>();
            IServiceA service = null;
            Assert.NotNull(factory);
            var ex = Record.Exception(() => service = factory());
            Assert.Null(ex);
            Assert.NotNull(service);
            Assert.IsType<ServiceA>(service);
            Assert.NotSame(service, factory());
        }

        [Fact]
        public void Register_RegistersLazy()
        {
            Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();

            var factory = Setup.Container.Resolve<Lazy<IServiceA>>();
            IServiceA service = null;
            Assert.NotNull(factory);
            var ex = Record.Exception(() => service = factory.Value);
            Assert.Null(ex);
            Assert.NotNull(service);
            Assert.IsType<ServiceA>(service);
            Assert.NotSame(service, Setup.Container.Resolve<Lazy<IServiceA>>().Value);
        }

#if !UNITY
        [Fact]
        public void Register_RegistersIEnumerable()
        {
            Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();
            var services = Setup.Container.Resolve<IEnumerable<IServiceA>>();

            Assert.NotNull(services);
            Assert.Single(services);
            Assert.IsType<ServiceA>(services.First());
        }
#endif

        [Fact]
        public void Register_LastInFirstOut()
        {
            Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA, ServiceA>();
            Setup.Registry.Register<IServiceA, CompositeService>();
            var service = Setup.Container.Resolve<IServiceA>();

            Assert.IsType<CompositeService>(service);
            Assert.NotSame(service, Setup.Container.Resolve<IServiceA>());
        }

#if !UNITY
        [Theory]
        [InlineData(0, typeof(ServiceA))]
        [InlineData(1, typeof(CompositeService))]
        public void Register_ResolveAll(int index, Type type)
        {
            Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();
            Setup.Registry.Register<IServiceA, CompositeService>();
            var services = Setup.Container.Resolve<IEnumerable<IServiceA>>();

            Assert.NotNull(services);
            Assert.Equal(2, services.Count());
            Assert.IsType(type, services.ElementAt(index));
        }

#endif

        [Fact]
        public void RegisterSingleton_RegistersSingletonService()
        {
            Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA, ServiceA>();

            var resolve1 = Setup.Container.Resolve<IServiceA>();
            var resolve2 = Setup.Container.Resolve<IServiceA>();

            Assert.IsType<ServiceA>(resolve1);
            Assert.Same(resolve1, resolve2);
        }

        [Fact]
        public void RegisterSinglton_RegistersFunc()
        {
            Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA, ServiceA>();

            var factory = Setup.Container.Resolve<Func<IServiceA>>();
            IServiceA service = null;
            Assert.NotNull(factory);
            var ex = Record.Exception(() => service = factory());
            Assert.Null(ex);
            Assert.NotNull(service);
            Assert.IsType<ServiceA>(service);
            Assert.Same(service, factory());
        }

        [Fact]
        public void RegisterSingleton_RegistersLazy()
        {
            Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA, ServiceA>();

            var factory = Setup.Container.Resolve<Lazy<IServiceA>>();
            IServiceA service = null;
            Assert.NotNull(factory);
            var ex = Record.Exception(() => service = factory.Value);
            Assert.Null(ex);
            Assert.NotNull(service);
            Assert.IsType<ServiceA>(service);
            Assert.Same(service, Setup.Container.Resolve<Lazy<IServiceA>>().Value);
        }

#if !UNITY

        [Fact]
        public void RegisterSingleton_RegistersIEnumerable()
        {
            Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();
            var services = Setup.Container.Resolve<IEnumerable<IServiceA>>();

            Assert.NotNull(services);
            Assert.Single(services);
            Assert.IsType<ServiceA>(services.First());
        }

#endif

        [Fact]
        public void RegisterInstance_RegistersSingletonService()
        {
            Setup.CreateContainer();
            var instance = new ServiceA();
            Setup.Registry.RegisterInstance<IServiceA>(instance);

            var resolve1 = Setup.Container.Resolve<IServiceA>();

            Assert.IsType<ServiceA>(resolve1);
            Assert.Same(instance, resolve1);
        }

        [Fact]
        public void RegisterSingleton_LastInFirstOut()
        {
            Setup.CreateContainer();
            Setup.Registry.Register<IServiceA, ServiceA>();
            Setup.Registry.RegisterSingleton<IServiceA, CompositeService>();
            var service = Setup.Container.Resolve<IServiceA>();

            Assert.IsType<CompositeService>(service);
            Assert.Same(service, Setup.Container.Resolve<IServiceA>());
        }

#if !UNITY
        [Theory]
        [InlineData(0, typeof(ServiceA))]
        [InlineData(1, typeof(CompositeService))]
        public void RegisterSingleton_ResolveAll(int index, Type type)
        {
            Setup.CreateContainer();
            Setup.Registry.RegisterSingleton<IServiceA, ServiceA>();
            Setup.Registry.RegisterSingleton<IServiceA, CompositeService>();
            var services = Setup.Container.Resolve<IEnumerable<IServiceA>>();

            Assert.NotNull(services);
            Assert.Equal(2, services.Count());
            Assert.IsType(type, services.ElementAt(index));
        }

#endif

        [Fact]
        public void RegisterInstance_RegistersFunc()
        {
            Setup.CreateContainer();
            var instance = new ServiceA();
            Setup.Registry.RegisterInstance<IServiceA>(instance);

            var factory = Setup.Container.Resolve<Func<IServiceA>>();
            IServiceA service = null;
            Assert.NotNull(factory);
            var ex = Record.Exception(() => service = factory());
            Assert.Null(ex);
            Assert.NotNull(service);
            Assert.IsType<ServiceA>(service);
            Assert.Same(service, factory());
        }

        [Fact]
        public void RegisterInstance_RegistersLazy()
        {
            Setup.CreateContainer();
            var instance = new ServiceA();
            Setup.Registry.RegisterInstance<IServiceA>(instance);

            var factory = Setup.Container.Resolve<Lazy<IServiceA>>();
            IServiceA service = null;
            Assert.NotNull(factory);
            var ex = Record.Exception(() => service = factory.Value);
            Assert.Null(ex);
            Assert.NotNull(service);
            Assert.IsType<ServiceA>(service);
            Assert.Same(service, Setup.Container.Resolve<Lazy<IServiceA>>().Value);
        }



#if !UNITY

        [Fact]
        public void RegisterInstance_RegistersIEnumerable()
        {
            Setup.CreateContainer();
            var instance = new ServiceA();
            Setup.Registry.RegisterInstance<IServiceA>(instance);
            var services = Setup.Container.Resolve<IEnumerable<IServiceA>>();

            Assert.NotNull(services);
            Assert.Single(services);
            Assert.IsType<ServiceA>(services.First());
        }

#endif
    }
}
