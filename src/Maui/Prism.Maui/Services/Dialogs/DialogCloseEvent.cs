using System.ComponentModel;

namespace Prism.Services;

public struct DialogCloseEvent
{
    private readonly MulticastDelegate _callback;

    public DialogCloseEvent()
    {
        _callback = null;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DialogCloseEvent(Action<IDialogParameters> callback)
    {
        _callback = callback;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DialogCloseEvent(Func<IDialogParameters, Task> callback)
    {
        _callback = callback;
    }

    public void Invoke() =>
        Invoke(null);

    public void Invoke(IDialogParameters parameters)
    {
        parameters ??= new DialogParameters();

        switch(_callback)
        {
            case Action<IDialogParameters> actionCallback:
                actionCallback(parameters);
                break;
            case Func<IDialogParameters, Task> taskCallback:
                taskCallback(parameters);
                break;
        }
    }
}