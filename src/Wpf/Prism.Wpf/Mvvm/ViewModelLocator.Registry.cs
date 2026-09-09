namespace Prism.Mvvm;

public static partial class ViewModelLocator
{
#if AVALONIA
    internal static readonly AttachedProperty<Type> ViewModelProperty =
        AvaloniaProperty.RegisterAttached<Control, Type>("ViewModelType", typeof(ViewModelLocator));

    internal static readonly AttachedProperty<IContainerProvider> ContainerProviderProperty =
        AvaloniaProperty.RegisterAttached<Control, IContainerProvider>("ContainerProvider", typeof(ViewModelLocator));

    private static readonly AttachedProperty<object> AutowiredViewModelProperty =
        AvaloniaProperty.RegisterAttached<Control, object>("AutowiredViewModel", typeof(ViewModelLocator));

    /// <summary>
    /// Identifies the registration used to create a view.
    /// </summary>
    public static readonly AttachedProperty<string> NavigationNameProperty =
        AvaloniaProperty.RegisterAttached<Control, string>("NavigationName", typeof(ViewModelLocator));
#else
    internal static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached(
        "ViewModelType", typeof(Type), typeof(ViewModelLocator), new PropertyMetadata(null));

    internal static readonly DependencyProperty ContainerProviderProperty = DependencyProperty.RegisterAttached(
        "ContainerProvider", typeof(IContainerProvider), typeof(ViewModelLocator), new PropertyMetadata(null));

    private static readonly DependencyProperty AutowiredViewModelProperty = DependencyProperty.RegisterAttached(
        "AutowiredViewModel", typeof(object), typeof(ViewModelLocator), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the registration used to create a view.
    /// </summary>
    public static readonly DependencyProperty NavigationNameProperty = DependencyProperty.RegisterAttached(
        "NavigationName", typeof(string), typeof(ViewModelLocator), new PropertyMetadata(null));
#endif

    /// <summary>
    /// Gets the view's navigation name, falling back to its type name.
    /// </summary>
    public static string GetNavigationName(DependencyObject obj) =>
        (string)obj.GetValue(NavigationNameProperty) ?? obj.GetType().Name;

    /// <summary>
    /// Sets the registration name associated with a view.
    /// </summary>
    public static void SetNavigationName(DependencyObject obj, string value) =>
        obj.SetValue(NavigationNameProperty, value);

    internal static void Autowire(object view, bool force = false)
    {
        if (view is not FrameworkElement element)
            return;

#if UNO_WINUI
        if (GetAutowireViewModel(element) == false)
#else
        if (GetAutoWireViewModel(element) == false)
#endif
            return;

        // Construction can wire a context before the registry supplies the requested alias.
        // Preserve application-assigned contexts, but allow the locator's context to be corrected.
        if (!force && element.DataContext is not null &&
            !ReferenceEquals(element.DataContext, element.GetValue(AutowiredViewModelProperty)))
            return;

        try
        {
            ViewModelLocationProvider.AutoWireViewModelChanged(view, Bind,
                element.GetValue(ViewModelProperty) as Type);
        }
        catch (ViewModelCreationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ViewModelCreationException(view, ex);
        }
    }

    private static void Bind(object view, object viewModel)
    {
        if (view is FrameworkElement element)
        {
            element.DataContext = viewModel;
            element.SetValue(AutowiredViewModelProperty, viewModel);
        }
    }
}
