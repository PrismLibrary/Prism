

using System;
using System.Windows.Input;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockCommand : ICommand
    {
        public bool ExecuteCalled { get; set; }
        public bool CanExecuteReturnValue = true;
        public object ExecuteParameter;
        public object CanExecuteParameter;
        public int CanExecuteTimesCalled;

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ExecuteCalled = true;
            ExecuteParameter = parameter;
        }

        public bool CanExecute(object parameter)
        {
            CanExecuteTimesCalled++;
            CanExecuteParameter = parameter;
            return CanExecuteReturnValue;
        }

        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}