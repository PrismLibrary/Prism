using Prism.Dialogs;
using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed class SmokeDialogViewModel : IDialogAware
{
    private readonly SmokeState _state;

    public SmokeDialogViewModel(SmokeState state, WindowLifetime lifetime, IContainerProvider container)
    {
        _state = state;
        Lifetime = lifetime;
        Container = container;
    }

    public DialogCloseListener RequestClose { get; private set; }
    public WindowLifetime Lifetime { get; }
    public IContainerProvider Container { get; }
    public string Id { get; private set; } = string.Empty;
    public bool AutoClose { get; private set; }

    public bool CanCloseDialog() => true;

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Id = parameters.TryGetValue<string>("id", out var id) ? id : $"manual-{Guid.NewGuid()}";
        AutoClose = parameters.TryGetValue<bool>("autoClose", out var autoClose) && autoClose;
        var observation = _state.GetDialog(Id);
        observation.ViewModel = this;
        observation.OpenedCount++;
        observation.OpenSequence = _state.NextSequence();
    }

    public void OnDialogClosed()
    {
        var observation = _state.GetDialog(Id);
        observation.ClosedCount++;
        observation.CloseSequence = _state.NextSequence();
    }
}
