// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Commands;

namespace Prism.Tests.Mvvm
{
    /// <summary>
    /// Summary description for DelegateCommandFixture
    /// </summary>
    [TestClass]
    public class DelegateCommandFixture
    {
        [TestMethod]
        public void WhenConstructedWithGenericTypeOfObject_InitializesValues()
        {
            // Prepare

            // Act
            var actual = new DelegateCommand<object>(param => { });

            // verify
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenConstructedWithGenericTypeOfNullable_InitializesValues()
        {
            // Prepare

            // Act
            var actual = new DelegateCommand<int?>(param => {});

            // verify
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void WhenConstructedWithGenericTypeIsNonNullableValueType_Throws()
        {
            // Prepare

            // Act
            var actual = new DelegateCommand<int>(param => { });

            // verify
        }

        [TestMethod]
        public void ExecuteCallsPassedInExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute);
            object parameter = new object();

            command.Execute(parameter);

            Assert.AreSame(parameter, handlers.ExecuteParameter);
        }

        [TestMethod]
        public void CanExecuteCallsPassedInCanExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute, handlers.CanExecute);
            object parameter = new object();

            handlers.CanExecuteReturnValue = true;
            bool retVal = command.CanExecute(parameter);

            Assert.AreSame(parameter, handlers.CanExecuteParameter);
            Assert.AreEqual(handlers.CanExecuteReturnValue, retVal);
        }

        [TestMethod]
        public void CanExecuteReturnsTrueWithouthCanExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute);

            bool retVal = command.CanExecute(null);

            Assert.AreEqual(true, retVal);
        }


        [TestMethod]
        public void RaiseCanExecuteChangedRaisesCanExecuteChanged()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute);
            bool canExecuteChangedRaised = false;
            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            command.RaiseCanExecuteChanged();

            Assert.IsTrue(canExecuteChangedRaised);
        }

        //rlb: Removed, there is no point to allow a DelegateCommand without an execute method action.
        //[TestMethod]
        //public void ShouldAllowOnlyCanExecuteDelegate()
        //{
        //    bool canExecuteCalled = false;
        //    var command = new DelegateCommand<object>(null, delegate
        //    {
        //        canExecuteCalled = true;
        //        return true;
        //    });
        //    command.CanExecute(0);
        //    Assert.IsTrue(canExecuteCalled);
        //}

        [TestMethod]
        public void ShouldPassParameterInstanceOnExecute()
        {
            bool executeCalled = false;
            MyClass testClass = new MyClass();
            ICommand command = new DelegateCommand<MyClass>(delegate(MyClass parameter)
            {
                Assert.AreSame(testClass, parameter);
                executeCalled = true;
            });

            command.Execute(testClass);
            Assert.IsTrue(executeCalled);
        }

        [TestMethod]
        public void ShouldPassParameterInstanceOnCanExecute()
        {
            bool canExecuteCalled = false;
            MyClass testClass = new MyClass();
            ICommand command = new DelegateCommand<MyClass>((p)=>{}, delegate(MyClass parameter)
            {
                Assert.AreSame(testClass, parameter);
                canExecuteCalled = true;
                return true;
            });

            command.CanExecute(testClass);
            Assert.IsTrue(canExecuteCalled);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfAllDelegatesAreNull()
        {
            var command = new DelegateCommand<object>(null, null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfExecuteMethodDelegateNull()
        {
            var command = new DelegateCommand<object>(null);
        }

        [TestMethod]
        public void IsActivePropertyChangeFiresEvent()
        {
            bool fired = false;
            var command = new DelegateCommand<object>(DoNothing);
            command.IsActiveChanged += delegate { fired = true; };
            command.IsActive = true;

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldKeepWeakReferenceToOnCanExecuteChangedHandlers()
        {
            var command = new DelegateCommand<MyClass>((MyClass c) => { });

            var handlers = new CanExecutChangeHandler();
            var weakHandlerRef = new WeakReference(handlers);
            command.CanExecuteChanged += handlers.CanExecuteChangeHandler;
            handlers = null;

            GC.Collect();
            command.RaiseCanExecuteChanged();

            Assert.IsFalse(weakHandlerRef.IsAlive);
            Assert.IsNotNull(command); // Only here to ensure command survives optimizations and the GC.Collect
        }

        [TestMethod]
        public void NonGenericDelegateCommandExecuteShouldInvokeExecuteAction()
        {
            bool executed = false;
            var command = new DelegateCommand(() => { executed = true; });
            command.Execute();

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void NonGenericDelegateCommandCanExecuteShouldInvokeCanExecuteFunc()
        {
            bool invoked = false;
            var command = new DelegateCommand(() => { }, () => { invoked = true; return true; });

            bool canExecute = command.CanExecute();

            Assert.IsTrue(invoked);
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        public void NonGenericDelegateCommandShouldDefaultCanExecuteToTrue()
        {
            var command = new DelegateCommand(() => { });
            Assert.IsTrue(command.CanExecute());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NonGenericDelegateThrowsIfDelegatesAreNull()
        {
            var command = new DelegateCommand(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NonGenericDelegateCommandThrowsIfExecuteDelegateIsNull()
        {
            var command = new DelegateCommand(null);
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
