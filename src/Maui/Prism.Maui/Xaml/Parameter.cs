
namespace Prism.Xaml;

/// <summary>
/// Represents a parameter used in XAML markup.
/// </summary>
public class Parameter : BindableObject
{
    /// <summary>
    /// Gets or sets the key associated with the parameter.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Identifies the <see cref="Value"/> bindable property.
    /// </summary>
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(object), typeof(Parameter));

    /// <summary>
    /// Gets or sets the value of the parameter.
    /// </summary>
    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}
