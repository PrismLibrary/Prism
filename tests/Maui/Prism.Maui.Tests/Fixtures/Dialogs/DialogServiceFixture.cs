using System.Windows.Input;
using Microsoft.Maui.Dispatching;
using Moq;
using Prism.Dialogs;
using Prism.Common;
using Prism.Events;
using Prism.Maui.Tests.Mocks;
using Prism.Navigation;
using Prism.Navigation.Xaml;

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
        var host = new DialogHost { PopGate = release.Task };
        var model = new Model();
        var test = new DialogTest(host, model);
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
        var host = new DialogHost { PushGate = release.Task };
        var model = new Model();
        if (fromOnDialogOpened) model.Opening = () => model.RequestClose.Invoke();
        else host.AfterPush = () => model.RequestClose.Invoke();
        var test = new DialogTest(host, model);
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
        var host = new DialogHost { PopGate = release.Task };
        var model = new Model();
        var test = new DialogTest(host, model);
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
        var host = new DialogHost();
        var model = new Model { AllowClose = false };
        var test = new DialogTest(host, model);
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
        var host = new DialogHost
        {
            PopGate = canceled ? Task.FromCanceled(new CancellationToken(true)) : Task.FromException(new InvalidOperationException("pop failed"))
        };
        var model = new Model();
        var test = new DialogTest(host, model);
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
        var host = new DialogHost
        {
            PushGate = canceled ? Task.FromCanceled(new CancellationToken(true)) : Task.FromException(new InvalidOperationException("push failed"))
        };
        var model = new Model();
        model.Opening = () => model.RequestClose.Invoke();
        var test = new DialogTest(host, model);
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
        var host = new DialogHost { PopGate = Task.FromException(new InvalidOperationException("blocked")) };
        var model = new Model();
        var test = new DialogTest(host, model);
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
        var host = new DialogHost();
        var model = new Model { Closing = () => throw new InvalidOperationException("lifecycle") };
        var test = new DialogTest(host, model);
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
        var host = new DialogHost();
        var model = new Model { Activating = () => throw new InvalidOperationException("activation") };
        var test = new DialogTest(host, model);
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
        var lower = new DialogHost { PopGate = release.Task };
        var upper = new DialogHost();
        var lowerModel = new Model();
        var upperModel = new Model { AllowClose = false };
        var test = new DialogTest(lower, lowerModel);
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

    [Fact]
    public async Task PendingCloseDoesNotAuthorizeAnotherWindowsDevicePop()
    {
        var otherHost = new DialogHost();
        var otherModel = new Model { AllowClose = false };
        var other = new DialogTest(otherHost, otherModel);
        other.Show();
        await otherModel.Activated.Task;
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new DialogHost { PopGate = release.Task };
        var model = new Model();
        var test = new DialogTest(host, model);
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

    private sealed class DialogTest
    {
        private readonly Mock<IContainerProvider> _provider = new();
        private readonly Mock<IDialogViewRegistry> _registry = new();
        private readonly Queue<IDialogContainer> _containers = new();
        private readonly Service _service;
        public ContentPage Page { get; } = new();
        public TaskCompletionSource<IDialogResult> Result { get; set; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int Callbacks { get; private set; }

        public DialogTest(DialogHost host, Model model)
        {
            _ = new PrismWindow { Page = Page };
            _provider.Setup(p => p.Resolve(typeof(IDialogViewRegistry))).Returns(_registry.Object);
            _provider.Setup(p => p.Resolve(typeof(IDialogContainer))).Returns(() => _containers.Dequeue());
            Page.SetContainerProvider(_provider.Object);
            _service = new Service(Page);
            Add("dialog", host, model);
        }

        public void Add(string name, DialogHost host, Model model)
        {
            _containers.Enqueue(host);
            _registry.Setup(r => r.CreateView(_provider.Object, name)).Returns(new ContentView { BindingContext = model });
        }

        public Task<INavigationResult> GoBackAsync()
        {
            var accessor = new Mock<IPageAccessor>();
            accessor.SetupGet(a => a.Page).Returns(Page);
            var windows = new Mock<IWindowManager>();
            windows.SetupGet(w => w.Windows).Returns(new[] { (Window)Page.Parent });
            return new PageNavigationService(_provider.Object, windows.Object, new EventAggregator(), accessor.Object)
                .GoBackAsync(new NavigationParameters());
        }

        public void Show(string name = "dialog", DialogCallback? callback = null) => _service.ShowDialog(name, new DialogParameters(),
            callback ?? new DialogCallback().OnClose(result => { Callbacks++; Result.TrySetResult(result); }));
    }

    private sealed class Service(Page page) : DialogServiceBase
    {
        protected override Page GetCurrentPage() => page;
    }

    private sealed class DialogHost : ContentPage, IDialogContainer
    {
        public Task PushGate { get; set; } = Task.CompletedTask;
        public Task PopGate { get; set; } = Task.CompletedTask;
        public Action? AfterPush { get; set; }
        public TaskCompletionSource PopEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int PopCount { get; private set; }
        public View DialogView { get; private set; } = null!;
        public ICommand Dismiss { get; private set; } = null!;

        public async Task ConfigureLayout(Page page, View view, bool hide, ICommand dismiss, IDialogParameters parameters)
        {
            DialogView = view;
            Dismiss = dismiss;
            Content = view;
            await PushGate;
            await page.Navigation.PushModalAsync(this, false);
            IDialogContainer.DialogStack.Add(this);
            AfterPush?.Invoke();
        }

        public async Task DoPop(Page page)
        {
            PopCount++;
            PopEntered.TrySetResult();
            await PopGate;
            if (page.Navigation.ModalStack.LastOrDefault() != this)
                throw new InvalidOperationException("Another modal covers this dialog.");
            await page.Navigation.PopModalAsync(false);
            if (page.Navigation.ModalStack.Contains(this))
                throw new InvalidOperationException("PrismWindow canceled the native pop.");
            IDialogContainer.DialogStack.Remove(this);
        }
    }

    private sealed class Model : IDialogAware, IActiveAware
    {
        private bool _active;
        public List<string> Events { get; } = [];
        public TaskCompletionSource Activated { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource ClosedTask { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Action? Opening { get; set; }
        public Action? Closing { get; set; }
        public Action? Activating { get; set; }
        public bool AllowClose { get; set; } = true;
        public int Closed { get; private set; }
        public int CanCloseChecks { get; private set; }
        public DialogCloseListener RequestClose { get; }
        public bool CanCloseDialog() { CanCloseChecks++; return AllowClose; }
        public void OnDialogOpened(IDialogParameters parameters) { Events.Add("opened"); Opening?.Invoke(); }
        public void OnDialogClosed() { Closed++; Events.Add("closed"); ClosedTask.TrySetResult(); Closing?.Invoke(); }
        public bool IsActive
        {
            get => _active;
            set { _active = value; Events.Add(value ? "active" : "inactive"); if (value) { Activating?.Invoke(); Activated.TrySetResult(); } IsActiveChanged?.Invoke(this, EventArgs.Empty); }
        }
        public event EventHandler? IsActiveChanged;
    }
}

[CollectionDefinition("Dialog service lifecycle", DisableParallelization = true)]
public class DialogServiceCollection;
