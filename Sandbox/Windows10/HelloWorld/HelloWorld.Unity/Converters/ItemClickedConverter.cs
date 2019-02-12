using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HelloWorld.Converters
{
    public class ItemClickedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ItemClickEventArgs args)
                return args.ClickedItem;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
