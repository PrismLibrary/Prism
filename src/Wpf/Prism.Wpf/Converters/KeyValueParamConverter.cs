using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Prism.Converters;

/// <summary>
/// Key value parameter converter used in the case of multi parameters using multi binding
/// </summary>
public class KeyValueParamConverter : MarkupExtension, IValueConverter
{
    /// <summary>
    /// Parameter Key
    /// </summary>
    public string Key { get; set; }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new KeyValuePair<string, object>(Key, value);
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => null;

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
