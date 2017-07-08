# Working with TabbedPage's (or any MultiPage<T>)

## Initialization

Currently Initialization largely left up to the developer to handle. Prism will pass NavigationParameters to the Selected Child page of a TabbedPage if included in Navigation Uri like `NavigationService.NavigateAsync("MyTabbedPage/ViewA")`. However the other child pages will not have a chance to initialize with the NavigationParameters. There is a full example showing different techniques for handling View Initialization in the [samples](https://github.com/PrismLibrary/Prism-Samples-Forms) repo.

```cs
public class ChildViewAModel : BindableBase, INavigatingAware
{
    protected bool HasInitialized { get; set; }

    private string _message;
    public string Message
    {
        get { return _message; }
        set { SetProperty(ref _message, value); }
    }

    public void OnNavigatingTo(NavigationParameters parameters)
    {
        if(HasInitialized) return;
        HasInitialized = true;

        Message = parameters.GetValue<string>("message");
    }
}
```

You will notice in the example code that we checked the value for `HasInitialzied` and if it was true we simply return, otherwise we continue with our initialization logic. This will prevent any issues with double initialization as previously mentioned. To initialize the children we must implement `INavigatingAware` in our actual TabbedPage as follows:

```cs
public partial class MyTabbedPage : TabbedPage, INavigatingAware
{
    public MyTabbedPage()
    {
        InitializedComponent();
    }

    public void OnNavigatingTo(NavigationParameters parameters)
    {
        foreach(var child in Children)
        {
            PageUtilities.OnNavigatingTo(child, parameters);
        }
    }
}
```

## IActiveAware

Prism Forms 6.3.0's introduction of Behaviors, also introduced the `MutliPageActiveAwareBehavior`, with an implementation for `CarouselPage` and `TabbedPage`. Each of these page types automatically get this behavior attached by the Navigation Service. This behavior will help you determine when to handle activation or deactivation events in the ViewModels of your Child Pages.

The following code shows a possible way to handle a Child Page that will be initialized with `NavigationParameters` and then handle `IActiveAware.IsActive`. There is more than one way to do this, and this should not be taken as "The way it has to be done". It is also worth noting that the Behavior, will only set `IsActive` to `true` or `false`. While using the Event Handler is one possible implementation, you could just as easily bypass it's use.

```cs
public abstract class ChildViewModelBase : BindableBase, IActiveAware, INavigatingAware, IDestructible
{
    protected bool HasInitialized { get; set; }

    public event EventHandler IsActiveChanged;

    private bool _isActive;
    public bool IsActive
    {
        get { return _isActive; }
        set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
    }

    public virtual void OnNavigatingTo(NavigationParameters parameters)
    {

    }

    public virtual void Destroy()
    {

    }

    protected virtual void RaiseIsActiveChanged()
    {
        IsActiveChanged?.Invoke(this, EventArgs.Empty);
    }
}

public class ChildViewA : ChildViewModelBase
{
    public ChildViewA()
    {
        IsActiveChanged += HandleIsActiveTrue;
        IsActiveChanged += HandleIsActiveFalse;
    }

    public override void OnNavigatingTo(NavigationParameters parameters)
    {
        if(HasInitialized) return;

        HasInitialized = true;

        // Implement your implementation logic here...
    }

    private void HandleIsActiveTrue(object sender, EventArgs args)
    {
        if(IsActive == false) return;

        // Handle Logic Here
    }

    private void HandleIsActiveFalse(object sender, EventArgs args)
    {
        if(IsActive == true) return;

        // Handle Logic Here
    }

    public override void Destroy()
    {
        IsActiveChanged -= HandleIsActiveTrue;
        IsActiveChanged -= HandleIsActiveFalse;
    }
}
```
