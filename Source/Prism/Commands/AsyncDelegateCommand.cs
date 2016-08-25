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
    public class AsyncDelegateCommand : DelegateCommand, ICommand
    {
        /// <summary>
        /// Creates a new instance of <see cref="AsyncDelegateCommand"/> with awaitable handler method to invoke on execution.
        /// </summary>
        /// <param name="executeMethod">The awaitable handler method to invoke when <see cref="ICommand.Execute"/> is called.</param>
        public AsyncDelegateCommand(Func<Task> executeMethod)
            : this(executeMethod, () => true)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AsyncDelegateCommand"/> with awaitable handler method to invoke on execution
        /// and a <see langword="Func" /> to query for determining if the command can execute.
        /// </summary>
        /// <param name="executeMethod">The awaitable handler method to invoke when <see cref="ICommand.Execute"/> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{TResult}"/> to invoke when <see cref="ICommand.CanExecute"/> is called</param>
        public AsyncDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(executeMethod, canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);
        }

        async void ICommand.Execute(object parameter)
        {
            await _executeMethod(parameter);
        }

        public override Task Execute()
        {
            return _executeMethod(null);
        }
    }
}
