using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Prism.Commands
{
    /// <summary>
    /// Represents each node of nested properties expression and takes care of 
    /// subscribing/unsubscribing INotifyPropertyChanged.PropertyChanged listeners on it.
    /// </summary>
    internal class PropertyObserverNode
    {
        private readonly Action _action;
        private readonly LinkedList<Expression> _fragments;
        private string _propertyName;
        private PropertyObserver _propertyObserver;

        private PropertyObserverNode(Expression propertyExpression, Action action, PropertyObserver propertyObserver)
        {
            _fragments = ExtractFragments(propertyExpression as MemberExpression);
            _action = action;
            _propertyObserver = propertyObserver;
            SubscribeListeners(_fragments);
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

        private void SubscribeListeners(LinkedList<Expression> fragments)
        {
            var propertyFragment = fragments.First;
            var propertyOwnerFragment = propertyFragment.Next;
            _propertyName = (propertyFragment.Value as MemberExpression).Member.Name; 
            var propertyOwnerObject = EvalExpression(propertyOwnerFragment.Value);

            if (propertyOwnerObject != null)
            {
                var inpcObject = propertyOwnerObject as INotifyPropertyChanged;

                if (inpcObject == null)
                {
                    throw new InvalidOperationException("Trying to subscribe PropertyChanged listener in object that " +
                        $"owns '{_propertyName}' property, but the object does not implements INotifyPropertyChanged.");
                }

                _propertyObserver.RegisterListener(inpcObject, OnPropertyChanged);
            }

            if (propertyOwnerFragment.Value is MemberExpression)
            {
                Observes(propertyOwnerFragment.Value, _propertyObserver.Restart, _propertyObserver);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Invoke action when e.PropertyName == null in order to satisfy:
            //  - DelegateCommandFixture.GenericDelegateCommandObservingPropertyShouldRaiseOnEmptyPropertyName
            //  - DelegateCommandFixture.NonGenericDelegateCommandObservingPropertyShouldRaiseOnEmptyPropertyName
            if (e.PropertyName == _propertyName || e.PropertyName == null)
            {
                _action?.Invoke(); 
            }
        }

        private object EvalExpression(Expression expression)
        {
            object result; 

            if (expression is MemberExpression)
            {
                var memberExpression = expression as MemberExpression;
                var objectMember = Expression.Convert(memberExpression, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();

                //TODO: Remove try ... catch by checking if object associated to "getter" is null
                try
                {
                    result = getter.Invoke();
                }
                // When object associated to "getter" is null Android and Windows threw NullReferenceException,
                // iOS threw TargetException. So Im catching Exception assuming the objects is null. :/
                catch (Exception) 
                {
                    result = null;
                }
            }
            else if(expression is ConstantExpression)
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

        private LinkedList<Expression> ExtractFragments(MemberExpression memberExpression)
        {
            if(memberExpression == null)
            {
                throw new ArgumentNullException(nameof(memberExpression));
            }

            var result = new LinkedList<Expression>();

            while (memberExpression != null)
            {
                result.AddLast(memberExpression);
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            var root = (result.Last() as MemberExpression)?.Expression as ConstantExpression;

            if(root == null)
            {
                throw new InvalidOperationException(
                    "Could not get root element. This operation supports only nested properties lambda like this: () => Prop.NestedProp.NestedProp.PropertyToObserve. Is it called on the correct form?");
            }

            result.AddLast(root);

            return result;
        }
    }
}
