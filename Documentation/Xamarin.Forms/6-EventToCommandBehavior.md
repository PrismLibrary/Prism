# Using the EventToCommandBehavior

The `EventToCommandBehavior` class provide a convenient way to, in XAML, "bind" events to `ICommand` according to MVVM paradigm to avoid code behind.

## Usage

The `EventToCommandBehavior` expose the following properties
* **EventName** The name of the event to listen to. For example _ItemTapped_
* **Command** The `ICommand` that will be executed when the event is raised
* **CommandParameter** The parameter that will be sent to the `ICommand.Execute` method
* **EventArgsConverter** Instance of `IValueConverter` that allows operating on the `EventArgs` type for the *EventName*
* **EventArgsConverterParameter** The parameter that will be sent as the _parameter_ argument to `IValueConverter.Convert` method

## Example
````c#
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" 
	xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
	x:Class="MyNamespace.ContentPage"
	x:b="clr-namespace:Prism.Behaviors;assembly=Prism.Forms">
    <ListView>
		<ListView.Behaviors>
			<b:EventToCommandBehavior EventName="Tapped" Command={Binding ItemTappedCommand} />
		</ListView.Behaviors>
	</ListView>
</ContentPage>
````

Using the **EventArgsConverter** to retrieve the `ItemTappedEventArgs.Item` property.

````c#
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Prism.Converters
{
    public class ItemTappedEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var itemTappedEventArgs = value as ItemTappedEventArgs;
            if (itemTappedEventArgs == null)
            {
				throw new ArgumentException("Expected value to be of type ItemTappedEventArgs", nameof(value));
			}
            return itemTappedEventArgs.Item;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
````

The XAML need a reference to the converter and the converter resource need to be defined
````c#
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" 
	xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
	x:Class="MyNamespace.ContentPage"
	x:b="clr-namespace:Prism.Behaviors;assembly=Prism.Forms"
	x:c="clr-namespace:Prism.Converters;assembly=Prism.Forms">
	<Resources>
		<ResourceDictionary>
			<c:ItemTappedEventArgsConverter x:Name="itemTappedEventArgsConverter" />
		</ResourceDictionary>
	</Resources>
    <ListView>
		<ListView.Behaviors>
			<b:EventToCommandBehavior EventName="Tapped" Command={Binding ItemTappedCommand}
									  EventArgsConverter="{StaticResource itemTappedEventArgsConverter}" />
		</ListView.Behaviors>
	</ListView>
</ContentPage>
````

## Thanks
Thanks to https://anthonysimmon.com/eventtocommand-in-xamarin-forms-apps/ from which a took the implementation and modified it slightly