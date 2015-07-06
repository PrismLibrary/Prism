

using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Interactivity;

namespace Prism.Wpf.Tests.Interactivity
{
    [TestClass]
    public class CommandBehaviorBaseFixture
    {
        [TestMethod]
        public void ExecuteUsesCommandParameterWhenSet()
        {
            var targetUIElement = new UIElement();
            var target = new TestableCommandBehaviorBase(targetUIElement);
            target.CommandParameter = "123";
            TestCommand testCommand = new TestCommand();
            target.Command = testCommand;

            target.ExecuteCommand("testparam");

            Assert.AreEqual("123", testCommand.ExecuteCalledWithParameter);
        }

        [TestMethod]
        public void ExecuteUsesParameterWhenCommandParameterNotSet()
        {
            var targetUIElement = new UIElement();
            var target = new TestableCommandBehaviorBase(targetUIElement);
            TestCommand testCommand = new TestCommand();
            target.Command = testCommand;

            target.ExecuteCommand("testparam");

            Assert.AreEqual("testparam", testCommand.ExecuteCalledWithParameter);
        }
    }

    class TestableCommandBehaviorBase : CommandBehaviorBase<UIElement>
    {
        public TestableCommandBehaviorBase(UIElement targetObject)
            : base(targetObject)
        {}

        public new void ExecuteCommand(object parameter)
        {
            base.ExecuteCommand(parameter);
        }
    }

    class TestCommand : ICommand
    {
        public object CanExecuteCalledWithParameter { get; set; }
        public bool CanExecute(object parameter)
        {
            CanExecuteCalledWithParameter = parameter;
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public object ExecuteCalledWithParameter { get; set; }
        public void Execute(object parameter)
        {
            ExecuteCalledWithParameter = parameter;
        }
    }

}
