using Microsoft.Maui.Dispatching;
using Prism.Dialogs;
using Prism.Maui.Tests.Mocks;
using Prism.Navigation;

#nullable enable
namespace Prism.Maui.Tests.Fixtures.Dialogs;

[Collection("Dialog service lifecycle")]
public class DialogServiceFixture : IDisposable
{
    public DialogServiceFixture() => DispatcherProvider.SetCurrent(TestDispatcher.Provider);

    public void Dispose()
    {
        IDialogContainer.DialogStack.Clear();
        PageNavigationService.NavigationSource = PageNavigationSource.Device;
    }

    [Fact]
    public async Task RepeatedCloseDoesNotResetSourceWhileNativePopIsPending()
    {
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new DialogServiceTestHost { PopGate = release.Task };
        var model = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(host, model);
        test.Show();
        await model.Activated.Task;
        model.RequestClose.Invoke(ButtonResult.OK);
        await host.PopEntered.Task;
        model.RequestClose.Invoke(ButtonResult.Cancel);
        Assert.Equal(PageNavigationSource.DialogService, PageNavigationService.NavigationSource);
        Assert.Equal(1, host.PopCount);
        release.SetResult();
        var result = await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(ButtonResult.OK, result.Result);
        Assert.Null(result.Exception);
        Assert.Equal(1, model.Closed);
        Assert.Equal(1, test.Callbacks);
        Assert.Empty(test.Page.Navigation.ModalStack);
        model.RequestClose.Invoke();
        Assert.Equal(1, host.PopCount);
        Assert.Equal(1, test.Callbacks);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task EarlyCloseWaitsForHostingAndActivation(bool fromOnDialogOpened)
    {
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new DialogServiceTestHost { PushGate = release.Task };
        var model = new DialogServiceTestModel();
        if (fromOnDialogOpened) model.Opening = () => model.RequestClose.Invoke();
        else host.AfterPush = () => model.RequestClose.Invoke();
        var test = new DialogServiceTestHarness(host, model);
        test.Show();
        Assert.Equal(0, host.PopCount);
        Assert.False(model.IsActive);
        release.SetResult();
        var result = await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Null(result.Exception);
        Assert.Equal(new[] { "opened", "active", "inactive", "closed" }, model.Events);
        Assert.Equal(1, test.Callbacks);
        Assert.DoesNotContain(host, IDialogContainer.DialogStack);
    }

    [Fact]
    public async Task GoBackDuringPendingDialogCloseDoesNotResetItsNavigationSource()
    {
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new DialogServiceTestHost { PopGate = release.Task };
        var model = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(host, model);
        test.Show();
        await model.Activated.Task;
        model.RequestClose.Invoke();
        await host.PopEntered.Task;
        try
        {
            await test.GoBackAsync();
        }
        finally
        {
            release.TrySetResult();
            await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5));
        }
        Assert.Null((await test.Result.Task).Exception);
        Assert.Equal(1, host.PopCount);
        Assert.Equal(1, model.Closed);
    }

    [Fact]
    public async Task VetoCanBeRetriedWithoutCallbackOrLifecycleDuplication()
    {
        var host = new DialogServiceTestHost();
        var model = new DialogServiceTestModel { AllowClose = false };
        var test = new DialogServiceTestHarness(host, model);
        test.Show();
        await model.Activated.Task;
        model.RequestClose.Invoke();
        Assert.Equal(0, host.PopCount);
        Assert.Equal(0, test.Callbacks);
        Assert.Contains(host, IDialogContainer.DialogStack);
        model.AllowClose = true;
        model.RequestClose.Invoke();
        await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(1, model.Closed);
        Assert.Equal(1, test.Callbacks);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailedOrCanceledRemovalCanBeRetried(bool canceled)
    {
        var host = new DialogServiceTestHost
        {
            PopGate = canceled ? Task.FromCanceled(new CancellationToken(true)) : Task.FromException(new InvalidOperationException("pop failed"))
        };
        var model = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(host, model);
        test.Show();
        await model.Activated.Task;
        model.RequestClose.Invoke();
        Assert.NotNull((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Contains(host, IDialogContainer.DialogStack);
        Assert.True(model.IsActive);
        Assert.Equal(0, model.Closed);
        test.Result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        host.PopGate = Task.CompletedTask;
        model.RequestClose.Invoke();
        Assert.Null((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Equal(1, model.Closed);
        Assert.Equal(2, host.PopCount);
        Assert.DoesNotContain(host, IDialogContainer.DialogStack);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailedOrCanceledShowIsTerminalAndResetsSource(bool canceled)
    {
        var host = new DialogServiceTestHost
        {
            PushGate = canceled ? Task.FromCanceled(new CancellationToken(true)) : Task.FromException(new InvalidOperationException("push failed"))
        };
        var model = new DialogServiceTestModel();
        model.Opening = () => model.RequestClose.Invoke();
        var test = new DialogServiceTestHarness(host, model);
        test.Show();
        Assert.NotNull((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        Assert.False(model.IsActive);
        Assert.Equal(0, host.PopCount);
        model.RequestClose.Invoke();
        Assert.Equal(1, test.Callbacks);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ErrorCallbackCanRetryTheFailedClose(bool asynchronous)
    {
        var host = new DialogServiceTestHost { PopGate = Task.FromException(new InvalidOperationException("blocked")) };
        var model = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(host, model);
        var closed = new TaskCompletionSource<IDialogResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var errors = 0;
        void Retry()
        {
            errors++;
            host.PopGate = Task.CompletedTask;
            model.RequestClose.Invoke();
        }
        var callback = new DialogCallback().OnClose(result => closed.TrySetResult(result));
        callback = asynchronous ? callback.OnErrorAsync(async () => { await Task.Yield(); Retry(); }) : callback.OnError(Retry);
        test.Show(callback: callback);
        await model.Activated.Task;
        model.RequestClose.Invoke();
        Assert.Null((await closed.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Equal(1, errors);
        Assert.Equal(1, model.Closed);
        Assert.Equal(2, host.PopCount);
    }

    [Fact]
    public async Task LifecycleExceptionAfterRemovalDoesNotAllowAnotherClose()
    {
        var host = new DialogServiceTestHost();
        var model = new DialogServiceTestModel { Closing = () => throw new InvalidOperationException("lifecycle") };
        var test = new DialogServiceTestHarness(host, model);
        test.Show();
        await model.Activated.Task;
        model.RequestClose.Invoke();
        Assert.NotNull((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        model.RequestClose.Invoke();
        Assert.Equal(1, host.PopCount);
        Assert.Equal(1, model.Closed);
        Assert.Equal(1, test.Callbacks);
    }

    [Fact]
    public async Task ActivationExceptionDoesNotTrapAnAlreadyHostedDialog()
    {
        var host = new DialogServiceTestHost();
        var model = new DialogServiceTestModel { Activating = () => throw new InvalidOperationException("activation") };
        var test = new DialogServiceTestHarness(host, model);
        test.Show();
        Assert.NotNull((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Contains(host, IDialogContainer.DialogStack);
        test.Result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        model.RequestClose.Invoke();
        Assert.Null((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Equal(1, model.Closed);
        Assert.DoesNotContain(host, IDialogContainer.DialogStack);
    }

    [Fact]
    public async Task CoveredCloseCannotAuthorizeDeviceDismissalOfAnotherDialog()
    {
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var lower = new DialogServiceTestHost { PopGate = release.Task };
        var upper = new DialogServiceTestHost();
        var lowerModel = new DialogServiceTestModel();
        var upperModel = new DialogServiceTestModel { AllowClose = false };
        var test = new DialogServiceTestHarness(lower, lowerModel);
        test.Show();
        await lowerModel.Activated.Task;
        test.Add("upper", upper, upperModel);
        test.Show("upper");
        await upperModel.Activated.Task;
        lowerModel.RequestClose.Invoke();
        await lower.PopEntered.Task;
        try
        {
            var checks = upperModel.CanCloseChecks;
            await test.Page.Navigation.PopModalAsync(false);
            Assert.Equal(2, test.Page.Navigation.ModalStack.Count);
            Assert.True(upperModel.CanCloseChecks > checks);
        }
        finally { release.TrySetResult(); }
        Assert.NotNull((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Contains(lower, IDialogContainer.DialogStack);
        Assert.Contains(upper, IDialogContainer.DialogStack);
        upperModel.AllowClose = true;
        upperModel.RequestClose.Invoke();
        await upperModel.ClosedTask.Task.WaitAsync(TimeSpan.FromSeconds(5));
        test.Result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        lowerModel.RequestClose.Invoke();
        Assert.Null((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Empty(test.Page.Navigation.ModalStack);
        Assert.Equal(1, lowerModel.Closed);
        Assert.Equal(1, upperModel.Closed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CoveredProductionContainerRetainsStateAndCanBeRetried(bool upperAllowsClose)
    {
        var lower = new DialogContainerPage();
        var upper = new DialogContainerPage();
        var lowerModel = new DialogServiceTestModel();
        var upperModel = new DialogServiceTestModel { AllowClose = upperAllowsClose };
        var test = new DialogServiceTestHarness(lower, lowerModel);
        test.Page.Resources.Add(Prism.Dialogs.Xaml.DialogLayout.PopupOverlayStyle, new Style(typeof(BoxView)));
        test.Show();
        await lowerModel.Activated.Task.WaitAsync(TimeSpan.FromSeconds(5));
        test.Add("upper", upper, upperModel);
        test.Show("upper");
        await upperModel.Activated.Task.WaitAsync(TimeSpan.FromSeconds(5));

        lowerModel.RequestClose.Invoke();
        var result = await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Contains(lower, IDialogContainer.DialogStack);
        Assert.Contains(upper, IDialogContainer.DialogStack);
        Assert.Equal(new Page[] { lower, upper }, test.Page.Navigation.ModalStack);
        Assert.NotNull(result.Exception);
        Assert.Equal(0, lowerModel.Closed);
        Assert.Equal(0, upperModel.Closed);

        upperModel.AllowClose = true;
        upperModel.RequestClose.Invoke();
        await upperModel.ClosedTask.Task.WaitAsync(TimeSpan.FromSeconds(5));
        test.Result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        lowerModel.RequestClose.Invoke();
        Assert.Null((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Empty(test.Page.Navigation.ModalStack);
        Assert.Equal(1, lowerModel.Closed);
        Assert.Equal(1, upperModel.Closed);
    }

    [Fact]
    public async Task CanceledProductionPopRetainsStateAndCanBeRetried()
    {
        var host = new DialogContainerPage();
        var model = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(host, model);
        test.Page.Resources.Add(Prism.Dialogs.Xaml.DialogLayout.PopupOverlayStyle, new Style(typeof(BoxView)));
        test.Show();
        await model.Activated.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var window = (Window)test.Page.Parent;
        void CancelPop(object? sender, ModalPoppingEventArgs args) => args.Cancel = true;
        window.ModalPopping += CancelPop;
        try
        {
            model.RequestClose.Invoke();
            var result = await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Contains(host, IDialogContainer.DialogStack);
            Assert.Same(host, Assert.Single(test.Page.Navigation.ModalStack));
            Assert.NotNull(result.Exception);
            Assert.True(model.IsActive);
            Assert.Equal(0, model.Closed);
        }
        finally { window.ModalPopping -= CancelPop; }

        test.Result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        model.RequestClose.Invoke();
        Assert.Null((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Empty(test.Page.Navigation.ModalStack);
        Assert.Equal(1, model.Closed);
    }

    [Fact]
    public async Task PendingCloseDoesNotAuthorizeAnotherWindowsDevicePop()
    {
        var otherHost = new DialogServiceTestHost();
        var otherModel = new DialogServiceTestModel { AllowClose = false };
        var other = new DialogServiceTestHarness(otherHost, otherModel);
        other.Show();
        await otherModel.Activated.Task;
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new DialogServiceTestHost { PopGate = release.Task };
        var model = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(host, model);
        test.Show();
        await model.Activated.Task;
        model.RequestClose.Invoke();
        await host.PopEntered.Task;
        try
        {
            var checks = otherModel.CanCloseChecks;
            await other.Page.Navigation.PopModalAsync(false);
            Assert.Same(otherHost, Assert.Single(other.Page.Navigation.ModalStack));
            Assert.True(otherModel.CanCloseChecks > checks);
        }
        finally { release.TrySetResult(); }
        Assert.Null((await test.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Equal(0, otherModel.Closed);
        otherModel.AllowClose = true;
        otherModel.RequestClose.Invoke();
        Assert.Null((await other.Result.Task.WaitAsync(TimeSpan.FromSeconds(5))).Exception);
        Assert.Empty(other.Page.Navigation.ModalStack);
    }

}
