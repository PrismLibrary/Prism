

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Interactivity.InteractionRequest;

namespace Prism.Wpf.Tests.Interactivity
{
    [TestClass]
    public class InteractionRequestFixture
    {
        [TestMethod]
        public void WhenANotificationIsRequested_ThenTheEventIsRaisedWithTheSuppliedContext()
        {
            var request = new InteractionRequest<Notification>();
            object suppliedContext = null;
            request.Raised += (o, e) => suppliedContext = e.Context;

            var context = new Notification();

            request.Raise(context, c => { });

            Assert.AreSame(context, suppliedContext);
        }

        [TestMethod]
        public void WhenTheEventCallbackIsExecuted_ThenTheNotifyCallbackIsInvokedWithTheSuppliedContext()
        {
            var request = new InteractionRequest<Notification>();
            Action eventCallback = null;
            request.Raised += (o, e) => eventCallback = e.Callback;

            var context = new Notification();
            object suppliedContext = null;

            request.Raise(context, c => { suppliedContext = c; });

            eventCallback();

            Assert.AreSame(context, suppliedContext);
        }

        [TestMethod]
        public async Task WhenEventIsRaisedAsyncDialog_NotificationIsPassedBackAsync()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var request = new InteractionRequest<INotification>();

            MockPopupDialogWindow popup = new MockPopupDialogWindow();
            popup.Attach(request);

            var notification = new Notification();

            Assert.IsTrue(popup.ExecutionCount == 0);

            var result = await request.RaiseAsync(notification);
            Assert.IsTrue(popup.ExecutionCount == 1);
            Assert.ReferenceEquals(notification, result);
        }

        [TestMethod]
        public void WhenEventIsRaisedSyncDialog_NotificationIsPassedBackSync()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var request = new InteractionRequest<INotification>();

            MockPopupDialogWindow popup = new MockPopupDialogWindow();
            popup.Attach(request);

            var notification = new Notification();

            Assert.IsTrue(popup.ExecutionCount == 0);

            var task = request.RaiseAsync(notification, executeSynchronously: true);
            Assert.IsTrue(task.IsCompleted);

            Assert.IsTrue(popup.ExecutionCount == 1);
            Assert.ReferenceEquals(notification, task.Result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenEventIsRaisedAsyncDialog_ThrowsBeforeCallback()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var request = new InteractionRequest<INotification>();

            MockPopupDialogWindow popup = new MockPopupDialogWindow { ThrowsBeforeCallback = true };
            popup.Attach(request);

            var notification = new Notification();

            await request.RaiseAsync(notification);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenEventIsRaisedAsyncDialog_ThrowsAfterCallback()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var request = new InteractionRequest<INotification>();

            MockPopupDialogWindow popup = new MockPopupDialogWindow { ThrowsAfterCallback = true };
            popup.Attach(request);

            var notification = new Notification();

            await request.RaiseAsync(notification);
        }

        [TestMethod]
        public async Task WhenEventIsRaisedAsync_NotificationIsPassedBackAsync()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var request = new InteractionRequest<INotification>();

            MockPopupWindow popup = new MockPopupWindow();
            popup.Attach(request);

            var notification = new Notification();

            var task = request.RaiseAsync(notification);

            await Task.Delay(200);
            bool completed = task.Wait(0);
            Assert.IsFalse(completed);

            popup.Confirm();

            var result = await task;
            Assert.ReferenceEquals(notification, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenEventIsRaisedAsync_Throws()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var request = new InteractionRequest<INotification>();

            MockPopupWindow popup = new MockPopupWindow { Throws = true };
            popup.Attach(request);

            var notification = new Notification();

            await request.RaiseAsync(notification);
        }
    }

    public class MockPopupDialogWindow
    {
        public int ExecutionCount { get; private set; }

        public bool ThrowsBeforeCallback { get; set; }
        public bool ThrowsAfterCallback { get; set; }

        public void Attach(IInteractionRequest request)
        {
            request.Raised += (s, args) =>
            {
                this.ExecutionCount++;
                if (this.ThrowsBeforeCallback)
                {
                    throw new InvalidOperationException();
                }
                args.Callback();
                if (this.ThrowsAfterCallback)
                {
                    throw new InvalidOperationException();
                }
            };
        }
    }

    public class MockPopupWindow
    {
        private Action callback;

        public bool Throws { get; set; }

        public void Attach(IInteractionRequest request)
        {
            request.Raised += (s, args) =>
            {
                if (this.Throws)
                {
                    throw new InvalidOperationException();
                }

                this.callback = args.Callback;
            };
        }

        public void Confirm()
        {
            this.callback();
        }
    }
}
