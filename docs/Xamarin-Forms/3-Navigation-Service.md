#Using the Navigation Service

Navigating in a Prism application is conceptually different than standard navigation in Xamarin.Forms.  While Xamarin.Forms navigation relies on a Page class instance to navigate, Prism removes all dependencies on Page types to achieve loosely coupled navigation from within a ViewModel.  In Prism, the concept of navigating to a View or navigating to a ViewModel does not exist.  Instead, you simply navigate to an experience, or a unique identifier, which represents the target view you wish to navigate to in your application.

Page navigation in Prism is accomplished by using the **INavigationService**.

## Getting the Navigation Service
To obtain the **INavigationService** in your ViewModels simply ask for it as a constructor parameter.  There is a caveat while injecting the Navigation Service into your ViewModels. The current version of the Prism.Forms library requires that you name the injection parameter precisely as ```navigationService```. Otherwise the Navigation Service is unaware of the current View it is used on.  This is a limitation of the dependency injection container.

```
public MainPageViewModel(INavigationService navigationService) // has to be named correctly
{
  _navigationService = navigationService;
}
```

## Navigating
Once you have the **INavigationService** in your ViewModel, you can navigate to your target views by calling the `INavigationService.NavigateAsync` method and provide the unique identifier/key that represents the target Page.

```
_navigationService.NavigateAsync("MainPage");
```

For more dynamic scenarios, or scenarios which involve navigating with Uris, you can use either a relative or an absolute Uri to navigate.
```
//relative
_navigationService.NavigateAsync(new Uri("MainPage", UriKind.Relative));

//absolute
_navigationService.NavigateAsync(new Uri("http://www.brianlagunas.com/MainPage", UriKind.Absolute);
```
_Note: An absolute URI resets the navigation stack regardless of where you call it.  It is equivalent to `Application.Current.MainPage = new MainPage()`_

**Important:** If you do not register your Pages with Prism, navigation will not work.

## Registering
Registering your Page for navigation is essentially mapping a unique identifier/key to the target view during the bootstrapping process.  In order to register your Pages for Navigation, override the **RegisterTypes** method in your **Bootstrapper**.

Bootstrapper:
```
protected override void RegisterTypes()
{
    //register pages for navigation
}
```

Next, use the `RegisterTypeForNavigation<T>` extension method off of the current dependency injection container.  There are three ways to register your Pages for navigation.

#### Default Registration
By default, **RegisterTypeForNavigation** will use the **Name** of the Page type as the unique identifier/key.  The following code snippet results in a mapping between the MainPage type, and the unique identifier/key of "MainPage".  This means when you request to navigate to the MainPage, you will provide the string "MainPage" as the navigation target.
```
protected override void RegisterTypes()
{
    Container.RegisterTypeForNavigation<MainPage>();
}
```

To navigate to the MainPage using this registration method:
```
_navigationService.NavigateAsync("MainPage");
```

#### Custom Registration
You can override this convention by providing a custom unique identifier/key as a method parameter.  In this case, we are overriding the convention for MainPage, and are creating a mapping between the MainPage Page type, and the unique identifier/key of "CustomKey".  So when we want to navigate to the MainPage, we would provide the "CustomKey" as the navigation target.
```
protected override void RegisterTypes()
{
    Container.RegisterTypeForNavigation<MainPage>("CustomKey");
}
```
To navigate to the MainPage using this registration method:
```
_navigationService.NavigateAsync("CustomKey");
```

## GoBackAsync
Going back to the previous View is as simple calling the `INavigationService.GoBackAsync` method. 

```
_navigationService.GoBackAsync();
```

## Passing parameters
The Prism navigation service also allows you to pass parameters to the target view during the navigation process.  Passing parameters to the next View can be done using an overload of the **INavigationService.NavigateAsync** method. This overload accepts a **NavigationParameters** object that can be used to supply data to the next View. The **NavigationParameters** object is in fact just a dictionary. It can accept any arbitrary object as a value.

```
var navigationParams = new NavigationParameters ();
navigationParams.Add("model", new Contact ());
_navigationService.NavigateAsync("MainPage", navigationParams);
```

You can also create an HTML query string to generate your parameter collection.
```
var queryString = "code=CR&desc=Red";
var navigationParams = new NavigationParameters(queryString);
_navigationService.NavigateAsync("MainPage", navigationParameters);
```
When using a Uri to navigate, you may append the Uri with parameters, which will be used as the navigation parameters.
```
//query string
_navigationService.NavigateAsync(new Uri("MainPage?id=3&name=brian", UriKind.Relative));

//using NavigationParameters in Uri
_navigationService.NavigateAsync(new Uri("MainPage" + navParameters.ToString(), UriKind.Relative));

//using both Uri parameters and NavigationParameters
var navParameters = new NavigationParameters ();
navParameters.Add("name", "brian");
_navigationService.NavigateAsync(new Uri("MainPage?id=3", UriKind.Relative), navParameters);
```


Getting to this data in the target View that is being navigated to, can be achieved by using the **INavigationAware** interface on the corresponding ViewModel.

## INavigationAware
The ViewModel of the target navigation Page can participate in the navigation process by implementing the **INavigationAware** interface.  This interface adds two methods to your ViewModel so you can intercept when the ViewModel is navigated to, or navigated away from.

Example:
```
public class ContactPageViewModel : INavigationAware
{
  private Contact _contact;
  
  public void OnNavigatedTo(NavigationParameters parameters)
  {
    _contact = (Contact)parameters["model"];
  }
  
  public void OnNavigatedFrom(NavigationParameters parameters)
  {
    
  }
}
```

## IConfirmNavigation
A ViewModel can determine whether or not it can perform a navigation operation. When a ViewModel implements the **IConfirmNavigation** or the **IConfirmNavigationAsync** interface, the navigation process looks to see what the result of this method is.  If _true_, a navigation process can be invoked, meaning a call to `NavigationService.NavigateAsync("target")` can be made.  If _false_, the ViewModel cannot invoke the navigation process. 

```
public class ContactPageViewModel : IConfirmNavigation 
{
  public bool CanNavigate(NavigationParameters parameters)
  {
    return true;
  }
}
```
