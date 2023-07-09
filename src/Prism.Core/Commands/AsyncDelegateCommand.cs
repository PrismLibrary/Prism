using System.Linq.Expressions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Properties;

#nullable enable
namespace Prism.Commands;

/// <summary>
/// Provides an implementation of the <see cref="IAsyncCommand"/>
/// </summary>
public class AsyncDelegateCommand : DelegateCommandBase, IAsyncCommand
{
    private bool _enableParallelExecution = false;
    private bool _isExecuting = false;
    private readonly Func<CancellationToken, Task> _executeMethod;
    private Func<bool> _canExecuteMethod;

    /// <summary>
    /// Creates a new instance of <see cref="AsyncDelegateCommand"/> with the <see cref="Func{Task}"/> to invoke on execution.
    /// </summary>
    /// <param name="executeMethod">The <see cref="Func{Task}"/> to invoke when <see cref="ICommand.Execute(object)"/> is called.</param>
    public AsyncDelegateCommand(Func<Task> executeMethod)
        : this(c => executeMethod(), () => true)
    {

    }

    /// <summary>
    /// Creates a new instance of <see cref="AsyncDelegateCommand"/> with the <see cref="Func{Task}"/> to invoke on execution.
    /// </summary>
    /// <param name="executeMethod">The <see cref="Func{CancellationToken, Task}"/> to invoke when <see cref="ICommand.Execute(object)"/> is called.</param>
    public AsyncDelegateCommand(Func<CancellationToken, Task> executeMethod)
        : this(executeMethod, () => true)
    {

    }

    /// <summary>
    /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Func{Task}"/> to invoke on execution
    /// and a <see langword="Func" /> to query for determining if the command can execute.
    /// </summary>
    /// <param name="executeMethod">The <see cref="Func{Task}"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
    /// <param name="canExecuteMethod">The delegate to invoke when <see cref="ICommand.CanExecute"/> is called</param>
    public AsyncDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
        : this(c => executeMethod(), canExecuteMethod)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Func{Task}"/> to invoke on execution
    /// and a <see langword="Func" /> to query for determining if the command can execute.
    /// </summary>
    /// <param name="executeMethod">The <see cref="Func{CancellationToken, Task}"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
    /// <param name="canExecuteMethod">The delegate to invoke when <see cref="ICommand.CanExecute"/> is called</param>
    public AsyncDelegateCommand(Func<CancellationToken, Task> executeMethod, Func<bool> canExecuteMethod)
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
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        try
        {
            IsExecuting = true;
            await _executeMethod(cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Do nothing... the Task was cancelled
        }
        catch (Exception ex)
        {
            if (!ExceptionHandler.CanHandle(ex))
                throw;

            ExceptionHandler.Handle(ex, null);
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
    public bool CanExecute()
    {
        try
        {
            if (!_enableParallelExecution && IsExecuting)
                return false;

            return _canExecuteMethod?.Invoke() ?? true;
        }
        catch (Exception ex)
        {
            if (!ExceptionHandler.CanHandle(ex))
                throw;

            ExceptionHandler.Handle(ex, null);

            return false;
        }
    }

    /// <summary>
    /// Handle the internal invocation of <see cref="ICommand.Execute(object)"/>
    /// </summary>
    /// <param name="parameter">Command Parameter</param>
    protected override async void Execute(object parameter)
    {
        await Execute();
    }

    /// <summary>
    /// Handle the internal invocation of <see cref="ICommand.CanExecute(object)"/>
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns><see langword="true"/> if the Command Can Execute, otherwise <see langword="false" /></returns>
    protected override bool CanExecute(object parameter)
    {
        return CanExecute();
    }

    /// <summary>
    /// Enables Parallel Execution of Async Tasks
    /// </summary>
    /// <returns></returns>
    public AsyncDelegateCommand EnableParallelExecution()
    {
        _enableParallelExecution = true;
        return this;
    }

    /// <summary>
    /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
    /// </summary>
    /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
    /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
    /// <returns>The current instance of DelegateCommand</returns>
    public AsyncDelegateCommand ObservesProperty<T>(Expression<Func<T>> propertyExpression)
    {
        ObservesPropertyInternal(propertyExpression);
        return this;
    }

    /// <summary>
    /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
    /// </summary>
    /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
    /// <returns>The current instance of DelegateCommand</returns>
    public AsyncDelegateCommand ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
    {
        _canExecuteMethod = canExecuteExpression.Compile();
        ObservesPropertyInternal(canExecuteExpression);
        return this;
    }

    /// <summary>
    /// Provides the ability to connect a delegate to catch exceptions encountered by CanExecute or the Execute methods of the DelegateCommand
    /// </summary>
    /// <param name="catch">TThe callback when a specific exception is encountered</param>
    /// <returns>The current instance of DelegateCommand</returns>
    public AsyncDelegateCommand Catch<TException>(Action<TException> @catch)
        where TException : Exception
    {
        ExceptionHandler.Register<TException>(@catch);
        return this;
    }

    /// <summary>
    /// Provides the ability to connect a delegate to catch exceptions encountered by CanExecute or the Execute methods of the DelegateCommand
    /// </summary>
    /// <param name="catch">The generic / default callback when an exception is encountered</param>
    /// <returns>The current instance of DelegateCommand</returns>
    public AsyncDelegateCommand Catch(Action<Exception> @catch)
    {
        ExceptionHandler.Register<Exception>(@catch);
        return this;
    }

    Task IAsyncCommand.ExecuteAsync(object? parameter)
    {
        return Execute(default);
    }

    Task IAsyncCommand.ExecuteAsync(object? parameter, CancellationToken cancellationToken)
    {
        return Execute(cancellationToken);
    }
}
