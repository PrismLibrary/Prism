using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Prism.Services.Dialogs.Xaml
{
    public class DialogParameters : BindableObject, IList<DialogParameter>
    {
        public static readonly BindableProperty ParentProperty = BindableProperty.Create(nameof(Parent),
            typeof(BindableObject),
            typeof(DialogParameters),
            default(BindableObject), propertyChanged: OnParentPropertyChanged);

        private readonly IList<DialogParameter> _list = new List<DialogParameter>();

        /// <summary>
        ///     Navigation Parameter Parent. This is a bindable property.
        /// </summary>
        /// <remarks>This is used to set the BindingContext of the CommandParameters to the BindingContext of it's parent.</remarks>
        public BindableObject Parent
        {
            get => (BindableObject)GetValue(ParentProperty);
            set => SetValue(ParentProperty, value);
        }

        public void Add(DialogParameter item) => _list.Add(item);

        public void Clear() => _list.Clear();

        public bool Contains(DialogParameter item) => _list.Contains(item);

        public void CopyTo(DialogParameter[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public int Count => _list.Count;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<DialogParameter> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(DialogParameter item) => _list.IndexOf(item);

        public void Insert(int index, DialogParameter item) => _list.Insert(index, item);

        public bool IsReadOnly => _list.IsReadOnly;

        public DialogParameter this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public bool Remove(DialogParameter item) => _list.Remove(item);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        public IDialogParameters ToNavigationParameters(BindableObject parent)
        {
            Parent = Parent ?? parent;
            var parameters = new Prism.Services.Dialogs.DialogParameters();
            for (var index = 0; index < _list.Count; index++)
            {
                var parameter = _list[index];
                parameters.Add(parameter.Key, parameter.Value);
            }

            return parameters;
        }

        private static void OnParentPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (oldvalue == newvalue) return;

            var self = (DialogParameters)bindable;
            if (oldvalue is BindableObject oldParent)
                oldParent.BindingContextChanged -= BindingContextChanged;

            if (newvalue is BindableObject newParent)
                newParent.BindingContextChanged += BindingContextChanged;

            void BindingContextChanged(object parentObject, EventArgs args)
            {
                var parent = (BindableObject)parentObject;
                for (var index = 0; index < self._list.Count; index++)
                {
                    var parameter = self._list[index];
                    parameter.BindingContext = parent.BindingContext;
                }
            }
        }
    }
}
