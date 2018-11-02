

using System;
using System.Windows.Controls;
using Xunit;
using Prism.Interactivity;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Interactivity
{
    
    public class InvokeCommandActionFixture
    {
        [StaFact]
        public void WhenCommandPropertyIsSet_ThenHooksUpCommandBehavior()
        {
            var someControl = new TextBox();
            var commandAction = new InvokeCommandAction();
            var command = new MockCommand();
            commandAction.Attach(someControl);
            commandAction.Command = command;

            Assert.False(command.ExecuteCalled);

            commandAction.InvokeAction(null);

            Assert.True(command.ExecuteCalled);
            Assert.Same(command, commandAction.GetValue(InvokeCommandAction.CommandProperty));
        }

        [StaFact]
        public void WhenAttachedAfterCommandPropertyIsSetAndInvoked_ThenInvokesCommand()
        {
            var someControl = new TextBox();
            var commandAction = new InvokeCommandAction();
            var command = new MockCommand();
            commandAction.Command = command;
            commandAction.Attach(someControl);

            Assert.False(command.ExecuteCalled);

            commandAction.InvokeAction(null);

            Assert.True(command.ExecuteCalled);
            Assert.Same(command, commandAction.GetValue(InvokeCommandAction.CommandProperty));
        }

        [StaFact]
        public void WhenChangingProperty_ThenUpdatesCommand()
        {
            var someControl = new TextBox();
            var oldCommand = new MockCommand();
            var newCommand = new MockCommand();
            var commandAction = new InvokeCommandAction();
            commandAction.Attach(someControl);
            commandAction.Command = oldCommand;
            commandAction.Command = newCommand;
            commandAction.InvokeAction(null);

            Assert.True(newCommand.ExecuteCalled);
            Assert.False(oldCommand.ExecuteCalled);
        }

        [StaFact]
        public void WhenInvokedWithCommandParameter_ThenPassesCommandParaeterToExecute()
        {
            var someControl = new TextBox();
            var command = new MockCommand();
            var parameter = new object();
            var commandAction = new InvokeCommandAction();
            commandAction.Attach(someControl);
            commandAction.Command = command;
            commandAction.CommandParameter = parameter;

            Assert.Null(command.ExecuteParameter);

            commandAction.InvokeAction(null);

            Assert.True(command.ExecuteCalled);
            Assert.NotNull(command.ExecuteParameter);
            Assert.Same(parameter, command.ExecuteParameter);
        }

        [StaFact]
        public void WhenCommandParameterChanged_ThenUpdatesIsEnabledState()
        {
            var someControl = new TextBox();
            var command = new MockCommand();
            var parameter = new object();
            var commandAction = new InvokeCommandAction();
            commandAction.Attach(someControl);
            commandAction.Command = command;

            Assert.Null(command.CanExecuteParameter);
            Assert.True(someControl.IsEnabled);

            command.CanExecuteReturnValue = false;
            commandAction.CommandParameter = parameter;

            Assert.NotNull(command.CanExecuteParameter);
            Assert.Same(parameter, command.CanExecuteParameter);
            Assert.False(someControl.IsEnabled);
        }

        [StaFact]
        public void WhenCanExecuteChanged_ThenUpdatesIsEnabledState()
        {
            var someControl = new TextBox();
            var command = new MockCommand();
            var parameter = new object();
            var commandAction = new InvokeCommandAction();
            commandAction.Attach(someControl);
            commandAction.Command = command;
            commandAction.CommandParameter = parameter;

            Assert.True(someControl.IsEnabled);

            command.CanExecuteReturnValue = false;
            command.RaiseCanExecuteChanged();

            Assert.NotNull(command.CanExecuteParameter);
            Assert.Same(parameter, command.CanExecuteParameter);
            Assert.False(someControl.IsEnabled);
        }

        [StaFact]
        public void WhenDetatched_ThenSetsCommandAndCommandParameterToNull()
        {
            var someControl = new TextBox();
            var command = new MockCommand();
            var parameter = new object();
            var commandAction = new InvokeCommandAction();
            commandAction.Attach(someControl);
            commandAction.Command = command;
            commandAction.CommandParameter = parameter;

            Assert.NotNull(commandAction.Command);
            Assert.NotNull(commandAction.CommandParameter);

            commandAction.Detach();

            Assert.Null(commandAction.Command);
            Assert.Null(commandAction.CommandParameter);
        }

        [StaFact]
        public void WhenCommandIsSetAndThenBehaviorIsAttached_ThenCommandsCanExecuteIsCalledOnce()
        {
            var someControl = new TextBox();
            var command = new MockCommand();
            var commandAction = new InvokeCommandAction();
            commandAction.Command = command;
            commandAction.Attach(someControl);

            Assert.Equal(1, command.CanExecuteTimesCalled);
        }

        [StaFact]
        public void WhenCommandAndCommandParameterAreSetPriorToBehaviorBeingAttached_ThenCommandIsExecutedCorrectlyOnInvoke()
        {
            var someControl = new TextBox();
            var command = new MockCommand();
            var parameter = new object();
            var commandAction = new InvokeCommandAction();
            commandAction.Command = command;
            commandAction.CommandParameter = parameter;
            commandAction.Attach(someControl);

            commandAction.InvokeAction(null);
           
            Assert.True(command.ExecuteCalled);
        }

        [StaFact]
        public void WhenCommandParameterNotSet_ThenEventArgsPassed()
        {
            var eventArgs = new TestEventArgs(null);
            var someControl = new TextBox();
            var command = new MockCommand();
            var parameter = new object();
            var commandAction = new InvokeCommandAction();
            commandAction.Command = command;
            commandAction.Attach(someControl);

            commandAction.InvokeAction(eventArgs);

            Assert.IsType<TestEventArgs>(command.ExecuteParameter);
        }

        [StaFact]
        public void WhenCommandParameterNotSetAndEventArgsParameterPathSet_ThenPathedValuePassed()
        {
            var eventArgs = new TestEventArgs("testname");
            var someControl = new TextBox();
            var command = new MockCommand();
            var parameter = new object();
            var commandAction = new InvokeCommandAction();
            commandAction.Command = command;
            commandAction.TriggerParameterPath = "Thing1.Thing2.Name";
            commandAction.Attach(someControl);

            commandAction.InvokeAction(eventArgs);

            Assert.Equal("testname", command.ExecuteParameter);
        }

        [StaFact]
        public void WhenAttachedAndCanExecuteReturnsTrue_ThenDisabledUIElementIsEnabled()
        {
            var someControl = new TextBox();
            someControl.IsEnabled = false;

            var command = new MockCommand();
            command.CanExecuteReturnValue = true;
            var commandAction = new InvokeCommandAction();
            commandAction.Command = command;
            commandAction.Attach(someControl);

            Assert.True(someControl.IsEnabled);
        }

        [StaFact]
        public void WhenAttachedAndCanExecuteReturnsFalse_ThenEnabledUIElementIsDisabled()
        {
            var someControl = new TextBox();
            someControl.IsEnabled = true;

            var command = new MockCommand();
            command.CanExecuteReturnValue = false;
            var commandAction = new InvokeCommandAction();
            commandAction.Command = command;
            commandAction.Attach(someControl);

            Assert.False(someControl.IsEnabled);
        }

        [StaFact]
        public void WhenAutoEnableIsFalse_ThenDisabledUIElementRemainsDisabled()
        {
            var someControl = new TextBox();
            someControl.IsEnabled = false;

            var command = new MockCommand();
            command.CanExecuteReturnValue = true;
            var commandAction = new InvokeCommandAction();
            commandAction.AutoEnable = false;
            commandAction.Command = command;
            commandAction.Attach(someControl);

            Assert.False(someControl.IsEnabled);
        }

        [StaFact]
        public void WhenAutoEnableIsFalse_ThenEnabledUIElementRemainsEnabled()
        {
            var someControl = new TextBox();
            someControl.IsEnabled = true;

            var command = new MockCommand();
            command.CanExecuteReturnValue = false;
            var commandAction = new InvokeCommandAction();
            commandAction.AutoEnable = false;
            commandAction.Command = command;
            commandAction.Attach(someControl);

            Assert.True(someControl.IsEnabled);
        }

        [StaFact]
        public void WhenAutoEnableIsUpdated_ThenDisabledUIElementIsEnabled()
        {
            var someControl = new TextBox();
            someControl.IsEnabled = false;

            var command = new MockCommand();
            var commandAction = new InvokeCommandAction();
            commandAction.AutoEnable = false;
            commandAction.Command = command;
            commandAction.Attach(someControl);

            Assert.False(someControl.IsEnabled);

            commandAction.AutoEnable = true;

            Assert.True(someControl.IsEnabled);
        }

        [StaFact]
        public void WhenAutoEnableIsUpdated_ThenEnabledUIElementIsDisabled()
        {
            var someControl = new TextBox();
            someControl.IsEnabled = true;

            var command = new MockCommand();
            command.CanExecuteReturnValue = false;
            var commandAction = new InvokeCommandAction();
            commandAction.AutoEnable = false;
            commandAction.Command = command;
            commandAction.Attach(someControl);

            Assert.True(someControl.IsEnabled);

            commandAction.AutoEnable = true;

            Assert.False(someControl.IsEnabled);
        }
    }

    class TestEventArgs : EventArgs
    {
        public TestEventArgs(string name)
        {
            this.Thing1 = new Thing1 { Thing2 = new Thing2 { Name = name } };
        }

        public Thing1 Thing1 { get; set; }
    }

    class Thing1
    {
        public Thing2 Thing2 { get; set; }
    }

    class Thing2
    {
        public string Name { get; set; }
    }
}