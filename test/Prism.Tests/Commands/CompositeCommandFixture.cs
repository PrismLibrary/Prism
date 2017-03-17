using System;
using System.Windows.Input;
using Xunit;
using Prism.Commands;

namespace Prism.Tests.Commands
{
    public class CompositeCommandFixture
    {
        [Fact]
        public void RegisterACommandShouldRaiseCanExecuteEvent()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommand = new TestCommand();

            multiCommand.RegisterCommand(new TestCommand());
            Assert.True(multiCommand.CanExecuteChangedRaised);
        }

        [Fact]
        public void ShouldDelegateExecuteToSingleRegistrant()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommand = new TestCommand();

            multiCommand.RegisterCommand(testCommand);

            Assert.False(testCommand.ExecuteCalled);
            multiCommand.Execute(null);
            Assert.True(testCommand.ExecuteCalled);
        }

        [Fact]
        public void ShouldDelegateExecuteToMultipleRegistrants()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand();
            TestCommand testCommandTwo = new TestCommand();

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.RegisterCommand(testCommandTwo);

            Assert.False(testCommandOne.ExecuteCalled);
            Assert.False(testCommandTwo.ExecuteCalled);
            multiCommand.Execute(null);
            Assert.True(testCommandOne.ExecuteCalled);
            Assert.True(testCommandTwo.ExecuteCalled);
        }

        [Fact]
        public void ShouldDelegateCanExecuteToSingleRegistrant()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommand = new TestCommand();

            multiCommand.RegisterCommand(testCommand);

            Assert.False(testCommand.CanExecuteCalled);
            multiCommand.CanExecute(null);
            Assert.True(testCommand.CanExecuteCalled);
        }

        [Fact]
        public void ShouldDelegateCanExecuteToMultipleRegistrants()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand();
            TestCommand testCommandTwo = new TestCommand();

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.RegisterCommand(testCommandTwo);

            Assert.False(testCommandOne.CanExecuteCalled);
            Assert.False(testCommandTwo.CanExecuteCalled);
            multiCommand.CanExecute(null);
            Assert.True(testCommandOne.CanExecuteCalled);
            Assert.True(testCommandTwo.CanExecuteCalled);
        }

        [Fact]
        public void CanExecuteShouldReturnTrueIfAllRegistrantsTrue()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };
            TestCommand testCommandTwo = new TestCommand() { CanExecuteValue = true };

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.RegisterCommand(testCommandTwo);

            Assert.True(multiCommand.CanExecute(null));
        }

        [Fact]
        public void CanExecuteShouldReturnFalseIfASingleRegistrantsFalse()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };
            TestCommand testCommandTwo = new TestCommand() { CanExecuteValue = false };

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.RegisterCommand(testCommandTwo);

            Assert.False(multiCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldReraiseCanExecuteChangedEvent()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };

            Assert.False(multiCommand.CanExecuteChangedRaised);
            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.CanExecuteChangedRaised = false;

            testCommandOne.FireCanExecuteChanged();
            Assert.True(multiCommand.CanExecuteChangedRaised);
        }

        [Fact]
        public void ShouldReraiseCanExecuteChangedEventAfterCollect()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };

            Assert.False(multiCommand.CanExecuteChangedRaised);
            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.CanExecuteChangedRaised = false;

            GC.Collect();

            testCommandOne.FireCanExecuteChanged();
            Assert.True(multiCommand.CanExecuteChangedRaised);
        }

        [Fact]
        public void ShouldReraiseDelegateCommandCanExecuteChangedEventAfterCollect()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            DelegateCommand<object> delegateCommand = new DelegateCommand<object>(delegate { });

            Assert.False(multiCommand.CanExecuteChangedRaised);
            multiCommand.RegisterCommand(delegateCommand);
            multiCommand.CanExecuteChangedRaised = false;

            GC.Collect();

            delegateCommand.RaiseCanExecuteChanged();

            Assert.True(multiCommand.CanExecuteChangedRaised);
        }

        [Fact]
        public void UnregisteringCommandWithNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var compositeCommand = new CompositeCommand();
                compositeCommand.UnregisterCommand(null);
            });
        }

        [Fact]
        public void UnregisterCommandRemovesFromExecuteDelegation()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.UnregisterCommand(testCommandOne);

            Assert.False(testCommandOne.ExecuteCalled);
            multiCommand.Execute(null);
            Assert.False(testCommandOne.ExecuteCalled);
        }

        [Fact]
        public void UnregisterCommandShouldRaiseCanExecuteEvent()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand();

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.CanExecuteChangedRaised = false;
            multiCommand.UnregisterCommand(testCommandOne);

            Assert.True(multiCommand.CanExecuteChangedRaised);
        }

        [Fact]
        public void ExecuteDoesNotThrowWhenAnExecuteCommandModifiesTheCommandsCollection()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            SelfUnregisterableCommand commandOne = new SelfUnregisterableCommand(multiCommand);
            SelfUnregisterableCommand commandTwo = new SelfUnregisterableCommand(multiCommand);

            multiCommand.RegisterCommand(commandOne);
            multiCommand.RegisterCommand(commandTwo);

            multiCommand.Execute(null);

            Assert.True(commandOne.ExecutedCalled);
            Assert.True(commandTwo.ExecutedCalled);
        }

        [Fact]
        public void UnregisterCommandDisconnectsCanExecuteChangedDelegate()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.UnregisterCommand(testCommandOne);
            multiCommand.CanExecuteChangedRaised = false;
            testCommandOne.FireCanExecuteChanged();
            Assert.False(multiCommand.CanExecuteChangedRaised);
        }

        [Fact]
        public void UnregisterCommandDisconnectsIsActiveChangedDelegate()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand(true);
            MockActiveAwareCommand commandOne = new MockActiveAwareCommand() { IsActive = true, IsValid = true };
            MockActiveAwareCommand commandTwo = new MockActiveAwareCommand() { IsActive = false, IsValid = false };
            activeAwareCommand.RegisterCommand(commandOne);
            activeAwareCommand.RegisterCommand(commandTwo);

            Assert.True(activeAwareCommand.CanExecute(null));

            activeAwareCommand.UnregisterCommand(commandOne);

            Assert.False(activeAwareCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldBubbleException()
        {
            Assert.Throws<DivideByZeroException>(() =>
            {
                TestableCompositeCommand multiCommand = new TestableCompositeCommand();
                BadDivisionCommand testCommand = new BadDivisionCommand();

                multiCommand.RegisterCommand(testCommand);
                multiCommand.Execute(null);
            });
        }

        [Fact]
        public void CanExecuteShouldReturnFalseWithNoCommandsRegistered()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            Assert.False(multiCommand.CanExecute(null));
        }

        [Fact]
        public void MultiDispatchCommandExecutesActiveRegisteredCommands()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand();
            MockActiveAwareCommand command = new MockActiveAwareCommand()
            {
                IsActive = true
            };
            activeAwareCommand.RegisterCommand(command);

            activeAwareCommand.Execute(null);

            Assert.True(command.WasExecuted);
        }

        [Fact]
        public void MultiDispatchCommandDoesNotExecutesInactiveRegisteredCommands()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand(true);
            MockActiveAwareCommand command = new MockActiveAwareCommand()
            {
                IsActive = false
            };
            activeAwareCommand.RegisterCommand(command);

            activeAwareCommand.Execute(null);

            Assert.False(command.WasExecuted);
        }

        [Fact]
        public void DispatchCommandDoesNotIncludeInactiveRegisteredCommandInVoting()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand(true);
            MockActiveAwareCommand command = new MockActiveAwareCommand();
            activeAwareCommand.RegisterCommand(command);
            command.IsValid = true;
            command.IsActive = false;

            Assert.False(activeAwareCommand.CanExecute(null), "Registered Click is inactive so should not participate in CanExecute vote");

            command.IsActive = true;

            Assert.True(activeAwareCommand.CanExecute(null));

            command.IsValid = false;

            Assert.False(activeAwareCommand.CanExecute(null));

        }

        [Fact]
        public void DispatchCommandShouldIgnoreInactiveCommandsInCanExecuteVote()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand(true);
            MockActiveAwareCommand commandOne = new MockActiveAwareCommand() { IsActive = false, IsValid = false };
            MockActiveAwareCommand commandTwo = new MockActiveAwareCommand() { IsActive = true, IsValid = true };

            activeAwareCommand.RegisterCommand(commandOne);
            activeAwareCommand.RegisterCommand(commandTwo);

            Assert.True(activeAwareCommand.CanExecute(null));
        }

        [Fact]
        public void ActivityCausesActiveAwareCommandToRequeryCanExecute()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand(true);
            MockActiveAwareCommand command = new MockActiveAwareCommand();
            activeAwareCommand.RegisterCommand(command);
            command.IsActive = true;

            bool globalCanExecuteChangeFired = false;
            activeAwareCommand.CanExecuteChanged += delegate
                                                        {
                                                            globalCanExecuteChangeFired = true;
                                                        };

            Assert.False(globalCanExecuteChangeFired);
            command.IsActive = false;
            Assert.True(globalCanExecuteChangeFired);
        }

        [Fact]
        public void ShouldNotMonitorActivityIfUseActiveMonitoringFalse()
        {
            var mockCommand = new MockActiveAwareCommand()
            {
                IsValid = true,
                IsActive = true
            };
            var nonActiveAwareCompositeCommand = new CompositeCommand(false);
            bool canExecuteChangedRaised = false;
            nonActiveAwareCompositeCommand.RegisterCommand(mockCommand);
            nonActiveAwareCompositeCommand.CanExecuteChanged += delegate
            {
                canExecuteChangedRaised = true;
            };

            mockCommand.IsActive = false;

            Assert.False(canExecuteChangedRaised);

            nonActiveAwareCompositeCommand.Execute(null);

            Assert.True(mockCommand.WasExecuted);
        }

        [Fact]
        public void ShouldRemoveCanExecuteChangedHandler()
        {
            bool canExecuteChangedRaised = false;

            var compositeCommand = new CompositeCommand();
            var commmand = new DelegateCommand(() => { });
            compositeCommand.RegisterCommand(commmand);

            EventHandler handler = (s, e) => canExecuteChangedRaised = true;

            compositeCommand.CanExecuteChanged += handler;
            commmand.RaiseCanExecuteChanged();

            Assert.True(canExecuteChangedRaised);

            canExecuteChangedRaised = false;
            compositeCommand.CanExecuteChanged -= handler;
            commmand.RaiseCanExecuteChanged();

            Assert.False(canExecuteChangedRaised);
        }

        [Fact]
        public void ShouldIgnoreChangesToIsActiveDuringExecution()
        {
            var firstCommand = new MockActiveAwareCommand { IsActive = true };
            var secondCommand = new MockActiveAwareCommand { IsActive = true };

            // During execution set the second command to inactive, this should not affect the currently
            // executed selection.  
            firstCommand.ExecuteAction += new Action<object>((object parameter) => secondCommand.IsActive = false);

            var compositeCommand = new CompositeCommand(true);

            compositeCommand.RegisterCommand(firstCommand);
            compositeCommand.RegisterCommand(secondCommand);

            compositeCommand.Execute(null);

            Assert.True(secondCommand.WasExecuted);
        }

        [Fact]
        public void RegisteringCommandInItselfThrows()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var compositeCommand = new CompositeCommand();
                compositeCommand.RegisterCommand(compositeCommand);
            });
        }

        [Fact]
        public void RegisteringCommandWithNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var compositeCommand = new CompositeCommand();
                compositeCommand.RegisterCommand(null);
            });
        }

        [Fact]
        public void RegisteringCommandTwiceThrows()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var compositeCommand = new CompositeCommand();
                var duplicateCommand = new TestCommand();
                compositeCommand.RegisterCommand(duplicateCommand);

                compositeCommand.RegisterCommand(duplicateCommand);
            });

        }

        [Fact]
        public void ShouldGetRegisteredCommands()
        {
            var firstCommand = new TestCommand();
            var secondCommand = new TestCommand();

            var compositeCommand = new CompositeCommand();
            compositeCommand.RegisterCommand(firstCommand);
            compositeCommand.RegisterCommand(secondCommand);

            var commands = compositeCommand.RegisteredCommands;

            Assert.True(commands.Count > 0);
        }
    }

    internal class MockActiveAwareCommand : IActiveAware, ICommand
    {
        private bool _isActive;

        public Action<object> ExecuteAction;


        #region IActiveAware Members

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnActiveChanged(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler IsActiveChanged = delegate { };
        #endregion

        virtual protected void OnActiveChanged(object sender, EventArgs e)
        {
            IsActiveChanged(sender, e);
        }

        public bool WasExecuted { get; set; }
        public bool IsValid { get; set; }


        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return IsValid;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            WasExecuted = true;
            ExecuteAction?.Invoke(parameter);
        }

        #endregion
    }

    internal class TestableCompositeCommand : CompositeCommand
    {
        public bool CanExecuteChangedRaised;
        private EventHandler handler;

        public TestableCompositeCommand()
        {
            this.handler = ((sender, e) => CanExecuteChangedRaised = true);
            CanExecuteChanged += this.handler;
        }
    }

    internal class TestCommand : ICommand
    {
        public bool CanExecuteCalled { get; set; }
        public bool ExecuteCalled { get; set; }
        public int ExecuteCallCount { get; set; }

        public bool CanExecuteValue = true;

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            CanExecuteCalled = true;
            return CanExecuteValue;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            ExecuteCalled = true;
            ExecuteCallCount += 1;
        }

        #endregion
    }

    internal class BadDivisionCommand : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            throw new DivideByZeroException("Test Divide By Zero");
        }

        #endregion
    }

    internal class SelfUnregisterableCommand : ICommand
    {
        public CompositeCommand Command;
        public bool ExecutedCalled = false;

        public SelfUnregisterableCommand(CompositeCommand command)
        {
            Command = command;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public void Execute(object parameter)
        {
            Command.UnregisterCommand(this);
            ExecutedCalled = true;
        }

        #endregion
    }
}
