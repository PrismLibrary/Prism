using System;
using System.Reflection;
using System.Threading.Tasks;
using Prism.Properties;

namespace Prism.Commands
{
    /// <summary>
    /// Delegate async command.
    /// </summary>
	public class DelegateAsyncCommand<T> : DelegateCommandBase, IAsyncCommand<T>
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private readonly Action _completedHandler;
        private readonly Action<Exception> _errorHandler;

        public DelegateAsyncCommand(Func<T, Task> executeMethod)
            : this(executeMethod, (o) => true)
        {
        }
        public DelegateAsyncCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, null, null)
        {
        }
        public DelegateAsyncCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod, Action<Exception> errorHandler)
            : this(executeMethod, canExecuteMethod, null, errorHandler)
        {
        }

        public DelegateAsyncCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod, Action completedHandler)
            : this(executeMethod, canExecuteMethod, completedHandler, null)
        {
        }

        public DelegateAsyncCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod = null,
            Action completedHandler = null, Action < Exception> errorHandler = null)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

            TypeInfo genericTypeInfo = typeof(T).GetTypeInfo();

            // DelegateCommand allows object or Nullable<>.  
            // note: Nullable<> is a struct so we cannot use a class constraint.
            if (genericTypeInfo.IsValueType)
            {
                if ((!genericTypeInfo.IsGenericType) || (!typeof(Nullable<>).GetTypeInfo().IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo())))
                {
                    throw new InvalidCastException(Resources.DelegateCommandInvalidGenericPayloadType);
                }
            }

            _execute = executeMethod;
            _canExecute = canExecuteMethod;
            _completedHandler = completedHandler;
            _errorHandler = errorHandler;
        }

        public bool CanExecute(T parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();

                    await _execute(parameter);
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
            ExecuteAsync((T)parameter).Await(_completedHandler, _errorHandler);
        }

        protected override bool CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }
    }
}
