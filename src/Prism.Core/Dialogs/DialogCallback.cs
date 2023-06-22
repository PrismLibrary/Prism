using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Prism.Dialogs;

/// <summary>
/// Provides a container for one or more Callbacks which may target specific Error Handling or Delegates to invoke on the successful close of the Dialog
/// </summary>
public readonly struct DialogCallback
{
    private readonly bool _empty = false;
    private readonly List<MulticastDelegate> _callbacks = new List<MulticastDelegate>();
    private readonly List<MulticastDelegate> _errorCallbacks = new List<MulticastDelegate>();

    /// <summary>
    /// Creates a new instance of a DialogCallback
    /// </summary>
    public DialogCallback()
        : this(false)
    {
    }

    private DialogCallback(bool empty) => _empty = empty;

    /// <summary>
    /// Invokes the Delegates based on a specific Exception that was encountered.
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task Invoke(Exception ex) =>
        Invoke(new DialogResult { Exception = ex });

    /// <summary>
    /// Invokes the Delegates for a given <see cref="IDialogResult"/>
    /// </summary>
    /// <param name="result">The Result</param>
    /// <returns>A <see cref="Task"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task Invoke(IDialogResult result)
    {
        if (_empty || (!_callbacks.Any() && !_errorCallbacks.Any()))
            return;

        if(result.Exception is not null)
        {
            // process error callbacks
            if (_errorCallbacks.Any())
            {
                foreach(MulticastDelegate errorCallback in _errorCallbacks)
                {
                    await Process(errorCallback, result);
                }
                return;
            }
        }


        foreach(var callback in  _callbacks)
        {
            await Process(callback, result);
        }
    }

    private static async Task Process(MulticastDelegate @delegate, IDialogResult result)
    {
        if (@delegate is Action action)
            action();
        else if (@delegate is Action<IDialogResult> actionResult)
            actionResult(result);
        else if (@delegate is Action<Exception> actionException && result.Exception is not null)
            actionException(result.Exception);
        else if (@delegate is Func<Task> func)
            await func();
        else if (@delegate is Func<IDialogResult, Task> funcResult)
            await funcResult(result);
        else if (@delegate is Func<Exception, Task> taskException && result.Exception is not null)
            await taskException(result.Exception);
    }

    /// <summary>
    /// Provides an empty DialogCallback that will not execute any 
    /// </summary>
    public static DialogCallback Empty => new DialogCallback(true);

    /// <summary>
    /// Provides a delegate callback method when the Dialog is closed
    /// </summary>
    /// <param name="action">The callback</param>
    /// <returns></returns>
    public DialogCallback OnClose(Action action)
    {
        _callbacks.Add(action);
        return this;
    }


    /// <summary>
    /// Provides a delegate callback method when the Dialog is closed
    /// </summary>
    /// <param name="action">The callback</param>
    /// <returns></returns>
    public DialogCallback OnClose(Action<IDialogResult> action)
    {
        _callbacks.Add(action);
        return this;
    }

    /// <summary>
    /// Provides a delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="action">The callback</param>
    /// <returns></returns>
    public DialogCallback OnError(Action action)
    {
        _errorCallbacks.Add(action);
        return this;
    }

    /// <summary>
    /// Provides a delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="action">The callback</param>
    /// <returns></returns>
    public DialogCallback OnError(Action<Exception> action)
    {
        _errorCallbacks.Add(action);
        return this;
    }

    /// <summary>
    /// Provides a delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="action">The callback</param>
    /// <returns></returns>
    public DialogCallback OnError(Action<IDialogResult> action)
    {
        _errorCallbacks.Add(action);
        return this;
    }

    /// <summary>
    /// Provides an asynchronous delegate callback method when the Dialog is closed
    /// </summary>
    /// <param name="func">The callback</param>
    /// <returns></returns>
    public DialogCallback OnCloseAsync(Func<Task> func)
    {
        _callbacks.Add(func);
        return this;
    }

    /// <summary>
    /// Provides an asynchronous delegate callback method when the Dialog is closed
    /// </summary>
    /// <param name="func">The callback</param>
    /// <returns></returns>
    public DialogCallback OnCloseAsync(Func<IDialogResult, Task> func)
    {
        _callbacks.Add(func);
        return this;
    }

    /// <summary>
    /// Provides an asynchronous delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="func">The callback</param>
    /// <returns></returns>
    public DialogCallback OnErrorAsync(Func<Task> func)
    {
        _errorCallbacks.Add(func);
        return this;
    }

    /// <summary>
    /// Provides an asynchronous delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="func">The callback</param>
    /// <returns></returns>
    public DialogCallback OnErrorAsync(Func<Exception, Task> func)
    {
        _errorCallbacks.Add(func);
        return this;
    }

    /// <summary>
    /// Provides an asynchronous delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="func">The callback</param>
    /// <returns></returns>
    public DialogCallback OnErrorAsync(Func<IDialogResult, Task> func)
    {
        _errorCallbacks.Add(func);
        return this;
    }
}
