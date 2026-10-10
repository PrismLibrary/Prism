using Prism.Common;
using Prism.Maui.Tests.Mocks.Shell;
using Prism.Navigation;

namespace Prism.Maui.Tests.Fixtures.Navigation.Shell;

// These characterize the remaining boundary; they intentionally do not claim
// that the current PageNavigationService supports MAUI Shell.
public sealed class ShellCompatibilityFixture
{
    [Fact]
    public void PrismCurrentPageTraversalDoesNotYetSupportMauiShell()
    {
        using var host = new ShellProbeHarness();
        var shell = new Microsoft.Maui.Controls.Shell();
        shell.Items.Add(new ShellContent { Route = "home", Content = host.CreatePage() });

        Assert.Throws<NotSupportedException>(() => MvvmHelpers.GetCurrentPage(shell));
    }

    [Fact]
    public void DestroyingShellDoesNotYetDestroyItsCachedRootPages()
    {
        using var host = new ShellProbeHarness();
        var page = host.CreatePage();
        var shell = new Microsoft.Maui.Controls.Shell();
        shell.Items.Add(new ShellContent { Route = "home", Content = page });

        MvvmHelpers.DestroyPage(shell);

        Assert.False(page.Destroyed);
        Assert.Equal(0, page.Dependency.DisposeCount);
    }

    [Fact]
    public async Task ShellQueryDeliveryDoesNotInvokePrismInitializationOrNavigationCallbacks()
    {
        using var host = new ShellProbeHarness();
        var page = host.CreatePage();
        var model = Assert.IsType<ShellProbeViewModel>(page.BindingContext);
        var shell = new Microsoft.Maui.Controls.Shell();
        shell.Items.Add(new ShellContent { Route = "home", Content = page });
        var window = new Window(shell);
        var payload = new object();
        var query = new ShellNavigationQueryParameters { { "payload", payload } };

        await shell.GoToAsync("//home", false, query);

        Assert.Same(payload, model.Query["payload"]);
        Assert.Same(payload, query["payload"]);
        await shell.GoToAsync("//home", false);
        Assert.False(model.Query.ContainsKey("payload"));
        Assert.Equal(0, model.InitializationCount);
        Assert.Equal(0, model.NavigatedToCount);
        Assert.Equal(0, model.NavigatedFromCount);
        Assert.Same(shell, window.Page);
    }

    [Fact]
    public async Task CancelledShellNavigationCompletesNormallyWithoutConstructingTheDestination()
    {
        using var host = new ShellProbeHarness();
        var shell = new Microsoft.Maui.Controls.Shell();
        shell.Items.Add(new ShellContent { Route = "home", Content = host.CreatePage() });
        var created = 0;
        shell.Items.Add(new ShellContent
        {
            Route = "next",
            ContentTemplate = new DataTemplate(() =>
            {
                created++;
                return host.CreatePage();
            })
        });
        _ = new Window(shell);
        await shell.GoToAsync("//home", false);
        var before = shell.CurrentState.Location;
        var cancelled = false;
        shell.Navigating += (_, args) =>
        {
            args.Cancel();
            cancelled = args.Cancelled;
        };

        await shell.GoToAsync("//next", false);

        Assert.True(cancelled);
        Assert.Equal(before, shell.CurrentState.Location);
        Assert.Equal(0, created);
    }

    [Fact]
    public async Task AsyncConfirmationCanCancelThroughAShellDeferral()
    {
        using var host = new ShellProbeHarness();
        var shell = new Microsoft.Maui.Controls.Shell();
        shell.Items.Add(new ShellContent { Route = "home", Content = host.CreatePage() });
        shell.Items.Add(new ShellContent { Route = "next", Content = host.CreatePage() });
        _ = new Window(shell);
        await shell.GoToAsync("//home", false);
        var before = shell.CurrentState.Location;
        ShellNavigatingEventArgs pending = null;
        ShellNavigatingDeferral deferral = null;
        shell.Navigating += (_, args) =>
        {
            pending = args;
            deferral = args.GetDeferral();
        };

        var navigation = shell.GoToAsync("//next", false);
        Assert.NotNull(pending);
        Assert.False(navigation.IsCompleted);

        pending.Cancel();
        deferral.Complete();
        await navigation;

        Assert.Equal(before, shell.CurrentState.Location);
        Assert.True(pending.Cancelled);
    }
}
