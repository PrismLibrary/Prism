using Prism.Ioc;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;
using Xunit;
using Xunit.Abstractions;

namespace Prism.Container.Wpf.Tests.Ioc
{
    public class ContainerProviderExtensionFixture : IDisposable
    {
        private static readonly MockService _unnamedService = new MockService();
        private static readonly IReadOnlyDictionary<string, MockService> _namedServiceDictionary = new Dictionary<string, MockService>
        {
            ["A"] = new MockService(),
            ["B"] = new MockService(),
        };

        public ContainerProviderExtensionFixture()
        {
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => ContainerHelper.CreateContainerExtension());
            var containerExtension = ContainerLocator.Current;
            containerExtension.RegisterInstance<IService>(_unnamedService);
            foreach (var kvp in _namedServiceDictionary)
            {
                containerExtension.RegisterInstance<IService>(kvp.Value, kvp.Key);
            }
            containerExtension.FinalizeExtension();
        }

        public void Dispose()
        {
            ContainerLocator.ResetContainer();
        }

        [Fact]
        public void CanResolveUnnamedServiceUsingConstructor()
        {
            var containerProvider = new ContainerProviderExtension(typeof(IService));
            var service = containerProvider.ProvideValue(null);

            Assert.Same(_unnamedService, service);
        }

        [Fact]
        public void CanResolveUnnamedServiceUsingProperty()
        {
            var containerProvider = new ContainerProviderExtension
            {
                Type = typeof(IService)
            };
            var service = containerProvider.ProvideValue(null);

            Assert.Same(_unnamedService, service);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        public void CanResolvedNamedServiceUsingConstructor(string name)
        {
            var expectedService = _namedServiceDictionary[name];

            var containerProvider = new ContainerProviderExtension(typeof(IService))
            {
                Name = name,
            };
            var service = containerProvider.ProvideValue(null);

            Assert.Same(expectedService, service);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        public void CanResolvedNamedServiceUsingProperty(string name)
        {
            var expected = _namedServiceDictionary[name];

            var containerProvider = new ContainerProviderExtension()
            {
                Type = typeof(IService),
                Name = name,
            };
            var service = containerProvider.ProvideValue(null);

            Assert.Same(expected, service);
        }

        [WpfFact]
        public void CanResolveServiceFromMarkupExtension()
        {
            var xaml =
@"<Window 
  xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
  xmlns:prism='http://prismlibrary.com/'
  xmlns:mocks='clr-namespace:Prism.IocContainer.Wpf.Tests.Support.Mocks;assembly=Prism.IocContainer.Wpf.Tests.Support'
  DataContext='{prism:ContainerProvider mocks:IService}' />";

            using (var reader = new StringReader(xaml))
            {
                var window = XamlServices.Load(reader) as Window;

                Assert.Same(_unnamedService, window.DataContext);
            }
        }

        [WpfFact]
        public void CanResolveServiceFromXmlElement()
        {
            var xaml =
@"<Window
  xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
  xmlns:prism='clr-namespace:Prism.Ioc;assembly=Prism.Wpf'
  xmlns:mocks='clr-namespace:Prism.IocContainer.Wpf.Tests.Support.Mocks;assembly=Prism.IocContainer.Wpf.Tests.Support'>
  <Window.DataContext>
    <prism:ContainerProvider Type='mocks:IService' />
  </Window.DataContext>
</Window>";

            using (var reader = new StringReader(xaml))
            {
                var window = XamlServices.Load(reader) as Window;

                Assert.Same(_unnamedService, window.DataContext);
            }
        }
    }
}
