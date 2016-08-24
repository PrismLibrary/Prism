using System;
using System.Threading.Tasks;

namespace Prism.Commands
{
    public class AsyncDelegateCommand : DelegateCommandBase, IAsyncCommand
    {
        Func<Task> _executeMethod;

        public AsyncDelegateCommand(Func<Task> executeMethod)
            : this(executeMethod, () => true)
        {
            
        }

        public AsyncDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(null, (o) => canExecuteMethod())
        {
            _executeMethod = executeMethod;
        }

        public Task ExecuteAsync(object parameter = null)
        {
            return _executeMethod();
        }

        public async override void Execute(object parameter = null)
        {
            await ExecuteAsync(parameter);
        }
    }
}
