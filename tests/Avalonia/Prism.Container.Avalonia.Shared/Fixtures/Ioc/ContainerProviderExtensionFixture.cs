/*
 * TODO: Fix me for Avalonia
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Prism.Ioc;
using Prism.IocContainer.Avalonia.Tests.Support.Mocks;
using Xunit;

namespace Prism.Container.Avalonia.Tests.Ioc
{
    public class ContainerProviderExtensionFixture : IDisposable
    {
        private static readonly MockService _unnamedService = new MockService();
        private static readonly IReadOnlyDictionary<string, MockService> _namedServiceDictionary = new Dictionary<string, MockService>
        {
            ["A"] = new MockService(),
            ["B"] = new MockService(),
        };

        private static readonly IContainerExtension _containerExtension
             = ContainerHelper.CreateContainerExtension();

        static ContainerProviderExtensionFixture()
        {
            // Preload assembly to resolve 'xmlns:prism' on xaml.
            Assembly.Load("Prism.Avalonia");

            _containerExtension.RegisterInstance<IService>(_unnamedService);
            foreach (var kvp in _namedServiceDictionary)
            {
                _containerExtension.RegisterInstance<IService>(kvp.Value, kvp.Key);
            }
        }

        public ContainerProviderExtensionFixture()
        {
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(_containerExtension);
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
            var expectedService = _namedServiceDictionary[name];

            var containerProvider = new ContainerProviderExtension()
            {
                Type = typeof(IService),
                Name = name,
            };
            var service = containerProvider.ProvideValue(null);

            Assert.Same(expectedService, service);
        }

        private const string _xamlWithMarkupExtension =
@"<Window 
  xmlns=""https://github.com/avaloniaui""
  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
  xmlns:prism='http://prismlibrary.com/'
  xmlns:mocks='clr-namespace:Prism.IocContainer.Avalonia.Tests.Support.Mocks;assembly=Prism.IocContainer.Avalonia.Tests.Support'
  DataContext='{prism:ContainerProvider mocks:IService}' />";

        private const string _xamlWithXmlElement =
@"<Window
  xmlns=""https://github.com/avaloniaui""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  xmlns:prism='http://prismlibrary.com/'
  xmlns:mocks='clr-namespace:Prism.IocContainer.Avalonia.Tests.Support.Mocks;assembly=Prism.IocContainer.Avalonia.Tests.Support'>
  <Window.DataContext>
    <prism:ContainerProvider Type='mocks:IService' />
  </Window.DataContext>
</Window>";

        [Theory]
        [InlineData(_xamlWithMarkupExtension)]
        [InlineData(_xamlWithXmlElement)]
        public void CanResolveServiceFromXaml(string xaml)
        {
            // Don't use StaTheoryAttribute. 
            // If use StaTheoryAttribute, ContainerLocator.Current will be changed by other test method
            // and Window.DataContext will be null.

            object dataContext = null;
            try
            {
                var thread = new Thread(() =>
                {
                    ////using (var reader = new StringReader(xaml))
                    ////{
                    ////    var window = XamlServices.Load(reader) as Window;
                    ////    dataContext = window.DataContext;
                    ////}

                    var window = AvaloniaRuntimeXamlLoader.Load(xaml) as Window;
                    dataContext = window.DataContext;
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Issue resolving AXAML: " + ex);
                Debug.WriteLine("Issue resolving AXAML: " + ex);
            }

            Assert.Same(_unnamedService, dataContext);
        }
    }
}
*/
