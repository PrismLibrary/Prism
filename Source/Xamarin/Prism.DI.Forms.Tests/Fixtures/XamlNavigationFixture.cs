using Xunit;
using Xunit.Abstractions;
using Prism.DI.Forms.Tests.Fixtures;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Xaml.Internals;
using Prism.DI.Forms.Tests.Mocks.Internals;
using Xamarin.Forms.Internals;
using Prism.Navigation.Xaml;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using Prism.DI.Forms.Tests.Mocks.Views;

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
    public class XamlNavigationFixture : FixtureBase
    {
        public XamlNavigationFixture(ITestOutputHelper testOutputHelper) 
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void RequiresServiceProvider()
        {
            var navigateExtension = new NavigateToExtension();
            var ex = Record.Exception(() => navigateExtension.ProvideValue(null));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void ProvidesCommand()
        {
            var serviceProvider = new XamlServiceProvider();
            var navigateExtension = new NavigateToExtension();

            object providedValue = null;
            var ex = Record.Exception(() => providedValue = navigateExtension.ProvideValue(serviceProvider));

            Assert.Null(ex);
            Assert.NotNull(providedValue);
            Assert.IsAssignableFrom<ICommand>(providedValue);
        }

        [Fact]
        public async Task ResolvesParentPage()
        {
            var app = CreateMockApplication();

            Log.Listeners.Clear();
            var logObserver = new FormsLogObserver();
            Log.Listeners.Add(logObserver);

            var layout = new Button();
            var serviceProvider = new XamlServiceProvider();
            serviceProvider.Add(typeof(IProvideValueTarget), new XamlValueTargetProvider(layout, "Command"));

            var navigateExtension = new NavigateToExtension()
            {
                Name = "/AutowireView"
            };
            navigateExtension.ProvideValue(serviceProvider);

            app.MainPage = new NavigationPage(new ContentPage
            {
                Content = layout
            });

            Assert.NotNull(layout.Parent);
            Assert.IsType<ContentPage>(layout.Parent);

            Assert.NotNull(navigateExtension.SourcePage);
            Assert.True(navigateExtension.CanExecute(null));
            var ex = await Record.ExceptionAsync(async () =>
            {
                navigateExtension.Execute(null);
                if (navigateExtension.IsNavigating)
                    await Task.Delay(100);
            });
            Assert.Null(ex);
            Assert.Empty(logObserver.Logs);

            Assert.IsType<AutowireView>(app.MainPage);
        }

        [Fact]
        public async Task LogsNavigationErrors_WithoutThrowingException()
        {
            var app = CreateMockApplication();

            Log.Listeners.Clear();
            var logObserver = new FormsLogObserver();
            Log.Listeners.Add(logObserver);

            var layout = new Button();
            var serviceProvider = new XamlServiceProvider();
            serviceProvider.Add(typeof(IProvideValueTarget), new XamlValueTargetProvider(layout, "Command"));

            var navigateExtension = new NavigateToExtension()
            {
                Name = "/NonExistentPage"
            };
            navigateExtension.ProvideValue(serviceProvider);

            app.MainPage = new NavigationPage(new ContentPage
            {
                Content = layout
            });

            Assert.NotNull(layout.Parent);
            Assert.IsType<ContentPage>(layout.Parent);

            Assert.NotNull(navigateExtension.SourcePage);
            Assert.True(navigateExtension.CanExecute(null));
            var ex = await Record.ExceptionAsync(async () =>
            {
                navigateExtension.Execute(null);
                if (navigateExtension.IsNavigating)
                    await Task.Delay(100);
            });
            Assert.Null(ex);
            Assert.NotEmpty(logObserver.Logs);

            Assert.IsType<NavigationPage>(app.MainPage);
        }

        [Fact]
        public void UsesMasterDetailPage_WhenParentIsMaster()
        {
            var app = CreateMockApplication();

            var layout = new Button();
            var serviceProvider = new XamlServiceProvider();
            serviceProvider.Add(typeof(IProvideValueTarget), new XamlValueTargetProvider(layout, "Command"));

            var navigateExtension = new NavigateToExtension()
            {
                Name = "SomeOtherView"
            };
            navigateExtension.ProvideValue(serviceProvider);

            app.MainPage = new MasterDetailPage
            {
                Master = new ContentPage
                {
                    Title = "Test",
                    Content = layout
                },
                Detail = new NavigationPage(new ContentPage())
            };

            Assert.NotNull(layout.Parent);
            Assert.IsType<ContentPage>(layout.Parent);

            Assert.NotNull(navigateExtension.SourcePage);
            Assert.IsType<MasterDetailPage>(navigateExtension.SourcePage);
        }
    }
}