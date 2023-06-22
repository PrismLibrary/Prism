using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Prism.Dialogs;

/// <summary>
/// Provides a type to manage the invocation of a callback to close the Dialog
/// </summary>
public struct DialogCloseEvent
{
    private readonly MulticastDelegate _callback;

    /// <summary>
    /// Creates a default instance of the <see cref="DialogCloseEvent"/>
    /// </summary>
    public DialogCloseEvent()
    {
        _callback = null;
    }

    /// <summary>
    /// Creates an instance of the <see cref="DialogCloseEvent"/> with an <see cref="Action{IDialogParameters}"/> callback.
    /// </summary>
    /// <param name="callback">The callback to invoke.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DialogCloseEvent(Action<IDialogParameters> callback)
    {
        _callback = callback;
    }

    /// <summary>
    /// Creates an instance of the <see cref="DialogCloseEvent"/> with an <see cref="Func{IDialogParameters, Task}"/> asynchronous callback.
    /// </summary>
    /// <param name="callback"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DialogCloseEvent(Func<IDialogParameters, Task> callback)
    {
        _callback = callback;
    }

    /// <summary>
    /// Invokes the initialized delegate with no <see cref="IDialogParameters"/>.
    /// </summary>
    public void Invoke() =>
        Invoke(null);

    /// <summary>
    /// Invokes the initialized delegate with the specified <see cref="IDialogParameters"/>.
    /// </summary>
    /// <param name="parameters">The <see cref="IDialogParameters"/>.</param>
    public async void Invoke(IDialogParameters parameters)
    {
        parameters ??= new DialogParameters();

        switch(_callback)
        {
            case Action<IDialogParameters> actionCallback:
                actionCallback(parameters);
                break;
            case Func<IDialogParameters, Task> taskCallback:
                await taskCallback(parameters);
                break;
        }
    }
}
