using System;
using Prism.Behaviors;
using Prism.Forms.Tests.Mocks.Behaviors;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Behaviors
{
    public class EventToCommandBehaviorFixture
    {
        [Fact]
        public void Command_CanExecute()
        {
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                Command = new Command(_ => Assert.True(false), o =>
                {
                    Assert.True(true);
                    return false;
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(null, null);
        }

        [Fact]
        public void Command_Execute()
        {
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                Command = new Command(_ => Assert.True(true))
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(null, null);
        }

        [Fact]
        public void InvalidEventName()
        {
            var behavior = new EventToCommandBehavior
            {
                EventName = "OnItemTapped"
            };
            var listView = new ListView();
            Assert.Throws<ArgumentException>(() => listView.Behaviors.Add(behavior));
        }

        [Fact]
        public void ValidEventName()
        {
            var behavior = new EventToCommandBehavior
            {
                EventName = "ItemTapped"
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
        }
    }
}