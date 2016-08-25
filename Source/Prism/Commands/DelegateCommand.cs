using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Properties;

namespace Prism.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates do not take any parameters for <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// </summary>
    /// <see cref="DelegateCommandBase"/>
    /// <see cref="DelegateCommand{T}"/>
    public class DelegateCommand : DelegateCommandBase, ICommand
    {
        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Action"/> to invoke on execution.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
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
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base((o) => executeMethod(), (o) => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);
        }

        void ICommand.Execute(object parameter)
        {
            _executeMethod(parameter);
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommand ObservesProperty<T>(Expression<Func<T>> propertyExpression)
        {
            ObservesPropertyInternal(propertyExpression);
            return this;
        }

        /// <summary>
        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute((o) => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommand ObservesCanExecute(Expression<Func<object, bool>> canExecuteExpression)
        {
            ObservesCanExecuteInternal(canExecuteExpression);
            return this;
        }

        /// <summary>
        /// Factory method to create a new instance of <see cref="AsyncDelegateCommand"/> from an awaitable handler method.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.</param>
        /// <returns>Constructed instance of <see cref="DelegateCommand"/></returns>
        [Obsolete("Please use AsyncDelegateCommand")]
        public static AsyncDelegateCommand FromAsyncHandler(Func<Task> executeMethod)
        {
            return new AsyncDelegateCommand(executeMethod);
        }

        /// <summary>
        /// Factory method to create a new instance of <see cref="AsyncDelegateCommand"/> from an awaitable handler method.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <returns>Constructed instance of <see cref="DelegateCommand"/></returns>
        [Obsolete("Please use AsyncDelegateCommand")]
        public static AsyncDelegateCommand FromAsyncHandler(Func<Task> executeMethod, Func<bool> canExecuteMethod)
        {
            return new AsyncDelegateCommand(executeMethod, canExecuteMethod);
        }

        ///<summary>
        /// Executes the command.
        ///</summary>
        public virtual Task Execute()
        {
            return _executeMethod(null);
        }

        /// <summary>
        /// Determines if the command can be executed.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the command can execute, otherwise returns <see langword="false"/>.</returns>
        public virtual bool CanExecute()
        {
            return CanExecute(null);
        }

        protected DelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base((o) => executeMethod(), (o) => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);
        }
    }
}
