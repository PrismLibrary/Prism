using Microsoft.Maui;
using Moq;
using Prism.Maui.Tests.Mocks.Shell;
using Prism.Navigation.Xaml;

namespace Prism.Maui.Tests.Fixtures.Navigation.Shell;

public sealed class ShellBackNavigationFixture
{
    [Fact]
    public async Task ShellPushAndBackTransferObjectsButDoNotReleaseThePrismDetailScope()
    {
        using var host = new ShellProbeHarness();
        var root = host.CreatePage();
        var shell = new Microsoft.Maui.Controls.Shell();
        shell.Items.Add(new ShellContent { Route = "home", Content = root });
        var handler = new Mock<IViewHandler>();
        handler.SetupGet(view => view.MauiContext).Returns(new MauiContext(host.Services));
        shell.Handler = handler.Object;
        _ = new Window(shell);
        await shell.GoToAsync("//home", false);
        var route = $"prism-shell-back-probe-{Guid.NewGuid():N}";
        Routing.RegisterRoute(route, new ShellRouteFactoryPrototype("detail"));
        ShellProbePage detail = null;
        try
        {
            var payload = new object();
            await shell.GoToAsync(route, false,
                new ShellNavigationQueryParameters { { "payload", payload } }).WaitAsync(TimeSpan.FromSeconds(5));
            detail = host.Track(Assert.IsType<ShellProbePage>(shell.CurrentPage));
            var model = Assert.IsType<ShellProbeViewModel>(detail.BindingContext);

            Assert.NotSame(root, detail);
            Assert.Same(payload, model.Query["payload"]);
            Assert.NotSame(root.GetContainerProvider(), detail.GetContainerProvider());
            Assert.Equal(0, model.InitializationCount);

            var result = new object();
            await shell.GoToAsync("..", false,
                new ShellNavigationQueryParameters { { "result", result } }).WaitAsync(TimeSpan.FromSeconds(5));

            Assert.Same(root, shell.CurrentPage);
            var rootModel = Assert.IsType<ShellProbeViewModel>(root.BindingContext);
            Assert.Same(result, rootModel.Query["result"]);
            Assert.False(rootModel.Query.ContainsKey("payload"));
            Assert.Equal(0, model.NavigatedFromCount);
            Assert.False(detail.Destroyed);
            Assert.Equal(0, detail.Dependency.DisposeCount);
            Assert.Equal(0, root.Dependency.DisposeCount);
        }
        finally
        {
            detail?.Behaviors.Clear();
            detail?.SetContainerProvider(null);
            Routing.UnRegisterRoute(route);
        }
    }
}
