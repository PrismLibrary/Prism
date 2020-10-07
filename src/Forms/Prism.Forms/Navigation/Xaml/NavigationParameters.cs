using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    // TODO: Remove in 7.4+
    [Obsolete("Use the new Parameters class")]
    public class NavigationParameters : Prism.Xaml.Parameters
    {
        //public static readonly BindableProperty ItemsSourceProperty =
        //    BindableProperty.Create(nameof(ItemsSource), typeof(IDictionary), typeof(NavigationParameters), null, propertyChanged: OnItemsSourceChanged);

        //private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    if(bindable is NavigationParameters parameters)
        //    {
        //        parameters.Clear();
        //        foreach(KeyValuePair<string, object> item in parameters.ItemsSource)
        //        {
        //            parameters.Add(new NavigationParameter { Key = item.Key, Value = item.Value });
        //        }
        //    }
        //}

        //public static readonly BindableProperty ParentProperty = BindableProperty.Create(nameof(Parent),
        //    typeof(BindableObject),
        //    typeof(NavigationParameters),
        //    default(BindableObject), propertyChanged: OnParentPropertyChanged);

        //private readonly IList<NavigationParameter> _list = new List<NavigationParameter>();

        //public IDictionary ItemsSource
        //{
        //    get => (IDictionary)GetValue(ItemsSourceProperty);
        //    set => SetValue(ItemsSourceProperty, value);
        //}

        ///// <summary>
        /////     Navigation Parameter Parent. This is a bindable property.
        ///// </summary>
        ///// <remarks>This is used to set the BindingContext of the CommandParameters to the BindingContext of it's parent.</remarks>
        //public BindableObject Parent
        //{
        //    get => (BindableObject) GetValue(ParentProperty);
        //    set => SetValue(ParentProperty, value);
        //}

        //public void Add(NavigationParameter item) => _list.Add(item);

        //public void Clear() => _list.Clear();

        //public bool Contains(NavigationParameter item) => _list.Contains(item);

        //public void CopyTo(NavigationParameter[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        //public int Count => _list.Count;

        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //public IEnumerator<NavigationParameter> GetEnumerator() => _list.GetEnumerator();

        //public int IndexOf(NavigationParameter item) => _list.IndexOf(item);

        //public void Insert(int index, NavigationParameter item) => _list.Insert(index, item);

        //public bool IsReadOnly => _list.IsReadOnly;

        //public NavigationParameter this[int index]
        //{
        //    get => _list[index];
        //    set => _list[index] = value;
        //}

        //public bool Remove(NavigationParameter item) => _list.Remove(item);

        //public void RemoveAt(int index) => _list.RemoveAt(index);

        //public INavigationParameters ToNavigationParameters(BindableObject parent)
        //{
        //    Parent = Parent ?? parent;
        //    var parameters = new Prism.Navigation.NavigationParameters();
        //    for (var index = 0; index < _list.Count; index++)
        //    {
        //        var parameter = _list[index];
        //        parameters.Add(parameter.Key, parameter.Value);
        //    }

        //    return parameters;
        //}

        //private static void OnParentPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        //{
        //    if (oldvalue == newvalue) return;

        //    var self = (NavigationParameters) bindable;
        //    if (oldvalue is BindableObject oldParent)
        //        oldParent.BindingContextChanged -= BindingContextChanged;

        //    if (newvalue is BindableObject newParent)
        //        newParent.BindingContextChanged += BindingContextChanged;

        //    void BindingContextChanged(object parentObject, EventArgs args)
        //    {
        //        var parent = (BindableObject) parentObject;
        //        self.BindingContext = parent?.BindingContext;
        //    }
        //}

        //protected override void OnBindingContextChanged()
        //{
        //    foreach(var param in this)
        //    {
        //        param.BindingContext = BindingContext;
        //    }
        //}
    }
}
