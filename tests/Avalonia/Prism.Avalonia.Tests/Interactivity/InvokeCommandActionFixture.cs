using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Prism.Avalonia.Tests.Mocks;
using Xunit;

namespace Prism.Avalonia.Tests.Interactivity
{
    // Override Prism.Interactivity.InvokeCommandAction until it can be implemented.
    //// Reference:
    //// https://github.com/wieslawsoltes/AvaloniaBehaviors/blob/master/src/Avalonia.Xaml.Interactions/Core/InvokeCommandAction.cs
    public class InvokeCommandAction : AvaloniaObject
    {
        /// <summary>
        /// Dependency property identifying if the associated element should automatically be enabled or disabled based on the result of the Command's CanExecute
        /// </summary>
        public static readonly StyledProperty<bool> AutoEnableProperty =
            AvaloniaProperty.Register<InvokeCommandAction, bool>(nameof(AutoEnable));

        /// <summary>Identifies the <seealso cref="Command"/> avalonia property.</summary>
        public static readonly StyledProperty<ICommand?> CommandProperty =
            AvaloniaProperty.Register<InvokeCommandAction, ICommand?>(nameof(Command));

        /// <summary>Identifies the <seealso cref="CommandParameter"/> avalonia property.</summary>
        public static readonly StyledProperty<object?> CommandParameterProperty =
            AvaloniaProperty.Register<InvokeCommandAction, object?>(nameof(CommandParameter));

        /// <summary>
        /// Dependency property identifying the TriggerParameterPath to be parsed to identify the child property of the trigger parameter to be used as the command parameter.
        /// </summary>
        public static readonly StyledProperty<string> TriggerParameterPathProperty =
            AvaloniaProperty.Register<InvokeCommandAction, string>(nameof(TriggerParameterPath));

        /// <summary>
        /// Gets or sets whether or not the associated element will automatically be enabled or disabled based on the result of the commands CanExecute
        /// </summary>
        public bool AutoEnable
        {
            get { return (bool)GetValue(AutoEnableProperty); }
            set { SetValue(AutoEnableProperty, value); }
        }

        public ICommand? Command { get; set; }

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public string TriggerParameterPath
        {
            get => GetValue(TriggerParameterPathProperty) as string;
            set => SetValue(TriggerParameterPathProperty, value);
        }

        public void Attach(Control? ctrl)
        { }

        public void Detach()
        { }

        public void InvokeAction(object? action)
        { }
    }

    public class InvokeCommandActionFixture
    {
        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
        public void WhenCommandIsSetAndThenBehaviorIsAttached_ThenCommandsCanExecuteIsCalledOnce()
        {
            var someControl = new TextBox();
            var command = new MockCommand();
            var commandAction = new InvokeCommandAction();
            commandAction.Command = command;
            commandAction.Attach(someControl);

            Assert.Equal(1, command.CanExecuteTimesCalled);
        }

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

        [StaFact(Skip = "InvokeCommandAction is not implmented.")]
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

    internal class TestEventArgs : EventArgs
    {
        public TestEventArgs(string name)
        {
            Thing1 = new Thing1 { Thing2 = new Thing2 { Name = name } };
        }

        public Thing1 Thing1 { get; set; }
    }

    internal class Thing1
    {
        public Thing2 Thing2 { get; set; }
    }

    internal class Thing2
    {
        public string Name { get; set; }
    }
}
