using System;
using System.Globalization;
using Prism.Behaviors;
using Prism.Forms.Tests.Mocks.Behaviors;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Behaviors
{
    public class EventToCommandBehaviorFixture
    {
        private class ItemTappedEventArgsConverter : IValueConverter
        {
            private readonly bool _returnParameter;

            public bool HasConverted { get; private set; }

            public ItemTappedEventArgsConverter(bool returnParameter)
            {
                _returnParameter = returnParameter;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                HasConverted = true;
                return _returnParameter ? parameter : (value as ItemTappedEventArgs)?.Item;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void Command_OrderOfExecution()
        {
            const string commandParameter = "ItemProperty";
            var converter = new ItemTappedEventArgsConverter(false);
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                EventArgsConverter = converter,
                CommandParameter = commandParameter,
                Command = new Command(o =>
                {
                    Assert.NotNull(o);
                    Assert.Equal(commandParameter, o);
                    Assert.False(converter.HasConverted);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, commandParameter));
        }

        [Fact]
        public void Command_Converter()
        {
            const string item = "ItemProperty";
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                EventArgsConverter = new ItemTappedEventArgsConverter(false),
                Command = new Command(o =>
                {
                    Assert.NotNull(o);
                    Assert.Equal(item, o);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, item));
        }

        [Fact]
        public void Command_ConverterWithConverterParameter()
        {
            const string item = "ItemProperty";
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                EventArgsConverter = new ItemTappedEventArgsConverter(true),
                EventArgsConverterParameter = item,
                Command = new Command(o =>
                {
                    Assert.NotNull(o);
                    Assert.Equal(item, o);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, null));
        }

        [Fact]
        public void Command_ExecuteWithParameter()
        {
            const string item = "ItemProperty";
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                CommandParameter = item,
                Command = new Command(o =>
                {
                    Assert.NotNull(o);
                    Assert.Equal(item, o);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, null));
        }

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
                    Assert.Equal(item, o);
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