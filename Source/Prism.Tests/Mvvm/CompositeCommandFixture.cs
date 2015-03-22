// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Commands;

namespace Prism.Tests.Mvvm
{
    [TestClass]
    public class CompositeCommandFixture
    {
        [TestMethod]
        public void RegisterACommandShouldRaiseCanExecuteEvent()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommand = new TestCommand();

            multiCommand.RegisterCommand(new TestCommand());
            Assert.IsTrue(multiCommand.CanExecuteChangedRaised);
        }

        [TestMethod]
        public void ShouldDelegateExecuteToSingleRegistrant()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommand = new TestCommand();

            multiCommand.RegisterCommand(testCommand);

            Assert.IsFalse(testCommand.ExecuteCalled);
            multiCommand.Execute(null);
            Assert.IsTrue(testCommand.ExecuteCalled);
        }

        [TestMethod]
        public void ShouldDelegateExecuteToMultipleRegistrants()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand();
            TestCommand testCommandTwo = new TestCommand();

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.RegisterCommand(testCommandTwo);

            Assert.IsFalse(testCommandOne.ExecuteCalled);
            Assert.IsFalse(testCommandTwo.ExecuteCalled);
            multiCommand.Execute(null);
            Assert.IsTrue(testCommandOne.ExecuteCalled);
            Assert.IsTrue(testCommandTwo.ExecuteCalled);
        }

        [TestMethod]
        public void ShouldDelegateCanExecuteToSingleRegistrant()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommand = new TestCommand();

            multiCommand.RegisterCommand(testCommand);

            Assert.IsFalse(testCommand.CanExecuteCalled);
            multiCommand.CanExecute(null);
            Assert.IsTrue(testCommand.CanExecuteCalled);
        }

        [TestMethod]
        public void ShouldDelegateCanExecuteToMultipleRegistrants()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand();
            TestCommand testCommandTwo = new TestCommand();

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.RegisterCommand(testCommandTwo);

            Assert.IsFalse(testCommandOne.CanExecuteCalled);
            Assert.IsFalse(testCommandTwo.CanExecuteCalled);
            multiCommand.CanExecute(null);
            Assert.IsTrue(testCommandOne.CanExecuteCalled);
            Assert.IsTrue(testCommandTwo.CanExecuteCalled);
        }

        [TestMethod]
        public void CanExecuteShouldReturnTrueIfAllRegistrantsTrue()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };
            TestCommand testCommandTwo = new TestCommand() { CanExecuteValue = true };

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.RegisterCommand(testCommandTwo);

            Assert.IsTrue(multiCommand.CanExecute(null));
        }

        [TestMethod]
        public void CanExecuteShouldReturnFalseIfASingleRegistrantsFalse()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };
            TestCommand testCommandTwo = new TestCommand() { CanExecuteValue = false };

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.RegisterCommand(testCommandTwo);

            Assert.IsFalse(multiCommand.CanExecute(null));
        }

        [TestMethod]
        public void ShouldReraiseCanExecuteChangedEvent()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };

            Assert.IsFalse(multiCommand.CanExecuteChangedRaised);
            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.CanExecuteChangedRaised = false;

            testCommandOne.FireCanExecuteChanged();
            Assert.IsTrue(multiCommand.CanExecuteChangedRaised);
        }

        [TestMethod]
        public void ShouldReraiseCanExecuteChangedEventAfterCollect()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };

            Assert.IsFalse(multiCommand.CanExecuteChangedRaised);
            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.CanExecuteChangedRaised = false;

            GC.Collect();

            testCommandOne.FireCanExecuteChanged();
            Assert.IsTrue(multiCommand.CanExecuteChangedRaised);
        }

        [TestMethod]
        public void ShouldReraiseDelegateCommandCanExecuteChangedEventAfterCollect()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            DelegateCommand<object> delegateCommand = new DelegateCommand<object>(delegate { });

            Assert.IsFalse(multiCommand.CanExecuteChangedRaised);
            multiCommand.RegisterCommand(delegateCommand);
            multiCommand.CanExecuteChangedRaised = false;

            GC.Collect();

            delegateCommand.RaiseCanExecuteChanged();
            Assert.IsTrue(multiCommand.CanExecuteChangedRaised);
        }

        [TestMethod]
        public void UnregisterCommandRemovesFromExecuteDelegation()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.UnregisterCommand(testCommandOne);

            Assert.IsFalse(testCommandOne.ExecuteCalled);
            multiCommand.Execute(null);
            Assert.IsFalse(testCommandOne.ExecuteCalled);
        }

        [TestMethod]
        public void UnregisterCommandShouldRaiseCanExecuteEvent()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand();

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.CanExecuteChangedRaised = false;
            multiCommand.UnregisterCommand(testCommandOne);

            Assert.IsTrue(multiCommand.CanExecuteChangedRaised);
        }

        [TestMethod]
        public void ExecuteDoesNotThrowWhenAnExecuteCommandModifiesTheCommandsCollection()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            SelfUnregisterableCommand commandOne = new SelfUnregisterableCommand(multiCommand);
            SelfUnregisterableCommand commandTwo = new SelfUnregisterableCommand(multiCommand);

            multiCommand.RegisterCommand(commandOne);
            multiCommand.RegisterCommand(commandTwo);

            multiCommand.Execute(null);

            Assert.IsTrue(commandOne.ExecutedCalled);
            Assert.IsTrue(commandTwo.ExecutedCalled);
        }

        [TestMethod]
        public void UnregisterCommandDisconnectsCanExecuteChangedDelegate()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            TestCommand testCommandOne = new TestCommand() { CanExecuteValue = true };

            multiCommand.RegisterCommand(testCommandOne);
            multiCommand.UnregisterCommand(testCommandOne);
            multiCommand.CanExecuteChangedRaised = false;
            testCommandOne.FireCanExecuteChanged();
            Assert.IsFalse(multiCommand.CanExecuteChangedRaised);
        }

        [TestMethod, ExpectedException(typeof(DivideByZeroException))]
        public void ShouldBubbleException()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            BadDivisionCommand testCommand = new BadDivisionCommand();

            multiCommand.RegisterCommand(testCommand);
            multiCommand.Execute(null);
        }

        [TestMethod]
        public void CanExecuteShouldReturnFalseWithNoCommandsRegistered()
        {
            TestableCompositeCommand multiCommand = new TestableCompositeCommand();
            Assert.IsFalse(multiCommand.CanExecute(null));
        }

        [TestMethod]
        public void MultiDispatchCommandExecutesActiveRegisteredCommands()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand();
            MockActiveAwareCommand command = new MockActiveAwareCommand();
            command.IsActive = true;
            activeAwareCommand.RegisterCommand(command);

            activeAwareCommand.Execute(null);

            Assert.IsTrue(command.WasExecuted);
        }

        [TestMethod]
        public void MultiDispatchCommandDoesNotExecutesInactiveRegisteredCommands()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand(true);
            MockActiveAwareCommand command = new MockActiveAwareCommand();
            command.IsActive = false;
            activeAwareCommand.RegisterCommand(command);

            activeAwareCommand.Execute(null);

            Assert.IsFalse(command.WasExecuted);
        }

        [TestMethod]
        public void DispatchCommandDoesNotIncludeInactiveRegisteredCommandInVoting()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand(true);
            MockActiveAwareCommand command = new MockActiveAwareCommand();
            activeAwareCommand.RegisterCommand(command);
            command.IsValid = true;
            command.IsActive = false;

            Assert.IsFalse(activeAwareCommand.CanExecute(null), "Registered Click is inactive so should not participate in CanExecute vote");

            command.IsActive = true;

            Assert.IsTrue(activeAwareCommand.CanExecute(null));

            command.IsValid = false;

            Assert.IsFalse(activeAwareCommand.CanExecute(null));

        }

        [TestMethod]
        public void DispatchCommandShouldIgnoreInactiveCommandsInCanExecuteVote()
        {
            CompositeCommand activeAwareCommand = new CompositeCommand(true);
            MockActiveAwareCommand commandOne = new MockActiveAwareCommand() { IsActive = false, IsValid = false };
            MockActiveAwareCommand commandTwo = new MockActiveAwareCommand() { IsActive = true, IsValid = true };

            activeAwareCommand.RegisterCommand(commandOne);
            activeAwareCommand.RegisterCommand(commandTwo);

            Assert.IsTrue(activeAwareCommand.CanExecute(null));
        }

        [TestMethod]
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

            Assert.IsFalse(globalCanExecuteChangeFired);
            command.IsActive = false;
            Assert.IsTrue(globalCanExecuteChangeFired);
        }

        [TestMethod]
        public void ShouldNotMonitorActivityIfUseActiveMonitoringFalse()
        {
            var mockCommand = new MockActiveAwareCommand();
            mockCommand.IsValid = true;
            mockCommand.IsActive = true;
            var nonActiveAwareCompositeCommand = new CompositeCommand(false);
            bool canExecuteChangedRaised = false;
            nonActiveAwareCompositeCommand.RegisterCommand(mockCommand);
            nonActiveAwareCompositeCommand.CanExecuteChanged += delegate
            {
                canExecuteChangedRaised = true;
            };

            mockCommand.IsActive = false;

            Assert.IsFalse(canExecuteChangedRaised);

            nonActiveAwareCompositeCommand.Execute(null);

            Assert.IsTrue(mockCommand.WasExecuted);
        }

        [TestMethod]
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

            Assert.IsTrue(secondCommand.WasExecuted);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteringCommandInItselfThrows()
        {
            var compositeCommand = new CompositeCommand();

            compositeCommand.RegisterCommand(compositeCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisteringCommandTwiceThrows()
        {
            var compositeCommand = new CompositeCommand();
            var duplicateCommand = new TestCommand();
            compositeCommand.RegisterCommand(duplicateCommand);

            compositeCommand.RegisterCommand(duplicateCommand);
        }

        [TestMethod]
        public void ShouldKeepWeakReferenceToOnCanExecuteChangedHandlers()
        {
            var command = new TestableCompositeCommand();

            var handlers = new CanExecutChangeHandler();
            var weakHandlerRef = new WeakReference(handlers);
            command.CanExecuteChanged += handlers.CanExecuteChangeHandler;
            handlers = null;

            GC.Collect();

            Assert.IsFalse(weakHandlerRef.IsAlive);
            Assert.IsNotNull(command); // Only here to ensure command survives optimizations and the GC.Collect
        }

        private class CanExecutChangeHandler
        {
            private int callCount = 0;

            public void CanExecuteChangeHandler(object sender, EventArgs e)
            {
                callCount++;
            }
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
            if (ExecuteAction != null)
                ExecuteAction(parameter);
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

        public event EventHandler CanExecuteChanged;

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

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Command.UnregisterCommand(this);
            ExecutedCalled = true;
        }

        #endregion
    }
}
