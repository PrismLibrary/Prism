using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Prism.Commands.Parameters;
using Prism.Dialogs;

namespace Prism.Converters;

/// <summary>
/// Show dialog converter parameters
/// </summary>
public class ShowDialogParameterConverter : MarkupExtension, IValueConverter, IMultiValueConverter
{
    /// <summary>
    /// Dialog name
    /// </summary>
    public string DialogName { get; set; }

    /// <summary>
    /// Parameter Key used in the case of binding value not empty
    /// </summary>
    public string Key { get; set; }

    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(DialogName))
            return null;

        var commandParam = new ShowDialogCommandParameter()
        {
            DialogName = DialogName,
        };

        if (values.Length == 0)
            return commandParam;

        commandParam.Parameters = new DialogParameters();
        foreach (var value in values)
        {
            if (value is KeyValuePair<string, object> bindingValue)
            {
                commandParam.Parameters.Add(bindingValue.Key, bindingValue.Value);
            }
        }

        return commandParam;
    }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(DialogName))
            return null;

        var commandParam = new ShowDialogCommandParameter()
        {
            DialogName = DialogName,
        };
        if (value is null || string.IsNullOrEmpty(Key))
            return commandParam;

        commandParam.Parameters = new DialogParameters()
            {
                { Key, value }
            };

        return commandParam;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => null;

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => null;

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
