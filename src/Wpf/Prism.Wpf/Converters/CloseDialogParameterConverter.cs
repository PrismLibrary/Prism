using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Prism.Commands.Parameters;
using Prism.Dialogs;

namespace Prism.Converters;

/// <summary>
/// Converter to prepare parameters for closing dialog
/// </summary>
public class CloseDialogParameterConverter : MarkupExtension, IValueConverter
{
    /// <summary>
    /// The result of the dialog.
    /// </summary>
    public ButtonResult DialogResult { get; set; }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IDialogAware dialogContext)
            throw new InvalidCastException("CloseDialogParameterConverter");
        return new CloseDialogCommandParameter
        {
            ButtonResult = DialogResult,
            DialogContext = dialogContext,
        };
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => null;

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
                => this;
}
