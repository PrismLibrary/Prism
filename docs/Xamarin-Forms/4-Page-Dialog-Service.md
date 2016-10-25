#Using the Page Dialog Service

Displaying an alert or asking a user to make a choice is a common UI task. Xamarin.Forms has two methods on the Page class for interacting with the user via a pop-up: DisplayAlert and DisplayActionSheet.  Prism provides a single **IPageDialogService** that abstracts away the Xamarin.Forms Page object dependency required for these actions and keeps your ViewModels clean and testable.  Simply request this service via the constructor of your ViewModel, and call either the DisplayAlert, or DisplayActionSheet to invoke the desired notification.

```
public MainPageViewModel(IPageDialogService dialogService)
{
    _dialogService = dialogService;
}
```

## DisplayAlertAsync
The **DisplayAlertAsync** method shows a modal pop-up to alert the user or ask simple questions of them. To display these alerts with Prism's **IPageDialogService**, use the **DisplayAlertAsync** method. The following line of code shows a simple message to the user:

```
_dialogService.DisplayAlertAsync("Alert", "You have been alerted", "OK");
```

![Alert dialog on the 3 major platforms](images/pagedialogservice_01.png)

This example does not collect information from the user. The alert displays modally and once dismissed the user continues interacting with the application. DisplayAlertAsync can also be used to capture a user's response by presenting two buttons and returning a boolean.

To get a response from an alert, supply text for both buttons and await the method. After the user selects one of the options the answer will be returned to your code. Note the async and await keywords in the sample code below:

```
var alertButton2 = new Button { Text = "DisplayAlert Yes/No" }; // triggers alert
alertButton2.Clicked += async (sender, e) => 
{
    var answer = await DisplayAlertAsync ("Question?", "Would you like to play a game", "Yes", "No");
    Debug.WriteLine("Answer: " + answer); // writes true or false to the console
};
```
![Question dialog on the 3 major platforms](images/pagedialogservice_02.png)

## DisplayActionSheetAsync

The UIActionSheet is a common UI element in iOS. The **IPageDialogService.DisplayActionSheetAsync** lets you include this control in cross-platforms apps, rendering native alternatives in Android and Windows Phone.

To display an action sheet, await **DisplayActionSheetAsync** in any ViewModel, passing the message and button labels as strings. The method returns the string label of the button that was clicked by the user. A simple example is shown here:

```
var actionButton1 = new Button { Text = "ActionSheet Simple" };
actionButton1.Clicked += async (sender, e) => 
{
    var action = await DisplayActionSheetAsync ("ActionSheet: Send to?", "Cancel", null, "Email", "Twitter", "Facebook");
    Debug.WriteLine("Action: " + action); // writes the selected button label to the console
};
```
![Action dialog on the 3 major platforms](images/pagedialogservice_03.png)

The destroy button is rendered differently than the others, and can be left null or specified as the third string parameter. This example uses the destroy button:

```
var actionButton2 = new Button { Text = "ActionSheet" };
actionButton2.Clicked += async (sender, e) => 
{
    var action = await DisplayActionSheetAsync ("ActionSheet: Save Photo?", "Cancel", "Delete", "Photo Roll", "Email");
    Debug.WriteLine("Action: " + action); // writes the selected button label to the console
};
```

![Another action dialog on the 3 major platforms](images/pagedialogservice_04.png)

Additionally, Prism provides another option which accepts an array of **IActionSheetButton** that allow you to specificy the title of the buttons, as well as the **DelegateCommand** that should be executed when the option is selected by the user.  This eliminates the need to capture a string result, perform a logical check against the result, and then execute a method or logic in response.

To create an IActionSheetButton, use one of the three factory methods off of the **ActionSheetButton** class.
- ActionSheetButton.CreateButton
- ActionSheetButton.CreateCancelButton
- ActionSheetButton.CreateDestroyButton


```
IActionSheetButton selectAAction = ActionSheetButton.CreateButton("Select A", new DelegateCommand(() => { Debug.WriteLine("Select A"); }));
IActionSheetButton cancelAction = ActionSheetButton.CreateCancelButton("Cancel", new DelegateCommand(() => { Debug.WriteLine("Cancel"); }));
IActionSheetButton destroyAction = ActionSheetButton.CreateDestroyButton("Destroy", new DelegateCommand(() => { Debug.WriteLine("Destroy"); }));

void ShowActionSheet()
{
    _pageDialogService.DisplayActionSheetAsync("My Action Sheet", selectAAction, cancelAction, destroyAction);
}
```

_Note: The order in which you pass in the IActionSheetButton parameters does not matter. The IPageDialogService will make sure the parameters are handled properly for you._
