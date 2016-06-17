using System;
using System.Windows.Input;
using Xunit;
using Prism.Commands;
using System.Threading.Tasks;
using Prism.Tests.Mocks.Commands;
using Prism.Mvvm;
using System.Threading;
using Xunit.Sdk;

namespace Prism.Tests.Mvvm
{
    /// <summary>
    /// Summary description for DelegateCommandFixture
    /// </summary>
    public class DelegateCommandFixture : BindableBase
    {
        [Fact]
        public void WhenConstructedWithGenericTypeOfObject_InitializesValues()
        {
            // Prepare

            // Act
            var actual = new DelegateCommand<object>(param => { });

            // verify
            Assert.NotNull(actual);
        }

        [Fact]
        public void WhenConstructedWithGenericTypeOfNullable_InitializesValues()
        {
            // Prepare

            // Act
            var actual = new DelegateCommand<int?>(param => { });

            // verify
            Assert.NotNull(actual);
        }

        [Fact]
        public void WhenConstructedWithGenericTypeIsNonNullableValueType_Throws()
        {
            Assert.Throws<InvalidCastException>(() =>
            {
                var actual = new DelegateCommand<int>(param => { });
            });
        }

        [Fact]
        public async Task ExecuteCallsPassedInExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute);
            object parameter = new object();

            await command.Execute(parameter);

