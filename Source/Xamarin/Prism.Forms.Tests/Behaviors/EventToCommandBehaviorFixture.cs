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
        public void Command_EventArgsParameterPath()
        {
            const string item = "ItemProperty";
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                EventArgsParameterPath = "Item",
                Command = new Command(o =>
                {
                    Assert.NotNull(o);
                    Assert.Equal("ItemProperty", o);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, item));
        }

        [Fact]
        public void Command_CanExecute()
        {
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                Command = new Command(_ => Assert.True(false), o =>
                {
                    Assert.Null(o);
                    return false;
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, null);
        }

        [Fact]
        public void Command_Execute()
        {
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                Command = new Command(Assert.Null)
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, null);
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