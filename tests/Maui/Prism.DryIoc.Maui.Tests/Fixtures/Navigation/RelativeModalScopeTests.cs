using Prism.Common;
using Prism.DryIoc.Maui.Tests.Mocks;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Navigation.Xaml;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class RelativeModalScopeTests : TestBase
{
    public RelativeModalScopeTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task ForwardNavigation_KeepsPoppedScopeAliveUntilSlowInitializationCompletes()
    {
        var dispatcher = new DeferredCleanupDispatcher();
        DispatcherProvider.SetCurrent(dispatcher);
        try
        {
            var app = CreateBuilder(prism => prism.RegisterTypes(container =>
                    container.RegisterForNavigation<MockSlowRelativePage>())
                .CreateWindow("NavigationPage/MockViewA")).Build();
            var window = GetWindow(app);
            var root = window.CurrentPage;
            Assert.True((await root.GetContainerProvider().Resolve<INavigationService>()
                .NavigateAsync("MockViewB?useModalNavigation=true")).Success);
            var modal = window.CurrentPage;
            var scope = modal.GetContainerProvider();
            var service = scope.Resolve<INavigationService>();
            var parameters = new NavigationParameters
            {
                { "checkScope", (Action)(() =>
                    {
                        dispatcher.RunPending();
                        Assert.Same(scope, modal.GetContainerProvider());
                        Assert.Same(modal, scope.Resolve<IPageAccessor>().Page);
                    })
                }
            };

            // NavigationPage forward segments are initialized in reverse order.
            var result = await service.NavigateAsync("../MockViewC/MockSlowRelativePage", parameters);

            Assert.True(result.Success, result.Exception?.ToString());
            Assert.Empty(window.Navigation.ModalStack);
            Assert.Collection(window.Page.Navigation.NavigationStack,
                page => Assert.Same(root, page),
                page => Assert.IsType<MockViewC>(page),
                page => Assert.IsType<MockSlowRelativePage>(page));
            dispatcher.RunPending();
            Assert.Null(modal.BindingContext);
        }
        finally
        {
            DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        }
    }
}
