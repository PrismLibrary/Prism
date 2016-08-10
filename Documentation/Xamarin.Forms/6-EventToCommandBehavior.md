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

## Thanks
Thanks to https://anthonysimmon.com/eventtocommand-in-xamarin-forms-apps/ from which a took the implementation and modified it slightly