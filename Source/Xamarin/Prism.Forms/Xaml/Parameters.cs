using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Prism.Xaml
{
    public class Parameters : BindableObject, IList<Parameter>
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IDictionary), typeof(Parameters), null, propertyChanged: OnItemsSourceChanged);

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Parameters parameters)
            {
                parameters.Clear();
                foreach (KeyValuePair<string, object> item in parameters.ItemsSource)
                {
                    parameters.Add(new Parameter { Key = item.Key, Value = item.Value });
                }
            }
        }

        public static readonly BindableProperty ParentProperty = BindableProperty.Create(nameof(Parent),
            typeof(BindableObject),
            typeof(Parameters),
            default(BindableObject), propertyChanged: OnParentPropertyChanged);

        private readonly IList<Parameter> _list = new List<Parameter>();

        public IDictionary ItemsSource
        {
            get => (IDictionary)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        ///     Navigation Parameter Parent. This is a bindable property.
        /// </summary>
        /// <remarks>This is used to set the BindingContext of the CommandParameters to the BindingContext of it's parent.</remarks>
        public BindableObject Parent
        {
            get => (BindableObject)GetValue(ParentProperty);
            set => SetValue(ParentProperty, value);
        }

        public void Add(Parameter item) => _list.Add(item);

        public void Clear() => _list.Clear();

        public bool Contains(Parameter item) => _list.Contains(item);

        public void CopyTo(Parameter[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public int Count => _list.Count;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<Parameter> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(Parameter item) => _list.IndexOf(item);

        public void Insert(int index, Parameter item) => _list.Insert(index, item);

        public bool IsReadOnly => _list.IsReadOnly;

        public Parameter this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public bool Remove(Parameter item) => _list.Remove(item);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        public T ToParameters<T>(BindableObject parent)
            where T : Common.ParametersBase
        {
            Parent = Parent ?? parent;
            var parameters = (T)Activator.CreateInstance(typeof(T));
            parameters.FromParameters(_list.Select(x => new KeyValuePair<string, object>(x.Key, x.Value)));

            return parameters;
        }

        private static void OnParentPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (oldvalue == newvalue) return;

            var self = (Parameters)bindable;
            if (oldvalue is BindableObject oldParent)
                oldParent.BindingContextChanged -= BindingContextChanged;

            if (newvalue is BindableObject newParent)
                newParent.BindingContextChanged += BindingContextChanged;

            void BindingContextChanged(object parentObject, EventArgs args)
            {
                var parent = (BindableObject)parentObject;
                self.BindingContext = parent?.BindingContext;
            }
        }

        protected override void OnBindingContextChanged()
        {
            foreach (var param in this)
            {
                param.BindingContext = BindingContext;
            }
        }
    }
}
