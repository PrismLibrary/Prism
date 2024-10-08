using System.Globalization;
using Prism.DryIoc.Maui.Tests.Mocks.Events;
using Prism.Events;

namespace Prism.DryIoc.Maui.Tests.Mocks.Converters;

internal class MockValueConverter : IValueConverter
{
    private IEventAggregator _eventAggreator { get; }

    public MockValueConverter(IEventAggregator eventAggreator)
    {
        _eventAggreator = eventAggreator;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        _eventAggreator.GetEvent<TestActionEvent>().Publish("Convert");
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        _eventAggreator.GetEvent<TestActionEvent>().Publish("ConvertBack");
        return value;
    }
}
