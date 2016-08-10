using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    /// <summary>
    /// Behavior class that enable using <see cref="ICommand"/> and <see cref="IValueConverter"/> to react
    /// on events raised by <see cref="View"/> element.
    /// </summary>
    /// <example>
    /// &lt;ListView&gt;
    ///     &lt;ListView.Behaviors&gt;
    ///         &lt;behaviors:EventToCommandBehavior EventName="Tapped" Command={Binding ItemTappedCommand} /&gt;
    ///     &lt;/ListView.Behaviors&gt;
    /// &lt;/ListView&gt;
    /// </example>
    // This is a modified version of https://anthonysimmon.com/eventtocommand-in-xamarin-forms-apps/
    public class EventToCommandBehavior : BehaviorsBase<View>
    {
        public static readonly BindableProperty EventNameProperty =
            BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventToCommandBehavior));

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior));

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(EventToCommandBehavior));

        public static readonly BindableProperty EventArgsConverterProperty =
            BindableProperty.Create(nameof(EventArgsConverter), typeof(IValueConverter), typeof(EventToCommandBehavior));

        public static readonly BindableProperty EventArgsConverterParameterProperty =
            BindableProperty.Create(nameof(EventArgsConverterParameter), typeof(object), typeof(EventToCommandBehavior));


        private EventInfo _eventInfo;
        private Delegate _handler;

        /// <summary>
        /// Name of the event that will be forwared to <see cref="Command"/>
        /// </summary>
        /// <remarks>
        /// An event that is invalid for the attached <see cref="View"/> will result in <see cref="ArgumentException"/> thrown.
        /// </remarks>
        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        /// <summary>
        /// The command to execute
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Argument sent to <see cref="ICommand.Execute"/>
        /// </summary>
        /// <para>
        /// If <see cref="EventArgsConverter"/> and <see cref="EventArgsConverterParameter"/> is set then the result of the conversion
        /// will be sent.
        /// </para>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Instance of <see cref="IValueConverter"/> to convert the <see cref="EventArgs"/> for <see cref="EventName"/>
        /// </summary>
        public IValueConverter EventArgsConverter
        {
            get { return (IValueConverter)GetValue(EventArgsConverterProperty); }
            set { SetValue(EventArgsConverterProperty, value); }
        }

        /// <summary>
        /// Argument passed as parameter to <see cref="IValueConverter.Convert"/>
        /// </summary>
        public object EventArgsConverterParameter
        {
            get { return GetValue(EventArgsConverterParameterProperty); }
            set { SetValue(EventArgsConverterParameterProperty, value); }
        }

        protected override void OnAttachedTo(View view)
        {
            base.OnAttachedTo(view);

            _eventInfo = AssociatedObject
                .GetType()
                .GetRuntimeEvents()
                .FirstOrDefault(e => e.Name == EventName);
            if (_eventInfo == null)
            {
                throw new ArgumentException(
                    $"No matching event '{EventName}' on attached type '{view.GetType().Name}'");
            }

            AddEventHandler(_eventInfo, AssociatedObject, OnFired);
        }

        protected override void OnDetachingFrom(View view)
        {
            if (_handler != null)
            {
                _eventInfo.RemoveEventHandler(AssociatedObject, _handler);
            }
            _handler = null;
            _eventInfo = null;
            base.OnDetachingFrom(view);
        }

        void AddEventHandler(EventInfo eventInfo, object item, Action<object, EventArgs> action)
        {
            var eventParameters = eventInfo.EventHandlerType
                .GetRuntimeMethods().First(m => m.Name == "Invoke")
                .GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType))
                .ToArray();

            var actionInvoke = action.GetType()
                .GetRuntimeMethods().First(m => m.Name == "Invoke");

            _handler = Expression.Lambda(
                eventInfo.EventHandlerType,
                Expression.Call(Expression.Constant(action), actionInvoke, eventParameters[0], eventParameters[1]),
                eventParameters)
                .Compile();

            eventInfo.AddEventHandler(item, _handler);
        }

        protected virtual void OnFired(object sender, EventArgs eventArgs)
        {
            if (Command == null)
            {
                return;
            }

            var parameter = CommandParameter;

            if (eventArgs != null && eventArgs != EventArgs.Empty)
            {
                if (EventArgsConverter != null)
                {
                    parameter = EventArgsConverter.Convert(eventArgs, typeof(object), EventArgsConverterParameter,
                        CultureInfo.CurrentUICulture);
                }
            }

            if (Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
        }
    }
}