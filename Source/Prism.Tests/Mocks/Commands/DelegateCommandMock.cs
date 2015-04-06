using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Tests.Mocks.Commands
{
    public class DelegateCommandMock : DelegateCommandBase
    {
        public DelegateCommandMock(Action<object> executeMethod) :
            base(executeMethod, (o) => true)
        {

        }

        public DelegateCommandMock(Action<object> executeMethod, Func<object, bool> canExecuteMethod) :
            base(executeMethod, canExecuteMethod)
        {

        }

        public static DelegateCommandMock FromAsyncHandler(Func<object, Task> executeMethod)
        {
            return new DelegateCommandMock(executeMethod);
        }

        public static DelegateCommandMock FromAsyncHandler(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod)
        {
            return new DelegateCommandMock(executeMethod, canExecuteMethod);
        }

        private DelegateCommandMock(Func<object, Task> executeMethod) :
            base(executeMethod, (o) => true)
        {

        }

        private DelegateCommandMock(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod) :
            base(executeMethod, canExecuteMethod)
        {

        }
    }
}