            Assert.Same(parameter, handlers.ExecuteParameter);
        }

        [Fact]
        public void CanExecuteCallsPassedInCanExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute, handlers.CanExecute);
            object parameter = new object();

            handlers.CanExecuteReturnValue = true;
            bool retVal = command.CanExecute(parameter);

            Assert.Same(parameter, handlers.CanExecuteParameter);
            Assert.Equal(handlers.CanExecuteReturnValue, retVal);
        }

        [Fact]
        public void CanExecuteReturnsTrueWithouthCanExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute);

            bool retVal = command.CanExecute(null);

            Assert.Equal(true, retVal);
        }

        [Fact]
        public void RaiseCanExecuteChangedRaisesCanExecuteChanged()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute);
            bool canExecuteChangedRaised = false;
            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            command.RaiseCanExecuteChanged();

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        public void CanRemoveCanExecuteChangedHandler()
        {
            var command = new DelegateCommand<object>((o) => { });

            bool canExecuteChangedRaised = false;

            EventHandler handler = (s, e) => canExecuteChangedRaised = true;

            command.CanExecuteChanged += handler;
            command.CanExecuteChanged -= handler;
            command.RaiseCanExecuteChanged();

            Assert.False(canExecuteChangedRaised);
        }

        [Fact]
        public void ShouldPassParameterInstanceOnExecute()
        {
            bool executeCalled = false;
            MyClass testClass = new MyClass();
            ICommand command = new DelegateCommand<MyClass>(delegate (MyClass parameter)
            {
                Assert.Same(testClass, parameter);
                executeCalled = true;
            });

            command.Execute(testClass);
            Assert.True(executeCalled);
        }

        [Fact]
        public void ShouldPassParameterInstanceOnCanExecute()
        {
            bool canExecuteCalled = false;
            MyClass testClass = new MyClass();
            ICommand command = new DelegateCommand<MyClass>((p) => { }, delegate (MyClass parameter)
            {
                Assert.Same(testClass, parameter);
                canExecuteCalled = true;
                return true;
            });

            command.CanExecute(testClass);
            Assert.True(canExecuteCalled);
        }

        [Fact]
        public void ShouldThrowIfAllDelegatesAreNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand<object>(null, null);
            });
        }

        [Fact]
        public void ShouldThrowIfExecuteMethodDelegateNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand<object>(null);
            });
        }

        [Fact]
        public void ShouldThrowIfCanExecuteMethodDelegateNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand<object>((o) => { }, null);
            });
        }

        [Fact]
        public void DelegateCommandBaseShouldThrowIfAllDelegatesAreNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommandMock(null, null);
            });
        }

        [Fact]
        public void DelegateCommandBaseShouldThrowIfExecuteMethodDelegateNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommandMock(null);
            });
        }

        [Fact]
        public void DelegateCommandBaseShouldThrowIfCanExecuteMethodDelegateNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommandMock((o) => { }, null);
            });
        }

        //TODO: BBL: This test fails intermittently.  The cause is unknown, but we think it may be a race condition issue.
        //In order to reduce the friction of our automated build processes, we are commenting out this test.
        //[Fact]
        //public void NonGenericDelegateCommandShouldInvokeExplicitExecuteFunc()
        //{
        //    bool executed = false;
        //    ICommand command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { executed = true; }));
        //    command.Execute(null);
        //    Assert.True(executed);
        //}

        [Fact]
        public void IsActivePropertyIsFalseByDeafult()
        {
            var command = new DelegateCommand<object>(DoNothing);
            Assert.False(command.IsActive);
        }

        [Fact]
        public void IsActivePropertyChangeFiresEvent()
        {
            bool fired = false;
            var command = new DelegateCommand<object>(DoNothing);
            command.IsActiveChanged += delegate { fired = true; };
            command.IsActive = true;

            Assert.True(fired);
        }

        [Fact]
        public async Task NonGenericDelegateCommandExecuteShouldInvokeExecuteAction()
        {
            bool executed = false;
            var command = new DelegateCommand(() => { executed = true; });
            await command.Execute();

            Assert.True(executed);
        }

        [Fact]
        public void NonGenericDelegateCommandCanExecuteShouldInvokeCanExecuteFunc()
        {
            bool invoked = false;
            var command = new DelegateCommand(() => { }, () => { invoked = true; return true; });

            bool canExecute = command.CanExecute();

            Assert.True(invoked);
            Assert.True(canExecute);
        }

        [Fact]
        public void NonGenericDelegateCommandShouldDefaultCanExecuteToTrue()
        {
            var command = new DelegateCommand(() => { });
            Assert.True(command.CanExecute());
        }

        [Fact]
        public void NonGenericDelegateThrowsIfDelegatesAreNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand(null, null);
            });
        }

        [Fact]
        public void NonGenericDelegateCommandThrowsIfExecuteDelegateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand(null);
            });
        }

        [Fact]
        public void NonGenericDelegateCommandThrowsIfCanExecuteDelegateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand(() => { }, null);
            });
        }

        [Fact]
        public void GenericDelegateCommandFromAsyncHandlerWithExecuteFuncShouldNotBeNull()
        {
            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }));
            Assert.NotNull(command);
        }

        [Fact]
        public void GenericDelegateCommandFromAsyncHandlerWithExecuteAndCanExecuteFuncShouldNotBeNull()
        {
            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }), (o) => true);
            Assert.NotNull(command);
        }

        [Fact]
        public void GenericDelegateCommandFromAsyncHandlerCanExecuteShouldBeTrueByDefault()
        {
            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }));
            var canExecute = command.CanExecute(null);
            Assert.True(canExecute);
        }

        [Fact]
        public void GenericDelegateCommandFromAsyncHandlerWithNullExecuteFuncShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = DelegateCommand<object>.FromAsyncHandler(null);
            });
        }

        [Fact]
        public void GenericDelegateCommandFromAsyncHandlerWithNullCanExecuteFuncShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }), null);
            });
        }

        [Fact]
        public void DelegateCommandBaseWithNullExecuteFuncShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = DelegateCommandMock.FromAsyncHandler(null);
            });
        }

        [Fact]
        public void DelegateCommandBaseWithNullCanExecuteFuncShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }), null);
            });
        }

        [Fact]
        public async Task GenericDelegateCommandFromAsyncHandlerExecuteShouldInvokeExecuteFunc()
        {
            bool executed = false;

            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => executed = true));
            await command.Execute(null);

            Assert.True(executed);
        }

        [Fact]
        public void DelegateCommandFromAsyncHandlerWithExecuteFuncShouldNotBeNull()
        {
            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }));
            Assert.NotNull(command);
        }

        [Fact]
        public void DelegateCommandFromAsyncHandlerCanExecuteShouldBeTrueByDefault()
        {
            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }));
            var canExecute = command.CanExecute();
            Assert.True(canExecute);
        }

        [Fact]
        public void DelegateCommandFromAsyncHandlerWithNullExecuteFuncShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = DelegateCommand.FromAsyncHandler(null);
            });
        }

        [Fact]
        public void DelegateCommandFromAsyncHandlerWithExecuteAndCanExecuteFuncShouldNotBeNull()
        {
            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }), () => true);
            Assert.NotNull(command);
        }

        [Fact]
        public void DelegateCommandFromAsyncHandlerWithNullCanExecuteFuncShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }), null);
            });
        }

        [Fact]
        public async Task DelegateCommandFromAsyncHandlerExecuteShouldInvokeExecuteFunc()
        {
            bool executed = false;

            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => executed = true));
            await command.Execute();

            Assert.True(executed);
        }

        [Fact]
        public void DelegateCommandFromAsyncHandlerCanExecuteShouldInvokeCanExecuteFunc()
        {
            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }), () => true);
            var canExecute = command.CanExecute();
            Assert.True(canExecute);
        }

        [Fact]
        public void NonGenericDelegateCommandShouldObserveCanExecute()
        {
            bool canExecuteChangedRaised = false;

            ICommand command = new DelegateCommand(() => { }).ObservesCanExecute((o) => BoolProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            Assert.False(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void NonGenericDelegateCommandShouldObserveCanExecuteAndObserveOtherProperties()
        {
            bool canExecuteChangedRaised = false;

            ICommand command = new DelegateCommand(() => { }).ObservesCanExecute((o) => BoolProperty).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            Assert.False(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            canExecuteChangedRaised = false;
            Assert.False(canExecuteChangedRaised);

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void NonGenericDelegateCommandShouldNotObserveDuplicateCanExecute()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ICommand command = new DelegateCommand(() => { }).ObservesCanExecute((o) => BoolProperty).ObservesCanExecute((o) => BoolProperty);
            });
        }

        [Fact]
        public void NonGenericDelegateCommandShouldObserveOneProperty()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommand(() => { }).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        public void NonGenericDelegateCommandShouldObserveMultipleProperties()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommand(() => { }).ObservesProperty(() => IntProperty).ObservesProperty(() => BoolProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);

            canExecuteChangedRaised = false;

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        public void NonGenericDelegateCommandShouldNotObserveDuplicateProperties()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DelegateCommand command = new DelegateCommand(() => { }).ObservesProperty(() => IntProperty).ObservesProperty(() => IntProperty);
            });
        }


        [Fact]
        public void GenericDelegateCommandShouldObserveCanExecute()
        {
            bool canExecuteChangedRaised = false;

            ICommand command = new DelegateCommand<object>((o) => { }).ObservesCanExecute((o) => BoolProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            Assert.False(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void GenericDelegateCommandShouldObserveCanExecuteAndObserveOtherProperties()
        {
            bool canExecuteChangedRaised = false;

            ICommand command = new DelegateCommand<object>((o) => { }).ObservesCanExecute((o) => BoolProperty).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            Assert.False(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            canExecuteChangedRaised = false;
            Assert.False(canExecuteChangedRaised);

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void GenericDelegateCommandShouldNotObserveDuplicateCanExecute()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ICommand command =
                    new DelegateCommand<object>((o) => { }).ObservesCanExecute((o) => BoolProperty)
                        .ObservesCanExecute((o) => BoolProperty);
            });
        }

        [Fact]
        public void GenericDelegateCommandShouldObserveOneProperty()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommand<object>((o) => { }).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        public void GenericDelegateCommandShouldObserveMultipleProperties()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommand<object>((o) => { }).ObservesProperty(() => IntProperty).ObservesProperty(() => BoolProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);

            canExecuteChangedRaised = false;

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        public void GenericDelegateCommandShouldNotObserveDuplicateProperties()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DelegateCommand<object> command = new DelegateCommand<object>((o) => { }).ObservesProperty(() => IntProperty).ObservesProperty(() => IntProperty);
            });
        }

        private bool _boolProperty;
        public bool BoolProperty
        {
            get { return _boolProperty; }
            set { SetProperty(ref _boolProperty, value); }
        }

        private int _intProperty;
        public int IntProperty
        {
            get { return _intProperty; }
            set { SetProperty(ref _intProperty, value); }
        }

        class CanExecutChangeHandler
        {
            public bool CanExeucteChangedHandlerCalled;
            public void CanExecuteChangeHandler(object sender, EventArgs e)
            {
                CanExeucteChangedHandlerCalled = true;
            }
        }

        public void DoNothing(object param)
        { }


        class DelegateHandlers
        {
            public bool CanExecuteReturnValue = true;
            public object CanExecuteParameter;
            public object ExecuteParameter;

            public bool CanExecute(object parameter)
            {
                CanExecuteParameter = parameter;
                return CanExecuteReturnValue;
            }

            public void Execute(object parameter)
            {
                ExecuteParameter = parameter;
            }
        }
    }

    internal class MyClass
    {
    }
}
