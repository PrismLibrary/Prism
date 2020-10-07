using System;
using System.Threading.Tasks;
using Prism.Events;
using Xunit;

namespace Prism.Tests.Events
{
    public class DelegateReferenceFixture
    {
        [Fact]
        public void KeepAlivePreventsDelegateFromBeingCollected()
        {
            var delegates = new SomeClassHandler();
            var delegateReference = new DelegateReference((Action<string>)delegates.DoEvent, true);

            delegates = null;
            GC.Collect();

            Assert.NotNull(delegateReference.Target);
        }

        [Fact]
        public async Task NotKeepAliveAllowsDelegateToBeCollected()
        {
            var delegates = new SomeClassHandler();
            var delegateReference = new DelegateReference((Action<string>)delegates.DoEvent, false);

            delegates = null;
            await Task.Delay(100);
            GC.Collect();

            Assert.Null(delegateReference.Target);
        }

        [Fact]
        public async Task NotKeepAliveKeepsDelegateIfStillAlive()
        {
            var delegates = new SomeClassHandler();
            var delegateReference = new DelegateReference((Action<string>)delegates.DoEvent, false);

            GC.Collect();

            Assert.NotNull(delegateReference.Target);

            GC.KeepAlive(delegates);  //Makes delegates ineligible for garbage collection until this point (to prevent oompiler optimizations that may release the referenced object prematurely).
            delegates = null;
            await Task.Delay(100);
            GC.Collect();

            Assert.Null(delegateReference.Target);
        }

        [Fact]
        public void TargetShouldReturnAction()
        {
            var classHandler = new SomeClassHandler();
            Action<string> myAction = new Action<string>(classHandler.MyAction);

            var weakAction = new DelegateReference(myAction, false);

            ((Action<string>)weakAction.Target)("payload");
            Assert.Equal("payload", classHandler.MyActionArg);
        }

        [Fact]
        public async Task ShouldAllowCollectionOfOriginalDelegate()
        {
            var classHandler = new SomeClassHandler();
            Action<string> myAction = new Action<string>(classHandler.MyAction);

            var weakAction = new DelegateReference(myAction, false);

            var originalAction = new WeakReference(myAction);
            myAction = null;
            await Task.Delay(100);
            GC.Collect();
            Assert.False(originalAction.IsAlive);

            ((Action<string>)weakAction.Target)("payload");
            Assert.Equal("payload", classHandler.MyActionArg);
        }

        [Fact]
        public async Task ShouldReturnNullIfTargetNotAlive()
        {
            SomeClassHandler handler = new SomeClassHandler();
            var weakHandlerRef = new WeakReference(handler);

            var action = new DelegateReference((Action<string>)handler.DoEvent, false);

            handler = null;
            await Task.Delay(100);
            GC.Collect();
            Assert.False(weakHandlerRef.IsAlive);

            Assert.Null(action.Target);
        }

        [Fact]
        public void WeakDelegateWorksWithStaticMethodDelegates()
        {
            var action = new DelegateReference((Action)SomeClassHandler.StaticMethod, false);

            Assert.NotNull(action.Target);
        }

        [Fact]
        public void TargetEqualsActionShouldReturnTrue()
        {
            var classHandler = new SomeClassHandler();
            Action<string> myAction = new Action<string>(classHandler.MyAction);

            var weakAction = new DelegateReference(myAction, false);

            Assert.True(weakAction.TargetEquals(new Action<string>(classHandler.MyAction)));
        }

        [Fact]
        public async Task TargetEqualsNullShouldReturnTrueIfTargetNotAlive()
        {
            SomeClassHandler handler = new SomeClassHandler();
            var weakHandlerRef = new WeakReference(handler);

            var action = new DelegateReference((Action<string>)handler.DoEvent, false);

            handler = null;

            // Intentional delay to encourage Garbage Collection to actually occur
            await Task.Delay(100);
            GC.Collect();
            Assert.False(weakHandlerRef.IsAlive);

            Assert.True(action.TargetEquals(null));
        }

        [Fact]
        public void TargetEqualsNullShouldReturnFalseIfTargetAlive()
        {
            SomeClassHandler handler = new SomeClassHandler();
            var weakHandlerRef = new WeakReference(handler);

            var action = new DelegateReference((Action<string>)handler.DoEvent, false);

            Assert.False(action.TargetEquals(null));
            Assert.True(weakHandlerRef.IsAlive);
            GC.KeepAlive(handler);
        }

        [Fact]
        public void TargetEqualsWorksWithStaticMethodDelegates()
        {
            var action = new DelegateReference((Action)SomeClassHandler.StaticMethod, false);

            Assert.True(action.TargetEquals((Action)SomeClassHandler.StaticMethod));
        }

        //todo: fix
        //[Fact]
        //public void NullDelegateThrows()
        //{
        //    Assert.ThrowsException<ArgumentNullException>(() =>
        //    {
        //        var action = new DelegateReference(null, true);
        //    });
        //}

        public class SomeClassHandler
        {
            public string MyActionArg;

            public void DoEvent(string value)
            {
                string myValue = value;
            }

            public static void StaticMethod()
            {
#pragma warning disable 0219
                int i = 0;
#pragma warning restore 0219
            }

            public void MyAction(string arg)
            {
                MyActionArg = arg;
            }
        }
    }
}
