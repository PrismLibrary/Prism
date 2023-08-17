using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Common;

namespace Prism.Dialogs;

/// <summary>
/// Provides a container for one or more Callbacks which may target specific Error Handling or Delegates to invoke on the successful close of the Dialog
/// </summary>
#nullable enable
public readonly struct DialogCallback
{
    private readonly bool _empty = false;
    private readonly List<MulticastDelegate> _callbacks = new ();
    private readonly MulticastExceptionHandler _errorCallbacks = new ();

    /// <summary>
    /// Creates a new instance of a DialogCallback
    /// </summary>
    public DialogCallback()
        : this(false)
    {
    }

    private DialogCallback(bool empty)
    {
        _empty = empty;
    }

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
        if (_empty || (result.Exception is DialogException && result.Exception.Message == DialogException.CanCloseIsFalse))
        {
            return;
        }
        else if (result.Exception is not null && _errorCallbacks.CanHandle(result.Exception))
        {
            await _errorCallbacks.HandleAsync(result.Exception, result);
            return;
        }
        else if(_callbacks.Any())
        {
            foreach(var callback in _callbacks)
            {
                await Process(callback, result);
            }
        }
    }

    private static async Task Process(MulticastDelegate @delegate, IDialogResult result)
    {
        if (@delegate is Action action)
            action();
        else if (@delegate is Action<IDialogResult> actionResult)
            actionResult(result);
        else if (@delegate is Func<Task> func)
            await func();
        else if (@delegate is Func<IDialogResult, Task> funcResult)
            await funcResult(result);
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
        _errorCallbacks.Register<Exception>(action);
        return this;
    }

    /// <summary>
    /// Provides a delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="action">The callback</param>
    /// <returns></returns>
    public DialogCallback OnError<TException>(Action action)
        where TException : Exception
    {
        _errorCallbacks.Register<TException>(action);
        return this;
    }

    /// <summary>
    /// Provides a delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="action">The callback</param>
    /// <returns></returns>
    public DialogCallback OnError(Action<Exception> action)
    {
        _errorCallbacks.Register<Exception>(action);
        return this;
    }

    /// <summary>
    /// Provides a delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="action">The callback</param>
    /// <returns></returns>
    public DialogCallback OnError<TException>(Action<TException> action)
        where TException : Exception
    {
        _errorCallbacks.Register<TException>(action);
        return this;
    }

    /// <summary>
    /// Provides a delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="action">The callback</param>
    /// <returns></returns>
    public DialogCallback OnError<TException>(Action<TException, IDialogResult> action)
        where TException : Exception
    {
        _errorCallbacks.Register<TException>(action);
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
        _errorCallbacks.Register<Exception>(func);
        return this;
    }

    /// <summary>
    /// Provides an asynchronous delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="func">The callback</param>
    /// <returns></returns>
    public DialogCallback OnErrorAsync<TException>(Func<Task> func)
        where TException : Exception
    {
        _errorCallbacks.Register<TException>(func);
        return this;
    }

    /// <summary>
    /// Provides an asynchronous delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="func">The callback</param>
    /// <returns></returns>
    public DialogCallback OnErrorAsync(Func<Exception, Task> func)
    {
        _errorCallbacks.Register<Exception>(func);
        return this;
    }

    /// <summary>
    /// Provides an asynchronous delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="func">The callback</param>
    /// <returns></returns>
    public DialogCallback OnErrorAsync<TException>(Func<TException, Task> func)
        where TException : Exception
    {
        _errorCallbacks.Register<TException>(func);
        return this;
    }

    /// <summary>
    /// Provides an asynchronous delegate callback method when an Exception is encountered
    /// </summary>
    /// <param name="func">The callback</param>
    /// <returns></returns>
    public DialogCallback OnErrorAsync<TException>(Func<TException, IDialogResult, Task> func)
        where TException : Exception
    {
        _errorCallbacks.Register<TException>(func);
        return this;
    }
}
