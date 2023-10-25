using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Prism.Commands.Parameters;
using Prism.Navigation;

namespace Prism.Converters;

/// <summary>
/// The global parameters used for handling region navigation
/// </summary>
public class ParamsNavigationConverter : MarkupExtension, IMultiValueConverter, IValueConverter
{
    /// <summary>
    /// Parameter Key used in the case of binding value not empty
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Region name
    /// </summary>
    public string RegionName { get; set; }

    /// <summary>
    /// Target view
    /// </summary>
    public string TargetView { get; set; }

    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(RegionName) || string.IsNullOrEmpty(TargetView))
            return null;

        var commandParam = new NavigateToViewCommandParameter
        {
            RegionName = RegionName,
            TargetView = TargetView,
        };

        if (values.Length == 0)
            return commandParam;

        commandParam.Parameters = new NavigationParameters();
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
        if (string.IsNullOrEmpty(RegionName) || string.IsNullOrEmpty(TargetView))
            return null;

        var commandParam = new NavigateToViewCommandParameter
        {
            RegionName = RegionName,
            TargetView = TargetView,
        };
        if (value is null || string.IsNullOrEmpty(Key))
            return commandParam;

        commandParam.Parameters = new NavigationParameters
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
