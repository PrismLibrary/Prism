using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Properties;

#nullable enable
namespace Prism.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates do not take any parameters for <see cref="Execute()"/> and <see cref="CanExecute()"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="DelegateCommand"/> is a command implementation that allows you to define the execute and canExecute logic
    /// using delegates. Unlike UI-based commands, DelegateCommand does not accept parameters.
    /// </para>
    /// <para>
    /// DelegateCommand supports fluent syntax through methods like <see cref="ObservesProperty{T}(Expression{Func{T}})"/>,
    /// <see cref="ObservesCanExecute(Expression{Func{bool}})"/>, and <see cref="Catch(Action{Exception})"/> to make creating
    /// and configuring commands easier.
    /// </para>
    /// </remarks>
    /// <seealso cref="DelegateCommandBase"/>
    /// <seealso cref="DelegateCommand{T}"/>
    public class DelegateCommand : DelegateCommandBase
    {
        Action _executeMethod;
        Func<bool> _canExecuteMethod;

        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Action"/> to invoke on execution.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute(object)"/> is called.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="executeMethod"/> is <see langword="null"/>.</exception>
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Action"/> to invoke on execution
        /// and a <see langword="Func" /> to query for determining if the command can execute.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{TResult}"/> to invoke when <see cref="ICommand.CanExecute"/> is called</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="executeMethod"/> or <paramref name="canExecuteMethod"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Both delegates must be non-null. If either is null, an <see cref="ArgumentNullException"/> will be thrown.
        /// </remarks>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base()
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        ///<summary>
        /// Executes the command.
        ///</summary>
        /// <remarks>
        /// Calls the delegate registered during construction. If an exception occurs and has been registered
        /// with <see cref="Catch(Action{Exception})"/>, it will be handled; otherwise, the exception is rethrown.
        /// </remarks>
        public void Execute()
        {
            try
            {
                _executeMethod();
            }
            catch (Exception ex)
            {
                if (!ExceptionHandler.CanHandle(ex))
                    throw;

                ExceptionHandler.Handle(ex, null);
            }
        }

        /// <summary>
        /// Determines if the command can be executed.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the command can execute,otherwise returns <see langword="false"/>.</returns>
        /// <remarks>
        /// Calls the canExecute delegate registered during construction. If an exception occurs, it will return <see langword="false"/>.
        /// </remarks>
        public bool CanExecute()
        {
            try
            {
                return _canExecuteMethod();
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
        protected override void Execute(object? parameter)
        {
            Execute();
        }

        /// <summary>
        /// Handle the internal invocation of <see cref="ICommand.CanExecute(object)"/>
        /// </summary>
        /// <param name="parameter">Command Parameter (ignored by this implementation)</param>
        /// <returns><see langword="true"/> if the Command Can Execute, otherwise <see langword="false" /></returns>
        protected override bool CanExecute(object? parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        /// <remarks>
        /// This method enables automatic notification of command execution state changes when observed properties change.
        /// </remarks>
        public DelegateCommand ObservesProperty<T>(Expression<Func<T>> propertyExpression)
        {
            ObservesPropertyInternal(propertyExpression);
            return this;
        }

        /// <summary>
        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        /// <remarks>
        /// This method replaces the canExecute delegate provided during construction with the compiled expression,
        /// and automatically raises CanExecuteChanged when the observed property changes.
        /// </remarks>
        public DelegateCommand ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
        {
            _canExecuteMethod = canExecuteExpression.Compile();
            ObservesPropertyInternal(canExecuteExpression);
            return this;
        }

        /// <summary>
        /// Registers an callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        /// <remarks>
        /// The exception handler will be invoked only for exceptions of type <see cref="Exception"/>.
        /// </remarks>
        public DelegateCommand Catch(Action<Exception> @catch)
        {
            ExceptionHandler.Register<Exception>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        /// <remarks>
        /// The exception handler will be invoked with both the exception and the command parameter.
        /// </remarks>
        public DelegateCommand Catch(Action<Exception, object> @catch)
        {
            ExceptionHandler.Register<Exception>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <typeparam name="TException">The Exception Type</typeparam>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand Catch<TException>(Action<TException> @catch)
            where TException : Exception
        {
            ExceptionHandler.Register<TException>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <typeparam name="TException">The Exception Type</typeparam>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand Catch<TException>(Action<TException, object> @catch)
            where TException : Exception
        {
            ExceptionHandler.Register<TException>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an async callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand Catch(Func<Exception, Task> @catch)
        {
            ExceptionHandler.Register<Exception>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an async callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand Catch(Func<Exception, object, Task> @catch)
        {
            ExceptionHandler.Register<Exception>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an async callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <typeparam name="TException">The Exception Type</typeparam>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand Catch<TException>(Func<TException, Task> @catch)
            where TException : Exception
        {
            ExceptionHandler.Register<TException>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an async callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <typeparam name="TException">The Exception Type</typeparam>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand Catch<TException>(Func<TException, object, Task> @catch)
            where TException : Exception
        {
            ExceptionHandler.Register<TException>(@catch);
            return this;
        }
    }
}
