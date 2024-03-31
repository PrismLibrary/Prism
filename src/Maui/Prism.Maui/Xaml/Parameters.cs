using System.Collections;

namespace Prism.Xaml;

/// <summary>
/// Represents a collection of parameters used for navigation.
/// </summary>
public class Parameters : BindableObject, IList<Parameter>
{
    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> bindable property.
    /// </summary>
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

    /// <summary>
    /// Identifies the <see cref="Parent"/> bindable property.
    /// </summary>
    public static readonly BindableProperty ParentProperty = BindableProperty.Create(nameof(Parent),
        typeof(BindableObject),
        typeof(Parameters),
        default(BindableObject), propertyChanged: OnParentPropertyChanged);

    private readonly IList<Parameter> _list = new List<Parameter>();

    /// <summary>
    /// Gets or sets the items source for the parameters.
    /// </summary>
    public IDictionary ItemsSource
    {
        get => (IDictionary)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the parent object for the parameters.
    /// </summary>
    /// <remarks>
    /// This is used to set the BindingContext of the CommandParameters to the BindingContext of its parent.
    /// </remarks>
    public BindableObject Parent
    {
        get => (BindableObject)GetValue(ParentProperty);
        set => SetValue(ParentProperty, value);
    }

    /// <summary>
    /// Adds a parameter to the collection.
    /// </summary>
    /// <param name="item">The parameter to add.</param>
    public void Add(Parameter item) => _list.Add(item);

    /// <summary>
    /// Removes all parameters from the collection.
    /// </summary>
    public void Clear() => _list.Clear();

    /// <summary>
    /// Determines whether the collection contains a specific parameter.
    /// </summary>
    /// <param name="item">The parameter to locate in the collection.</param>
    /// <returns>true if the parameter is found; otherwise, false.</returns>
    public bool Contains(Parameter item) => _list.Contains(item);

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(Parameter[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    /// <summary>
    /// Gets the number of parameters in the collection.
    /// </summary>
    public int Count => _list.Count;

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<Parameter> GetEnumerator() => _list.GetEnumerator();

    /// <summary>
    /// Returns the zero-based index of the first occurrence of a specific parameter in the collection.
    /// </summary>
    /// <param name="item">The parameter to locate in the collection.</param>
    /// <returns>The zero-based index of the first occurrence of item within the entire collection, if found; otherwise, -1.</returns>
    public int IndexOf(Parameter item) => _list.IndexOf(item);

    /// <summary>
    /// Inserts a parameter into the collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which the parameter should be inserted.</param>
    /// <param name="item">The parameter to insert.</param>
    public void Insert(int index, Parameter item) => _list.Insert(index, item);

    /// <inheritdoc/>
    public bool IsReadOnly => _list.IsReadOnly;

    /// <summary>
    /// Gets or sets the parameter at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the parameter to get or set.</param>
    /// <returns>The parameter at the specified index.</returns>
    public Parameter this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    /// <summary>
    /// Removes the first occurrence of a specific parameter from the collection.
    /// </summary>
    /// <param name="item">The parameter to remove.</param>
    /// <returns>true if the parameter is successfully removed; otherwise, false.</returns>
    public bool Remove(Parameter item) => _list.Remove(item);

    /// <summary>
    /// Removes the parameter at the specified index from the collection.
    /// </summary>
    /// <param name="index">The zero-based index of the parameter to remove.</param>
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

    /// <inheritdoc/>
    protected override void OnBindingContextChanged()
    {
        foreach (var param in this)
        {
            param.BindingContext = BindingContext;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
