using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Prism.Commands
{
    /// <summary>
    /// Provide a way to observe property changes of INotifyPropertyChanged objects and invokes a 
    /// custom action when the PropertyChanged event is fired.
    /// </summary>
    internal class PropertyObserver
    {
        private readonly Expression _propertyExpression;
        private readonly Action _action;
        private HashSet<Tuple<INotifyPropertyChanged, PropertyChangedEventHandler>> _registeredListeners;

        private PropertyObserver(Expression propertyExpression, Action action)
        {
            _propertyExpression = propertyExpression;
            _action = action;
            _registeredListeners = new HashSet<Tuple<INotifyPropertyChanged, PropertyChangedEventHandler>>();
            Start();
        }

        private void Start()
        {
            PropertyObserverNode.Observes(_propertyExpression, _action, this);
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls a custom action on 
        /// property changed notifications. The given expression must be in this form: "() => Prop.NestedProp.PropToObserve".
        /// </summary>
        /// <param name="propertyExpression">Expression representing property to be observed. Ex.: "() => Prop.NestedProp.PropToObserve".</param>
        /// <param name="action">Action to be invoked when PropertyChanged event occours.</param>
        internal static PropertyObserver Observes<T>(Expression<Func<T>> propertyExpression, Action action)
        {
            return new PropertyObserver(propertyExpression.Body, action);
        }          

        internal void RegisterListener(INotifyPropertyChanged inpcObject, PropertyChangedEventHandler onPropertyChanged)
        {
            var item = new Tuple<INotifyPropertyChanged, PropertyChangedEventHandler>(inpcObject, onPropertyChanged);
            _registeredListeners.Add(item);
            inpcObject.PropertyChanged += onPropertyChanged;
        }

        internal void Restart()
        {
            Stop();
            Start();
            _action?.Invoke();
        }

        /// <summary>
        /// Stop listen by unsubscribing all INotifyPropertyChanged.PropertyChanged listeners.
        /// </summary>
        internal void Stop()
        {
            foreach(Tuple<INotifyPropertyChanged, PropertyChangedEventHandler> listener in _registeredListeners)
            {
                listener.Item1.PropertyChanged -= listener.Item2;
            }

            _registeredListeners.Clear();
        }
    }
}
