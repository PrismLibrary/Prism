using System;
using Prism.Ioc.Mocks.Services;
using Prism.Ioc.Tests;
using Xunit;

namespace Prism.Ioc.DryIoc.Tests
{
    public class ContainerTestsWithDefaultSingleton : IClassFixture<ContainerSetupWithDefaultSingleton>, IDisposable
    {
        private bool disposedValue;

        protected ContainerSetup Setup { get; }

        public ContainerTestsWithDefaultSingleton(ContainerSetupWithDefaultSingleton setup)
        {
            Setup = setup;
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
        public void AutomaticTransientResolutionOfConcreteType()
        {
            var container = Setup.CreateContainer();
            var resolved1 = container.Resolve<ServiceA>();
            var resolved2 = container.Resolve<ServiceA>();
            Assert.NotNull(resolved1);
            Assert.NotSame(resolved1, resolved2);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Setup.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
