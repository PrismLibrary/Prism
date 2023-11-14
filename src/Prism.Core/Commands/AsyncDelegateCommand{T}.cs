using System.Linq.Expressions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Properties;

#nullable enable
namespace Prism.Commands;

/// <summary>
/// Provides an implementation of the <see cref="IAsyncCommand"/> with a generic parameter type.
/// </summary>
/// <typeparam name="T"></typeparam>
public class AsyncDelegateCommand<T> : DelegateCommandBase, IAsyncCommand
{
    private bool _enableParallelExecution = false;
    private bool _isExecuting = false;
    private readonly Func<T, CancellationToken, Task> _executeMethod;
    private Func<T, bool> _canExecuteMethod;
    private Func<CancellationToken> _getCancellationToken = () => CancellationToken.None;

    /// <summary>
    /// Creates a new instance of <see cref="AsyncDelegateCommand{T}"/> with the <see cref="Func{Task}"/> to invoke on execution.
    /// </summary>
    /// <param name="executeMethod">The <see cref="Func{T, Task}"/> to invoke when <see cref="ICommand.Execute(object)"/> is called.</param>
    public AsyncDelegateCommand(Func<T, Task> executeMethod)
#if NET6_0_OR_GREATER
        : this((p,t) => executeMethod(p).WaitAsync(t), _ => true)
#else
        : this((p, t) => executeMethod(p), _ => true)
#endif
    {

    }

    /// <summary>
    /// Creates a new instance of <see cref="AsyncDelegateCommand{T}"/> with the <see cref="Func{Task}"/> to invoke on execution.
    /// </summary>
    /// <param name="executeMethod">The <see cref="Func{T, CancellationToken, Task}"/> to invoke when <see cref="ICommand.Execute(object)"/> is called.</param>
    public AsyncDelegateCommand(Func<T, CancellationToken, Task> executeMethod)
        : this(executeMethod, _ => true)
    {

    }

    /// <summary>
    /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Func{Task}"/> to invoke on execution
    /// and a <see langword="Func" /> to query for determining if the command can execute.
    /// </summary>
    /// <param name="executeMethod">The <see cref="Func{T, Task}"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
    /// <param name="canExecuteMethod">The delegate to invoke when <see cref="ICommand.CanExecute"/> is called</param>
    public AsyncDelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
#if NET6_0_OR_GREATER
        : this((p, c) => executeMethod(p).WaitAsync(c), canExecuteMethod)
#else
        : this((p, c) => executeMethod(p), canExecuteMethod)
#endif
    {

    }

    /// <summary>
    /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Func{Task}"/> to invoke on execution
    /// and a <see langword="Func" /> to query for determining if the command can execute.
    /// </summary>
    /// <param name="executeMethod">The <see cref="Func{T, CancellationToken, Task}"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
    /// <param name="canExecuteMethod">The delegate to invoke when <see cref="ICommand.CanExecute"/> is called</param>
    public AsyncDelegateCommand(Func<T, CancellationToken, Task> executeMethod, Func<T, bool> canExecuteMethod)
        : base()
    {
        if (executeMethod == null || canExecuteMethod == null)
            throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

        _executeMethod = executeMethod;
        _canExecuteMethod = canExecuteMethod;
    }

    /// <summary>
    /// Gets the current state of the AsyncDelegateCommand
    /// </summary>
    public bool IsExecuting
    {
        get => _isExecuting;
        private set => SetProperty(ref _isExecuting, value, OnCanExecuteChanged);
    }

    ///<summary>
    /// Executes the command.
    ///</summary>
    public async Task Execute(T parameter, CancellationToken? cancellationToken = null)
    {
        var token = cancellationToken ?? _getCancellationToken();

        try
        {
            if (!_enableParallelExecution && IsExecuting)
                return;

            IsExecuting = true;
            await _executeMethod(parameter, token)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (!ExceptionHandler.CanHandle(ex))
                throw;

            ExceptionHandler.Handle(ex, parameter);
        }
        finally
        {
            IsExecuting = false;
        }
    }

    /// <summary>
    /// Determines if the command can be executed.
    /// </summary>
    /// <returns>Returns <see langword="true"/> if the command can execute,otherwise returns <see langword="false"/>.</returns>
    public bool CanExecute(T parameter)
    {
        try
        {
            if (!_enableParallelExecution && IsExecuting)
                return false;

            return _canExecuteMethod?.Invoke(parameter) ?? true;
        }
        catch (Exception ex)
        {
            if (!ExceptionHandler.CanHandle(ex))
                throw;

            ExceptionHandler.Handle(ex, parameter);

            return false;
        }
    }

