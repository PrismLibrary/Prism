namespace Prism.Mvvm;

/// <summary>
/// This class defines the attached property and related change handler that calls the <see cref="ViewModelLocationProvider"/>.
/// </summary>
public static class ViewModelLocator
{
    /// <summary>
    /// Instructs Prism whether or not to automatically create an instance of a ViewModel using a convention, and assign the associated View's <see cref="BindableObject.BindingContext"/> to that instance.
    /// </summary>
    public static readonly BindableProperty AutowireViewModelProperty =
        BindableProperty.CreateAttached("AutowireViewModel", typeof(ViewModelLocatorBehavior), typeof(ViewModelLocator), ViewModelLocatorBehavior.Automatic, propertyChanged: OnViewModelLocatorBehaviorChanged);

    private static void OnViewModelLocatorBehaviorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is ViewModelLocatorBehavior behavior && behavior == ViewModelLocatorBehavior.Forced)
        {
            Autowire(bindable);
        }
    }

    internal static readonly BindableProperty ViewModelProperty =
        BindableProperty.CreateAttached("ViewModelType",
            typeof(Type),
            typeof(ViewModelLocator),
            null,
            propertyChanged: OnViewModelPropertyChanged);

    public static readonly BindableProperty NavigationNameProperty =
        BindableProperty.CreateAttached("NavigationName", typeof(string), typeof(ViewModelLocator), null, defaultValueCreator: CreateDefaultNavigationName);

    private static object CreateDefaultNavigationName(BindableObject bindable) => bindable.GetType().Name;

    public static string GetNavigationName(BindableObject bindable) =>
        (string)bindable.GetValue(NavigationNameProperty);

    public static void SetNavigationName(BindableObject bindable, string name) =>
        bindable.SetValue(NavigationNameProperty, name);

    /// <summary>
    /// Gets the AutowireViewModel property value.
    /// </summary>
    /// <param name="bindable"></param>
    /// <returns></returns>
    public static ViewModelLocatorBehavior GetAutowireViewModel(BindableObject bindable)
    {
        return (ViewModelLocatorBehavior)bindable.GetValue(AutowireViewModelProperty);
    }

    /// <summary>
    /// Sets the AutowireViewModel property value.  If <c>true</c>, creates an instance of a ViewModel using a convention, and sets the associated View's <see cref="BindableObject.BindingContext"/> to that instance.
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="value"></param>
    public static void SetAutowireViewModel(BindableObject bindable, ViewModelLocatorBehavior value)
    {
        bindable.SetValue(AutowireViewModelProperty, value);
    }

    private static void OnViewModelPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue == null || bindable.BindingContext != null)
            return;
        else if(newValue is Type)
            bindable.SetValue(AutowireViewModelProperty, ViewModelLocatorBehavior.Automatic);
    }

    internal static void Autowire(object view)
    {
        if (view is Element element &&
            ((ViewModelLocatorBehavior)element.GetValue(AutowireViewModelProperty) == ViewModelLocatorBehavior.Disabled
            || (element.BindingContext is not null && element.BindingContext != element.Parent)))
        {
            return;
        }

        if (view is TabbedPage tabbed)
        {
            foreach (var child in tabbed.Children)
                Autowire(child);
        }
        else if(view is NavigationPage navigationPage && navigationPage.RootPage is not null)
        {
            Autowire(navigationPage.RootPage);
        }

        ViewModelLocationProvider.AutoWireViewModelChanged(view, Bind);

        if (view is BindableObject bindable && bindable.BindingContext is null)
        {
            bindable.BindingContext = new object();
        }
    }

    /// <summary>
    /// Sets the <see cref="BindableObject.BindingContext"/> of a View
    /// </summary>
    /// <param name="view">The View to set the <see cref="BindableObject.BindingContext"/> on</param>
    /// <param name="viewModel">The object to use as the <see cref="BindableObject.BindingContext"/> for the View</param>
    private static void Bind(object view, object viewModel)
    {
        if (view is BindableObject element)
        {
            element.BindingContext = viewModel;
        }
    }
}
