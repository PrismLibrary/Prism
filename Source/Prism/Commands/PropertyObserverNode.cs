using System;
using System.ComponentModel;
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
        private INotifyPropertyChanged _inpcObject;

        public string PropertyName { get; }
        public PropertyObserverNode Next { get; set; }

        public PropertyObserverNode(string propertyName, Action action)
        {
            PropertyName = propertyName;
            _action = () =>
            {
                action?.Invoke();
                if (Next == null) return;
                Next.UnsubscribeListener();
                GenerateNextNode();
            };
        }

        public void SubscribeListenerFor(INotifyPropertyChanged inpcObject)
        {
            _inpcObject = inpcObject;
            _inpcObject.PropertyChanged += OnPropertyChanged;

            if (Next != null) GenerateNextNode();
        }

        private void GenerateNextNode()
        {
            var propertyInfo = _inpcObject.GetType().GetRuntimeProperty(PropertyName); // TODO: To cache, if the step consume significant performance. Note: The type of _inpcObject may become its base type or derived type.
            var nextProperty = propertyInfo.GetValue(_inpcObject);
            if (nextProperty == null) return;
            if (!(nextProperty is INotifyPropertyChanged nextInpcObject))
                throw new InvalidOperationException("Trying to subscribe PropertyChanged listener in object that " +
                                                    $"owns '{Next.PropertyName}' property, but the object does not implements INotifyPropertyChanged.");

            Next.SubscribeListenerFor(nextInpcObject);
        }

        private void UnsubscribeListener()
        {
            if (_inpcObject != null)
                _inpcObject.PropertyChanged -= OnPropertyChanged;

            Next?.UnsubscribeListener();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Invoke action when e.PropertyName == null in order to satisfy:
            //  - DelegateCommandFixture.GenericDelegateCommandObservingPropertyShouldRaiseOnEmptyPropertyName
            //  - DelegateCommandFixture.NonGenericDelegateCommandObservingPropertyShouldRaiseOnEmptyPropertyName
            if (e?.PropertyName == PropertyName || e?.PropertyName == null)
            {
                _action?.Invoke();
            }
        }
    }
}
