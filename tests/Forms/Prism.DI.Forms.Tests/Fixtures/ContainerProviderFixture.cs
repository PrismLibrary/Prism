using System.Collections.Generic;
using Prism.DI.Forms.Tests;
using Prism.DI.Forms.Tests.Mocks;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Events;
using Prism.Forms.Tests.Mocks.Events;
using Prism.Ioc;
using Prism.Logging;
using Xunit;
using Xunit.Abstractions;

#if Autofac
namespace Prism.Autofac.Forms.Tests.Fixtures
#elif DryIoc
namespace Prism.DryIoc.Forms.Tests.Fixtures
#elif Ninject
namespace Prism.Ninject.Forms.Tests.Fixtures
#elif Unity
namespace Prism.Unity.Forms.Tests.Fixtures
#endif
{
    public class ContainerProviderFixture
    {
        ITestOutputHelper _testOutputHelper { get; }

        public ContainerProviderFixture(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            Xamarin.Forms.Mocks.MockForms.Init();
        }

        private PrismApplicationMock CreateMockApplication()
        {
            var initializer = new XunitPlatformInitializer(_testOutputHelper);
            return new PrismApplicationMock(initializer);
        }

        [Fact]
        public void CanResolveUnnamedType()
        {
            var app = CreateMockApplication();
            
            var containerProvider = new ContainerProvider<ConcreteTypeMock>();
            ConcreteTypeMock type = (ConcreteTypeMock) containerProvider;
            Assert.NotNull(type);

            Assert.IsType<ConcreteTypeMock>(type);
        }

        [Fact]
        public void CanResolvedNamedType()
        {
            var app = CreateMockApplication();
            var containerProvider = new ContainerProvider<ViewModelBMock>
            {
                Name = ViewModelBMock.Key
            };

            ViewModelBMock vm = (ViewModelBMock)containerProvider;
            Assert.NotNull(vm);
            Assert.IsType<ViewModelBMock>(vm);
        }

        [Fact]
        public void ProvidesValueFromResourceDictionary()
        {
            var app = CreateMockApplication();
            var ea = app.Container.Resolve<IEventAggregator>();
            var events = new List<string>();
            ea.GetEvent<TestActionEvent>().Subscribe((m) => events.Add(m));
            app.NavigationService.NavigateAsync("XamlViewMock");
            Assert.Contains(events, e => e == "Convert");
            Assert.NotNull(app.MainPage);
            var xamlView = app.MainPage as XamlViewMock;
            var viewModel = xamlView.BindingContext as XamlViewMockViewModel;

            xamlView.TestEntry.Text = "Foo Bar";
            Assert.Contains(events, e => e == "ConvertBack");

        }

        [Fact]
        public void ResolvesForDependencyResolver()
        {
            var app = CreateMockApplication();
            Assert.Same(app.Container.Resolve<ILoggerFacade>(), Xamarin.Forms.DependencyService.Resolve<ILoggerFacade>());
        }
    }
}