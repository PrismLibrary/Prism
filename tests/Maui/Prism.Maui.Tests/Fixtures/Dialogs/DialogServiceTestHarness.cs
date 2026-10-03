using Moq;
using Prism.Common;
using Prism.Dialogs;
using Prism.Events;
using Prism.Navigation;
using Prism.Navigation.Xaml;

#nullable enable
namespace Prism.Maui.Tests.Fixtures.Dialogs;

internal sealed class DialogServiceTestHarness
{
    private readonly Mock<IContainerProvider> _provider = new();
    private readonly Mock<IDialogViewRegistry> _registry = new();
    private readonly Queue<IDialogContainer> _containers = new();
    private readonly DialogServiceTestService _service;
    public ContentPage Page { get; } = new();
    public TaskCompletionSource<IDialogResult> Result { get; set; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int Callbacks { get; private set; }

    public DialogServiceTestHarness(IDialogContainer host, DialogServiceTestModel model)
    {
        _ = new PrismWindow { Page = Page };
        _provider.Setup(p => p.Resolve(typeof(IDialogViewRegistry))).Returns(_registry.Object);
        _provider.Setup(p => p.Resolve(typeof(IDialogContainer))).Returns(() => _containers.Dequeue());
        Page.SetContainerProvider(_provider.Object);
        _service = new DialogServiceTestService(Page);
        Add("dialog", host, model);
    }

    public void Add(string name, IDialogContainer host, DialogServiceTestModel model)
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
