using System;
using System.Collections;
using System.Collections.Generic;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Navigation.Xaml.Prism
{
    public class XamlNavigationParameters : BindableObject, IList<XamlNavigationParameter>
    {
        public static readonly BindableProperty ParentProperty = BindableProperty.Create(nameof(Parent),
            typeof(BindableObject),
            typeof(XamlNavigationParameters),
            default(BindableObject), propertyChanged: OnParentPropertyChanged);

        private static void OnParentPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if(oldvalue == newvalue) return;
            
            var self = (XamlNavigationParameters) bindable;
            if (oldvalue is BindableObject oldParent)
                oldParent.BindingContextChanged -= BindingContextChanged;

            if (newvalue is BindableObject newParent)
                newParent.BindingContextChanged += BindingContextChanged;

            void BindingContextChanged(object parentObject, EventArgs args)
            {
                var parent = (BindableObject) parentObject;
                for (var index = 0; index < self._list.Count; index++)
                {
                    var parameter = self._list[index];
                    parameter.BindingContext = parent.BindingContext;
                }
            }
        }

        /// <summary>
        /// Parent summary. This is a bindable property.
        /// </summary>
        public BindableObject Parent
        {
            get => (BindableObject) GetValue(ParentProperty);
            set => SetValue(ParentProperty, value);
        }

        private readonly IList<XamlNavigationParameter> _list = new List<XamlNavigationParameter>();

        public void Add(XamlNavigationParameter item) => _list.Add(item);

        public void Clear() => _list.Clear();

        public bool Contains(XamlNavigationParameter item) => _list.Contains(item);

        public void CopyTo(XamlNavigationParameter[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public int Count => _list.Count;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<XamlNavigationParameter> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(XamlNavigationParameter item) => _list.IndexOf(item);

        public void Insert(int index, XamlNavigationParameter item) => _list.Insert(index, item);

        public bool IsReadOnly => _list.IsReadOnly;

        public XamlNavigationParameter this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public bool Remove(XamlNavigationParameter item) => _list.Remove(item);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        public NavigationParameters ToNavigationParameters(BindableObject parent)
        {
            Parent = Parent ?? parent;
            var parameters = new NavigationParameters();
            for (var index = 0; index < _list.Count; index++)
            {
                var parameter = _list[index];
                parameters.Add(parameter.Key, parameter.Value);
            }

            return parameters;
        }
    }
}