using System;
using System.ComponentModel;
using System.Threading.Tasks;

#nullable enable
namespace Prism.Dialogs;

/// <summary>
/// This is set by the <see cref="IDialogService"/> on your <see cref="IDialogAware"/> ViewModel. This can then
/// be invoked by either the DialogService or your code to initiate closing the Dialog.
/// </summary>
public struct DialogCloseCallback
{
    private readonly MulticastDelegate _callback;

    /// <summary>
    /// Creates a default instance of the <see cref="DialogCloseCallback"/>
    /// </summary>
    public DialogCloseCallback()
    {
        _callback = () => { };
    }

    /// <summary>
    /// Creates an instance of the <see cref="DialogCloseCallback"/> with an <see cref="Action{IDialogResult}"/> callback.
    /// </summary>
    /// <param name="callback">The callback to invoke.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DialogCloseCallback(Action<IDialogResult> callback)
    {
        _callback = callback;
    }

    /// <summary>
    /// Creates an instance of the <see cref="DialogCloseCallback"/> with an <see cref="Func{IDialogResult, Task}"/> asynchronous callback.
    /// </summary>
    /// <param name="callback"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DialogCloseCallback(Func<IDialogResult, Task> callback)
    {
        _callback = callback;
    }

    /// <summary>
    /// Invokes the initialized delegate with no <see cref="IDialogResult"/>.
    /// </summary>
    public void Invoke() =>
        Invoke(new DialogResult());

    /// <summary>
    /// Invokes the initialized delegate with the specified <see cref="IDialogParameters"/>.
    /// </summary>
    /// <param name="parameters">The <see cref="IDialogParameters"/>.</param>
    /// <param name="result">The <see cref="ButtonResult"/>.</param>
    public void Invoke(IDialogParameters parameters, ButtonResult result = ButtonResult.None) =>
        Invoke(new DialogResult
        {
            Parameters = parameters,
            Result = result
        });

    /// <summary>
    /// Invokes the initialized delegate with the specified <see cref="IDialogResult"/>
    /// </summary>
    /// <param name="result"></param>
    public async void Invoke(IDialogResult result)
    {
        switch(_callback)
        {
            case Action<IDialogResult> actionCallback:
                actionCallback(result);
                break;
            case Func<IDialogResult, Task> taskCallback:
                await taskCallback(result);
                break;
            default:
                throw new InvalidOperationException("The DialogCloseCallback has not been properly initialized. This must be initialized by the DialogService, and should not be set by user code.");
        }
    }
}
