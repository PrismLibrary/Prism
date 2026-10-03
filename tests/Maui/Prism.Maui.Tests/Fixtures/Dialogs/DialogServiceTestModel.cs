using Prism.Dialogs;

#nullable enable
namespace Prism.Maui.Tests.Fixtures.Dialogs;

internal sealed class DialogServiceTestModel : IDialogAware, IActiveAware
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
