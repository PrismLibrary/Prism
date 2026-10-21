using Prism.Windows;

namespace Playground.Module.ViewModels;

internal class WindowLabViewModel : BindableBase
{
    private readonly IWindowManager _windows;
    private readonly IWindowContext _origin;
    private readonly IDialogService _dialogs;
    private IReadOnlyList<IWindowContext> _targets = Array.Empty<IWindowContext>();
    private IWindowContext? _selectedWindow;
    private string _status = string.Empty;

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
        get => _targets;
        private set => SetProperty(ref _targets, value);
    }

    public IWindowContext? SelectedWindow
    {
        get => _selectedWindow;
        set
        {
            if (SetProperty(ref _selectedWindow, value))
            {
                ActivateThenShowOriginCommand.RaiseCanExecuteChanged();
                ShowTargetCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

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
