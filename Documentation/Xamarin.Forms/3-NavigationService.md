# Using the Navigation Service

Navigating between screens is a UI task that nearly every app requires. Xamarin Forms provides the INavigation interface for this task. However, this interface has a few limitations that Prism.Forms Navigation Service tries to address. The Navigation Service makes it possible not only to navigate between Views, but to ViewModels as well, which helps to make Views more loosely coupled.

## Registering
Every View or ViewModel that's used to navigate to by the Navigation Service has to be registered in the bootstrapping phase of the app. Without registration, Prism is unable to inject dependencies that are required by the ViewModel you're navigating to.

Bootstrapper:
```
protected override void RegisterTypes()
{
  Container.RegisterTypeForNavigation<ContactPage, ContactPageViewModel>();
}
```

It is also possible to override the View name that is being registered for navigation. The RegisterTypeForNavigation method accepts a name parameter for this reason.

## Navigating
There is a caveat while injection the Navigation Service into your ViewModels. The current version of the Prism Forms library requires that you name the injection parameter precisely as ```navigationService```. Otherwise the Navigation Service is unaware of the current View it is used on. We are looking to change this, so you should be able to change the name of this parameter in the future.

```
public MainPageViewModel(INavigationService navigationService) // has to be named correctly
{
  _navigationService = navigationService;
}
```

### Navigate
Navigation to a View can be done by using the generic Navigate method, which can be used to navigate based on ViewModel in a typesafe way. The other option is to use the regular Navigate method which accepts the View name as a string.

```
_navigationService.Navigate("ContactPage");
// or better
_navigationService.Navigate<ContactPageViewModel>();
```

Depending on whether or not you're using a NavigationPage in your application, you might need to add the parameter useModalNavigation in the Navigate method, which defaults to true. When navigating within a NavigationPage you have to navigate like in this example:

```
_navigationService.Navigate<ContactPageViewModel(useModalNavigation: false);
```

### GoBack
Going back to the previous View is fairly simple by using the following example. The same applies for navigating within a NavigationPage regarding the use of the useModalNavigation parameter.

```
_navigationService.GoBack();
// or 
_navigationService.GoBack(useModalNavigation: true);
```

## Passing parameters
Passing parameters to the next View can be done using an overload of the Navigate method. This overload accepts a NavigationParameters object that can be used to supply data to the next View. The NavigationParameters object is in fact just a dictionary. It can accept any arbitrary object as a value.

```
var navigationParams = new NavigationParameters ();
navigationParams.Add("model", new Contact ());
_navigationService.Navigate<ContactPageViewModel>(navigationParams);
```

Getting to this data in the View that is being navigated to, can be achieved by using the INavigationAware interface.

## INavigationAware
This interface adds two methods to your ViewModel so you can intercept when the ViewModel is navigated to, or navigated away from. Any object that is added to the NavigationParameters object in the OnNavigatedFrom method will be available in the View that is being navigated to, which in return can get to this parameter by the OnNavigatedTo method.

Example:
```
public class ContactPageViewModel : INavigationAware {
  private Contact _contact;
  
  public void OnNavigatedTo (NavigationParameters parameters)
  {
    _contact = (Contact)parameters["model"];
  }
  
  public void OnNavigatedFrom (NavigationParameters parameters)
  {
    parameters = _contact;
  }
}
```

## IConfirmNavigation
A View can determine whether it can be navigated to or not. When implementing the IConfirmNavigation interface, the navigation process looks to see if this method returns true. Otherwise the page won't be navigated to.

```
public class ContactPageViewModel : IConfirmNavigation {
  public bool CanNavigate(NavigationParameters parameters)
  {
    // determine whether or not viewmodel can be navigated to
    return true;
  }
}
```
## NavigationPageProviderAttribute
TODO
