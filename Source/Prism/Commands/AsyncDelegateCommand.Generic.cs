using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Commands
{
    public class AsyncDelegateCommand<T>
    {
        Func<Task<T>> _executeMethod;

        public AsyncDelegateCommand(Func<Task<T>> executeMethod)
        {
            _executeMethod = executeMethod;
        }
    }
}