    /// <summary>
    /// Handle the internal invocation of <see cref="ICommand.Execute(object)"/>
    /// </summary>
    /// <param name="parameter">Command Parameter</param>
    protected override async void Execute(object? parameter)
    {
        var cancellationToken = _getCancellationToken();
        T parameterAsT;
        try
        {
            parameterAsT = (T)parameter!;
        }
        catch (Exception ex)
        {
            if (!ExceptionHandler.CanHandle(ex))
                throw;

            ExceptionHandler.Handle(ex, parameter);
            return;
        }

        // If we had an exception casting the parameter to T ,
        // we would have already returned. We want to surface any
        // exceptions thrown by the Execute method.
        await Execute(parameterAsT, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Handle the internal invocation of <see cref="ICommand.CanExecute(object)"/>
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns><see langword="true"/> if the Command Can Execute, otherwise <see langword="false" /></returns>
    protected override bool CanExecute(object? parameter)
    {
        try
        {
            return CanExecute((T)parameter!);
        }
        catch (Exception ex)
        {
            if (!ExceptionHandler.CanHandle(ex))
                throw;

            ExceptionHandler.Handle(ex, parameter);

            return false;
        }
    }

    /// <summary>
    /// Enables Parallel Execution of Async Tasks
    /// </summary>
    /// <returns>The current instance of <see cref="AsyncDelegateCommand{T}"/>.</returns>
    public AsyncDelegateCommand<T> EnableParallelExecution()
    {
        _enableParallelExecution = true;
        return this;
    }

    /// <summary>
    /// Sets the <see cref="CancellationTokenSourceFactory(Func{CancellationToken})"/> based on the specified timeout.
    /// </summary>
    /// <param name="timeout">A specified timeout.</param>
    /// <returns>The current instance of <see cref="AsyncDelegateCommand{T}"/>.</returns>
    public AsyncDelegateCommand<T> CancelAfter(TimeSpan timeout) =>
        CancellationTokenSourceFactory(() => new CancellationTokenSource(timeout).Token);

    /// <summary>
    /// Provides a delegate callback to provide a default CancellationToken when the Command is invoked.
    /// </summary>
    /// <param name="factory">The default <see cref="CancellationToken"/> Factory.</param>
    /// <returns>The current instance of <see cref="AsyncDelegateCommand{T}"/>.</returns>
    public AsyncDelegateCommand<T> CancellationTokenSourceFactory(Func<CancellationToken> factory)
    {
        _getCancellationToken = factory;
        return this;
    }

    /// <summary>
    /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
    /// </summary>
    /// <typeparam name="TType">The type of the return value of the method that this delegate encapsulates</typeparam>
    /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
    /// <returns>The current instance of <see cref="AsyncDelegateCommand{T}"/>.</returns>
    public AsyncDelegateCommand<T> ObservesProperty<TType>(Expression<Func<TType>> propertyExpression)
    {
        ObservesPropertyInternal(propertyExpression);
        return this;
    }

    /// <summary>
    /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
    /// </summary>
    /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
    /// <returns>The current instance of <see cref="AsyncDelegateCommand{T}"/>.</returns>
    public AsyncDelegateCommand<T> ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
    {
        Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(canExecuteExpression.Body, Expression.Parameter(typeof(T), "o"));
        _canExecuteMethod = expression.Compile();
        ObservesPropertyInternal(canExecuteExpression);
        return this;
    }

    /// <summary>
    /// Provides the ability to connect a delegate to catch exceptions encountered by CanExecute or the Execute methods of the DelegateCommand
    /// </summary>
    /// <param name="catch">TThe callback when a specific exception is encountered</param>
    /// <returns>The current instance of <see cref="AsyncDelegateCommand{T}"/>.</returns>
    public AsyncDelegateCommand<T> Catch<TException>(Action<TException> @catch)
        where TException : Exception
    {
        ExceptionHandler.Register<TException>(@catch);
        return this;
    }

    /// <summary>
    /// Provides the ability to connect a delegate to catch exceptions encountered by CanExecute or the Execute methods of the DelegateCommand
    /// </summary>
    /// <param name="catch">The generic / default callback when an exception is encountered</param>
    /// <returns>The current instance of <see cref="AsyncDelegateCommand{T}"/>.</returns>
    public AsyncDelegateCommand<T> Catch(Action<Exception> @catch)
    {
        ExceptionHandler.Register<Exception>(@catch);
        return this;
    }

    async Task IAsyncCommand.ExecuteAsync(object? parameter)
    {
        try
        {
            // If T is not nullable this may throw an exception
            await Execute((T)parameter!, _getCancellationToken());
        }
        catch (Exception ex)
        {
            if (!ExceptionHandler.CanHandle(ex))
                throw;

            ExceptionHandler.Handle(ex, parameter);
        }
    }

    async Task IAsyncCommand.ExecuteAsync(object? parameter, CancellationToken cancellationToken)
    {
        try
        {
            // If T is not nullable this may throw an exception
            await Execute((T)parameter!, cancellationToken);
        }
        catch (Exception ex)
        {
            if (!ExceptionHandler.CanHandle(ex))
                throw;

            ExceptionHandler.Handle(ex, parameter);
        }
    }
}
