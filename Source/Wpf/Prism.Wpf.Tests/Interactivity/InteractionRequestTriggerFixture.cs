

using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Interactivity.InteractionRequest;
using Prism.Wpf.Tests.Mocks;
using System.Threading.Tasks;

namespace Prism.Wpf.Tests.Interactivity
{
    [TestClass]
    public class InteractionRequestTriggerFixture
    {
        public InteractionRequest<INotification> SourceProperty { get; set; }

        [TestMethod]
        public void WhenSourceObjectIsSet_ShouldSubscribeToRaisedEvent()
        {
            InteractionRequest<INotification> request = new InteractionRequest<INotification>();
            TestableInteractionRequestTrigger trigger = new TestableInteractionRequestTrigger();
            DependencyObject associatedObject = new DependencyObject();

            trigger.Attach(associatedObject);
            trigger.SourceObject = request;

            Assert.IsTrue(trigger.ExecutionCount == 0);

            request.Raise(new Notification());
            Assert.IsTrue(trigger.ExecutionCount == 1);
        }

        [TestMethod]
        public void WhenEventIsRaised_ShouldExecuteTriggerActions()
        {
            InteractionRequest<INotification> request = new InteractionRequest<INotification>();
            TestableInteractionRequestTrigger trigger = new TestableInteractionRequestTrigger();
            DependencyObject associatedObject = new DependencyObject();
            TestableTriggerAction action = new TestableTriggerAction();

            trigger.Actions.Add(action);
            trigger.Attach(associatedObject);
            trigger.SourceObject = request;

            Assert.IsTrue(action.ExecutionCount == 0);

            request.Raise(new Notification());
            Assert.IsTrue(action.ExecutionCount == 1);
        }
    }

    public class TestableInteractionRequestTrigger : InteractionRequestTrigger
    {
        public int ExecutionCount { get; set; }

        protected override void OnEvent(EventArgs eventArgs)
        {
            this.ExecutionCount++;
            base.OnEvent(eventArgs);
        }
    }

    public class TestableTriggerAction : System.Windows.Interactivity.TriggerAction<DependencyObject>
    {
        public int ExecutionCount { get; set; }

        protected override void Invoke(object parameter)
        {
            this.ExecutionCount++;
            if (parameter is InteractionRequestedEventArgs)
            {
                ((InteractionRequestedEventArgs)parameter).Callback();
            }
        }
    }
}
