

using System;
using Xunit;
using Prism.Interactivity.InteractionRequest;

namespace Prism.Wpf.Tests.Interactivity
{
    
    public class InteractionRequestFixture
    {
        [Fact]
        public void WhenANotificationIsRequested_ThenTheEventIsRaisedWithTheSuppliedContext()
        {
            var request = new InteractionRequest<Notification>();
            object suppliedContext = null;
            request.Raised += (o, e) => suppliedContext = e.Context;

            var context = new Notification();

            request.Raise(context, c => { });

            Assert.Same(context, suppliedContext);
        }

        [Fact]
        public void WhenTheEventCallbackIsExecuted_ThenTheNotifyCallbackIsInvokedWithTheSuppliedContext()
        {
            var request = new InteractionRequest<Notification>();
            Action eventCallback = null;
            request.Raised += (o, e) => eventCallback = e.Callback;

            var context = new Notification();
            object suppliedContext = null;

            request.Raise(context, c => { suppliedContext = c; });

            eventCallback();

            Assert.Same(context, suppliedContext);
        }
    }
}
