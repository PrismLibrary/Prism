using System;
using System.Threading.Tasks;
using Prism.Properties;

namespace Prism.Commands
{
    /// <summary>
    /// Delegate async command.
    /// </summary>
    public class DelegateAsyncCommand : DelegateCommandBase, IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private readonly Action _completedHandler;
        private readonly Action<Exception> _errorHandler;

        public DelegateAsyncCommand(Func<Task> executeMethod)
            : this(executeMethod, () => true)
        {
        }
        public DelegateAsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, null, null)
        {
        }
        public DelegateAsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod, Action<Exception> errorHandler)
            : this(executeMethod, canExecuteMethod, null, errorHandler)
        {
        }

        public DelegateAsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod, Action completedHandler)
            : this(executeMethod, canExecuteMethod, completedHandler, null)
        {
        }

        public DelegateAsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod,
            Action completedHandler, Action<Exception> errorHandler)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);
            }

            _execute = executeMethod;
            _canExecute = canExecuteMethod;
            _completedHandler = completedHandler;
            _errorHandler = errorHandler;
        }

        public bool CanExecute()
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();

                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }            
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void Execute(object parameter)
        {
            ExecuteAsync().Await(_completedHandler, _errorHandler);
        }

        protected override bool CanExecute(object parameter)
        {
            return CanExecute();
        }
    }
}
