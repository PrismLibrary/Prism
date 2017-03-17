using System;
using System.Globalization;
using Prism.Behaviors;
using Prism.Commands;
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
            var executedCommand = false;
            var converter = new ItemTappedEventArgsConverter(false);
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                EventArgsConverter = converter,
                CommandParameter = commandParameter,
                Command = new DelegateCommand<string>(o =>
                {
                    executedCommand = true;
                    Assert.NotNull(o);
                    Assert.Equal(commandParameter, o);
                    Assert.False(converter.HasConverted);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, commandParameter));
            Assert.True(executedCommand);
        }

        [Fact]
        public void Command_Converter()
        {
            const string item = "ItemProperty";
            var executedCommand = false;
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                EventArgsConverter = new ItemTappedEventArgsConverter(false),
                Command = new DelegateCommand<string>(o =>
                {
                    executedCommand = true;
                    Assert.NotNull(o);
                    Assert.Equal(item, o);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, item));
            Assert.True(executedCommand);
        }

        [Fact]
        public void Command_ConverterWithConverterParameter()
        {
            const string item = "ItemProperty";
            var executedCommand = false;
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                EventArgsConverter = new ItemTappedEventArgsConverter(true),
                EventArgsConverterParameter = item,
                Command = new DelegateCommand<string>(o =>
                {
                    executedCommand = true;
                    Assert.NotNull(o);
                    Assert.Equal(item, o);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, null));
            Assert.True(executedCommand);
        }

        [Fact]
        public void Command_ExecuteWithParameter()
        {
            const string item = "ItemProperty";
            var executedCommand = false;
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                CommandParameter = item,
                Command = new DelegateCommand<string>(o =>
                {
                    executedCommand = true;
                    Assert.NotNull(o);
                    Assert.Equal(item, o);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, null));
            Assert.True(executedCommand);
        }

        [Fact]
        public void Command_EventArgsParameterPath()
        {
            const string item = "ItemProperty";
            var executedCommand = false;
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                EventArgsParameterPath = "Item",
                Command = new DelegateCommand<string>(o =>
                {
                    executedCommand = true;
                    Assert.NotNull(o);
                    Assert.Equal(item, o);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, item));
            Assert.True(executedCommand);
        }

        [Fact]
        public void Command_EventArgsParameterPath_Nested()
        {
            dynamic item = new
            {
                AProperty = "Value"
            };
            var executedCommand = false;
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                EventArgsParameterPath = "Item.AProperty",
                Command = new DelegateCommand<object>(o =>
                {
                    executedCommand = true;
                    Assert.NotNull(o);
                    Assert.Equal("Value", o);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, new ItemTappedEventArgs(listView, item));
            Assert.True(executedCommand);
        }

        [Fact]
        public void Command_CanExecute()
        {
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                Command = new DelegateCommand(() => Assert.True(false), () => false)
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, null);
        }

        [Fact]
        public void Command_CanExecuteWithParameterShouldExecute()
        {
            var shouldExeute = bool.TrueString;
            var executedCommand = false;
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                CommandParameter = shouldExeute,
                Command = new DelegateCommand<string>(o =>
                {
                    executedCommand = true;
                    Assert.True(true);
                }, o => o.Equals(bool.TrueString))
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, null);
            Assert.True(executedCommand);
        }

        [Fact]
        public void Command_CanExecuteWithParameterShouldNotExeute()
        {
            var shouldExeute = bool.FalseString;
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                CommandParameter = shouldExeute,
                Command = new DelegateCommand<string>(o => Assert.True(false), o => o.Equals(bool.TrueString))
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, null);
        }

        [Fact]
        public void Command_Execute()
        {
            var executedCommand = false;
            var behavior = new EventToCommandBehaviorMock
            {
                EventName = "ItemTapped",
                Command = new DelegateCommand(() =>
                {
                    executedCommand = true;
                    Assert.True(true);
                })
            };
            var listView = new ListView();
            listView.Behaviors.Add(behavior);
            behavior.RaiseEvent(listView, null);
            Assert.True(executedCommand);
        }

        [Fact]
        public void EventName_InvalidEventShouldThrow()
        {
            var behavior = new EventToCommandBehavior
            {
                EventName = "OnItemTapped"
            };
            var listView = new ListView();
            Assert.Throws<ArgumentException>(() => listView.Behaviors.Add(behavior));
        }

        [Fact]
        public void EventName_Valid()
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