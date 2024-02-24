using System.Globalization;

namespace Prism.Dialogs;

internal class RelativeContentSizeConverter : IValueConverter
{
    private double relativeSize;
    public double RelativeSize
    {
        get => relativeSize;
        set
        {
            if (value == 0)
            {
                relativeSize = -1;
            }
            else if (value > 1)
            {
                relativeSize = value / 100;
            }
            else
            {
                relativeSize = value;
            }
        }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var pageSize = double.Parse(value.ToString());
        if (pageSize < 0)
            return pageSize;

        return RelativeSize * pageSize;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
