using Prism.Windows;

namespace Playground.Module.ViewModels;

internal class WindowLabViewModel : BindableBase
{
    private readonly IWindowManager _windows;
    private readonly IWindowContext _origin;
    private readonly IDialogService _dialogs;

    public WindowLabViewModel(IWindowManager windows, IWindowContext origin, IDialogService dialogs)
    {
        _windows = windows;
        _origin = origin;
        _dialogs = dialogs;
        RefreshCommand = new DelegateCommand(Refresh);
        ShowOriginCommand = new DelegateCommand(() => Show(_dialogs, "originating window"));
        ActivateThenShowOriginCommand = new DelegateCommand(ActivateThenShowOrigin, () => SelectedWindow is not null);
        ShowTargetCommand = new DelegateCommand(ShowTarget, () => SelectedWindow is not null);
        Refresh();
    }

    public IReadOnlyList<IWindowContext> Targets
    {
        get;
        private set => SetProperty(ref field, value);
    } = Array.Empty<IWindowContext>();

    public IWindowContext? SelectedWindow
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                ActivateThenShowOriginCommand.RaiseCanExecuteChanged();
                ShowTargetCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Status
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ShowOriginCommand { get; }
    public DelegateCommand ActivateThenShowOriginCommand { get; }
    public DelegateCommand ShowTargetCommand { get; }

    private void Refresh()
    {
        Targets = _windows.Windows.Where(window => !window.IsClosed).ToArray();
        SelectedWindow = Targets.FirstOrDefault(window => !ReferenceEquals(window, _origin)) ?? Targets.FirstOrDefault();
    }

    private void ActivateThenShowOrigin()
    {
        if (SelectedWindow is null || SelectedWindow.IsClosed)
        {
            Status = "The selected window is closed. Refresh the window list.";
            return;
        }
        SelectedWindow.Window.Activate();
        Show(_dialogs, "originating window, regardless of activation");
    }

    private void ShowTarget()
    {
        try
        {
            if (SelectedWindow is null)
                return;
            Show(SelectedWindow.Container.Resolve<IDialogService>(), "explicitly selected window");
        }
        catch (Exception exception)
        {
            Status = exception.Message;
        }
    }

    private void Show(IDialogService dialogs, string target)
    {
        var callback = new DialogCallback()
            .OnClose(result => Status = $"Dialog in {target} closed: {result.Result}.")
            .OnError((Exception exception) => Status = exception.Message);
        dialogs.ShowDialog("NotificationDialog", new DialogParameters
        {
            { "title", "Window ownership" },
            { "message", $"This dialog belongs to the {target}." }
        }, callback);
    }
}
