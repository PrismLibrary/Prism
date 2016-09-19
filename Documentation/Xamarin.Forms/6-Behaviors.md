# Prism Behaviors

Prism provides ```BehaviorsBase<T>``` both to make it easier for new Behaviors to be added to Prism as well as your projects. This base behavior provides you with an ```AssociatedObject``` of type T to make it easy to access the object the behavior is attached to. 

Included Behaviors:
- [MultiPageNavigationBehavior](#lateral-navigation-for-multipages)

## Lateral Navigation for MultiPages

Many apps may contain some form of MultiPage (TabbedPage, CarouselPage, or custom). Prism provides implementations for CarouselPage and MultiPage<Page> (TabbedPage) as well as a generic MultiPage<T> that you are able to use should neither of these fit your needs out of the box. These pages break traditional navigation paradigms when it comes to handling scenarios where our view models need to be aware of the navigation event. In order to assist with this Prism has introduced the ```MultiPageNavigationBehavior```. 

### Navigation Awareness

By default this behavior will look for ```IMultiPageNavigationAware``` and it's async counterpart ```IMultiPageNavigationAwareAsync``` on the children of the MultiPage as well as their BindingContext (View Model).

```cs
public class TabA : Page, IMultiPageNavigationAware
{
    public void OnInternalNavigatedFrom(NavigationParameters parameters)
    {
        // Do Foo
    }

    public void OnInternalNavigatedTo(NavigationParameters parameters)
    {
        // Do Bar
    }
}
```

```cs
public class TabAViewModel : IMultiPageNavigationAwareAsync
{
    public async Task OnInternalNavigatedFromAsync(NavigationParameters parameters)
    {
        // Await Foo
    }

    public async Task OnInternalNavigatedToAsync(NavigationParameters parameters)
    {
        // await Bar
    }
}
```


### IMultiPageNavigationOptions 

In the event that you wish to disable the automatic injection of this behavior by the ```NavigationService``` you will need to implement IMultiPageNavigationOptions

```cs
public class MyTabbedPage : TabbedPage, IMultiPageNavigationOptions
{
    // Will prevent the Behavior from being injected by the NavigationService
    public bool InjectNavigationBehavior { get; }
}
```

### IInternalNavigationParent

Due to the nature of how normal navigation works versus the sideways navigation of a MultiPage the Xamarin Forms API does not provide a way for us to directly pass navigation parameters from Parent to child. We therefore accomplish this by implementing the ```IInternalNavigationParent``` on either the MultiPage or it's ViewModel. This allows us a way to expose NavigationParameters that can be shared between the MultiPage parent and it's children. Additionally should you implement this on both the MultiPage and it's ViewModel it will merge the two with preference for parameters from the ViewModel (should you have duplicate keys). Note that this does not need to be implemented and the behavior will simply pass a new instance of the NavigationParameters.

```cs
public class MyTabbedPageViewModel : BindableBase, INavigationAware, IInternalNavigationParent
{
    public NavigationParameters SharedParameters { get; set; }

    public void OnNavigatedTo(NavigationParameters parameters)
    {
        parameters.Add("example", true);
        SharedParameters = parameters;
    }
}

public class TabAViewModel : BindableBase, IInternalNavigationAware
{
    public void OnInternalNavigatedTo(NavigationParameters parameters)
    {
        // parameters will contain the key "example"
    }

    public void OnInternalNavigatedFrom(NavigationParameters parameters)
    {
        parameters.Add("message", "hello from Tab A");
    }
}

public  class TabBViewModel : BindableBase, IInternalNavigationAware
{
    public void OnInternalNavigatedTo(NavigationParameters parameters)
    {
        // parameters will contain the key "example"
        // parameters will also contain the key "message" with the message from Tab A
    }
}
```
