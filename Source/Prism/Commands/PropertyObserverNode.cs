using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Prism.Commands
{
    /// <summary>
    /// Represents each node of nested properties expression and takes care of 
    /// subscribing/unsubscribing INotifyPropertyChanged.PropertyChanged listeners on it.
    /// </summary>
    internal class PropertyObserverNode
    {
        private readonly Action _action;
        private string _propertyName;
        private PropertyObserver _propertyObserver;

        private PropertyObserverNode(Expression propertyExpression, Action action, PropertyObserver propertyObserver)
        {
            _action = action;
            _propertyObserver = propertyObserver;
            SubscribeListeners(propertyExpression as MemberExpression);
        }

        /// <summary>
        /// Initiate PropertyObserverNode for a given property expression.
        /// </summary>
        /// <param name="propertyExpression">Expression representing property to be observed.</param>
        /// <param name="action">Action to be invoked when PropertyChanged event occours.</param>
        /// <param name="propertyObserver">PropertyObserver instance which owns registered event handlers.</param>
        internal static void Observes(Expression propertyExpression, Action action, PropertyObserver propertyObserver)
        {
            new PropertyObserverNode(propertyExpression, action, propertyObserver);
        }

        private void SubscribeListeners(MemberExpression propertyExpression)
        {

            // Represents the object that property in propertyExpression belongs to. 
            // It may be the ModelView object in case of property in propertyExpression belongs to it.
            Expression propertyOwnerExpression = propertyExpression.Expression as Expression;

            _propertyName = propertyExpression.Member.Name;
            object propertyOwnerObject = GetPropertyValue(propertyOwnerExpression);

            if (propertyOwnerObject != null)
            {
                INotifyPropertyChanged inpcObject = propertyOwnerObject as INotifyPropertyChanged;

                if (inpcObject == null)
                {
                    throw new InvalidOperationException("Trying to subscribe PropertyChanged listener in object that " +
                        $"owns '{_propertyName}' property, but the object does not implements INotifyPropertyChanged.");
                }

                _propertyObserver.RegisterListener(inpcObject, OnPropertyChanged);
            }

            if (propertyOwnerExpression is MemberExpression)
            {
                Observes(propertyOwnerExpression, _propertyObserver.Restart, _propertyObserver);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Invoke action when e.PropertyName == null in order to satisfy:
            //  - DelegateCommandFixture.GenericDelegateCommandObservingPropertyShouldRaiseOnEmptyPropertyName
            //  - DelegateCommandFixture.NonGenericDelegateCommandObservingPropertyShouldRaiseOnEmptyPropertyName
            if (e?.PropertyName == _propertyName || e?.PropertyName == null)
            {
                _action?.Invoke();
            }
        }

        private static object GetPropertyValue(Expression expression)
        {
            object result;

            if (expression is MemberExpression)
            {
                MemberExpression memberExpression = expression as MemberExpression;
                string propertyName = memberExpression.Member.Name;
                object propertyOwnerObject = GetPropertyValue(memberExpression.Expression);

                if (propertyOwnerObject == null)
                {
                    result = null;
                }
                else
                {
                    PropertyInfo propertyInfo = propertyOwnerObject.GetType().GetRuntimeProperty(propertyName);
                    result = propertyInfo?.GetValue(propertyOwnerObject);
                }
            }
            else if (expression is ConstantExpression)
            {
                result = (expression as ConstantExpression)?.Value;
            }
            else
            {
                throw new NotSupportedException("Operation not supported for the given expression type. " +
                    "Only MemberExpression and ConstanteExpression are currently supported.");
            }

            return result;
        }
    }
}
