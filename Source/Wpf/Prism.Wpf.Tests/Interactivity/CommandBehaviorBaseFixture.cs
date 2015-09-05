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

        [TestMethod]
        public void CommandBehaviorBaseAllowsDisableByDefault()
        {
            var targetUIElement = new UIElement();
            var target = new TestableCommandBehaviorBase(targetUIElement);

            Assert.IsTrue(target.AutoEnable);
        }

        [TestMethod]
        public void CommandBehaviorBaseEnablesUIElement()
        {
            var targetUIElement = new UIElement();
            targetUIElement.IsEnabled = false;

            var target = new TestableCommandBehaviorBase(targetUIElement);
            TestCommand testCommand = new TestCommand();
            target.Command = testCommand;
            target.ExecuteCommand(null);

            Assert.IsTrue(targetUIElement.IsEnabled);
        }

        [TestMethod]
        public void CommandBehaviorBaseDisablesUIElement()
        {
            var targetUIElement = new UIElement();
            targetUIElement.IsEnabled = true;

            var target = new TestableCommandBehaviorBase(targetUIElement);
            TestCommand testCommand = new TestCommand();
            testCommand.CanExecuteResult = false;
            target.Command = testCommand;
            target.ExecuteCommand(null);

            Assert.IsFalse(targetUIElement.IsEnabled);
        }

        [TestMethod]
        public void WhenAutoEnableIsFalse_ThenDisabledUIElementRemainsDisabled()
        {
            var targetUIElement = new UIElement();
            targetUIElement.IsEnabled = false;

            var target = new TestableCommandBehaviorBase(targetUIElement);
            target.AutoEnable = false;
            TestCommand testCommand = new TestCommand();
            target.Command = testCommand;
            target.ExecuteCommand(null);

            Assert.IsFalse(targetUIElement.IsEnabled);
        }

        [TestMethod]
        public void WhenAutoEnableIsUpdated_ThenDisabledUIElementIsEnabled()
        {
            var targetUIElement = new UIElement();
            targetUIElement.IsEnabled = false;

            var target = new TestableCommandBehaviorBase(targetUIElement);
            target.AutoEnable = false;
            TestCommand testCommand = new TestCommand();
            target.Command = testCommand;
            target.ExecuteCommand(null);

            Assert.IsFalse(targetUIElement.IsEnabled);

            target.AutoEnable = true;

            Assert.IsTrue(targetUIElement.IsEnabled);
        }

        [TestMethod]
        public void WhenAutoEnableIsUpdated_ThenEnabledUIElementIsDisabled()
        {
            var targetUIElement = new UIElement();
            targetUIElement.IsEnabled = true;

            var target = new TestableCommandBehaviorBase(targetUIElement);
            target.AutoEnable = false;
            TestCommand testCommand = new TestCommand();
            testCommand.CanExecuteResult = false;
            target.Command = testCommand;
            target.ExecuteCommand(null);

            Assert.IsTrue(targetUIElement.IsEnabled);

            target.AutoEnable = true;

            Assert.IsFalse(targetUIElement.IsEnabled);
        }
    }

    class TestableCommandBehaviorBase : CommandBehaviorBase<UIElement>
    {
        public TestableCommandBehaviorBase(UIElement targetObject)
            : base(targetObject)
        { }

        public new void ExecuteCommand(object parameter)
        {
            base.ExecuteCommand(parameter);
        }
    }

    class TestCommand : ICommand
    {
        bool _canExecte = true;
        public bool CanExecuteResult
        {
            get { return _canExecte; }
            set { _canExecte = value; }
        }

        public object CanExecuteCalledWithParameter { get; set; }
        public bool CanExecute(object parameter)
        {
            CanExecuteCalledWithParameter = parameter;
            return CanExecuteResult;
        }

        public event EventHandler CanExecuteChanged;

        public object ExecuteCalledWithParameter { get; set; }
        public void Execute(object parameter)
        {
            ExecuteCalledWithParameter = parameter;
        }
    }

}
