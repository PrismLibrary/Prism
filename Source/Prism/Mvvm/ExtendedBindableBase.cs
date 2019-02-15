using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Prism.Attributes;

namespace Prism.Mvvm
{
    public abstract class ExtendedBindableBase : BindableBase
    {
        private readonly Dictionary<string, List<string>> dependencyDictionary;

        protected ExtendedBindableBase()
        {
            //Resolve Property Attributes
            foreach (PropertyInfo dependentPropertyInfo in GetType().GetTypeInfo().DeclaredProperties)
            {
                // Check for NotifyAttribute
                var notifyAttribute = dependentPropertyInfo.GetCustomAttribute<NotifyPropertyAttribute>();
                if (notifyAttribute != null)
                {
                    if (dependencyDictionary == null)
                    {
                        dependencyDictionary = new Dictionary<string, List<string>>();
                    }

                    foreach (string property in notifyAttribute.SourceProperties)
                    {
                        ChangedObjectNotifyPropertyChange(dependentPropertyInfo.Name, property);
                    }
                }

                // Check for DependsOnAttribute
                var dependsOnAttribute = dependentPropertyInfo.GetCustomAttribute<DependsOnPropertyAttribute>();
                if (dependsOnAttribute != null)
                {
                    if (dependencyDictionary == null)
                    {
                        dependencyDictionary = new Dictionary<string, List<string>>();
                    }

                    foreach (string property in dependsOnAttribute.SourceProperties)
                    {
                        ChangedObjectNotifyPropertyChange(property, dependentPropertyInfo.Name);
                    }
                }
            }
        }


        /// <summary>
        ///     Associates OnPropertyChanged event to properties.
        /// </summary>
        /// <param name="propertyName">Main property</param>
        /// <param name="actions">Related properties</param>
        private void ChangedObjectNotifyPropertyChange(string propertyName, params string[] actions)
        {
            if(dependencyDictionary.TryGetValue(propertyName, out List<string> list))
            {
                dependencyDictionary[propertyName] = new List<string>(list.Union(actions));
            }
            else
            {
                dependencyDictionary.Add(propertyName, new List<string>(actions));
            }
        }

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="args">The PropertyChangedEventArgs</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if(dependencyDictionary != null && dependencyDictionary.TryGetValue(args.PropertyName, out List<string> list))
            {
                foreach(var item in list)
                {
                    RaisePropertyChanged(item);
                }
            }
        }

        private readonly Lazy<Dictionary<string, object>> _storage = new Lazy<Dictionary<string, object>>(() => new Dictionary<string, object>());

        private readonly Lazy<Dictionary<object, string>> _collectionDependencies = new Lazy<Dictionary<object, string>>(() => new Dictionary<object, string>());

        /// <summary>
        ///     Sets a value in viewmodel storage and raises property changed if value has changed
        /// </summary>
        /// <param name="propertyName">Name.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="TValueType">The 1st type parameter.</typeparam>
        protected bool SetValue<TValueType>(TValueType value, [CallerMemberName] string propertyName = null)
        {
            SetObjectForKey(propertyName, value);
            RaisePropertyChanged(propertyName);

            return true;
        }


        /// <summary>
        ///     Returns a value from the viewmodel storage
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="property">Name.</param>
        /// <typeparam name="TValueType">The 1st type parameter.</typeparam>
        protected TValueType GetValue<TValueType>([CallerMemberName] string property = null)
        {
            return GetValue(() => default(TValueType), property);
        }

        /// <summary>
        ///     Returns a value from the viewmodel storage
        /// </summary>
        protected TValueType GetValue<TValueType>(Func<TValueType> defaultValueFunc, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException(nameof(propertyName));
            }

            return GetObjectForKey(propertyName, defaultValueFunc());
        }

        /// <summary>
        ///     Sets the object for key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void SetObjectForKey<T>(string key, T value)
        {
            if (_storage == null)
            {
                return;
            }

            if (_storage.Value.ContainsKey(key))
            {
                object existingValue = _storage.Value[key];
                if (existingValue != null && existingValue is INotifyCollectionChanged collectionChanged)
                {
                    collectionChanged.CollectionChanged -= HandleNotifyCollectionChangedEventHandler;
                    if (_collectionDependencies.Value.ContainsKey(collectionChanged))
                    {
                        _collectionDependencies.Value.Remove(collectionChanged);
                    }
                }

                _storage.Value[key] = value;
            }
            else
            {
                _storage.Value.Add(key, value);
            }

            if (value != null && value is INotifyCollectionChanged changed)
            {
                _collectionDependencies.Value.Add(value, key);
                changed.CollectionChanged += HandleNotifyCollectionChangedEventHandler;
            }
        }

        protected T GetObjectForKey<T>(string key, T defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }

            if (_storage == null)
            {
                return defaultValue;
            }

            if (!_storage.Value.ContainsKey(key))
            {
                if (defaultValue == null)
                {
                    return default(T);
                }

                SetObjectForKey(key, defaultValue);
            }

            return (T)_storage.Value[key];
        }

        /// <summary>
        ///     Resolve Property Attributes
        /// </summary>
        /// <summary>
        ///     Handles the notify collection changed event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void HandleNotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is INotifyCollectionChanged && _collectionDependencies.Value.ContainsKey(sender))
            {
                RaisePropertyChanged(_collectionDependencies.Value[sender]);
            }
        }
    }
}