using Prism.Navigation.Regions;

namespace Prism.Mvvm;

public static partial class ViewModelLocator
{
    internal static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached(
        "ViewModelType", typeof(Type), typeof(ViewModelLocator), new PropertyMetadata(null));

    internal static readonly DependencyProperty ContainerProviderProperty = DependencyProperty.RegisterAttached(
        "ContainerProvider", typeof(IContainerProvider), typeof(ViewModelLocator), new PropertyMetadata(null));

    private static readonly DependencyProperty AutowiredViewModelProperty = DependencyProperty.RegisterAttached(
        "AutowiredViewModel", typeof(object), typeof(ViewModelLocator), new PropertyMetadata(null));

    private static readonly DependencyProperty DeferredAutowireProperty = DependencyProperty.RegisterAttached(
        "DeferredAutowire", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false));

    /// <summary>Identifies the registration used to create a view.</summary>
    public static readonly DependencyProperty NavigationNameProperty = DependencyProperty.RegisterAttached(
        "NavigationName", typeof(string), typeof(ViewModelLocator), new PropertyMetadata(null));

    /// <summary>Gets the view's navigation name, falling back to its type name.</summary>
    public static string GetNavigationName(DependencyObject obj) =>
        (string)obj.GetValue(NavigationNameProperty) ?? obj.GetType().Name;

    /// <summary>Sets the registration name associated with a view.</summary>
    public static void SetNavigationName(DependencyObject obj, string value) =>
        obj.SetValue(NavigationNameProperty, value);

    internal static void Autowire(object view, bool force = false)
    {
        if (view is not FrameworkElement element || GetAutowireViewModel(element) == false)
            return;

        // XAML can request autowiring in the constructor, before its window or view
        // registry has attached the originating scope. Never use the application root.
        var provider = NativeRegionScope.FindProvider(element);
        if (provider is null)
        {
            if (!(bool)element.GetValue(DeferredAutowireProperty))
            {
                element.SetValue(DeferredAutowireProperty, true);
                element.Loaded += OnDeferredAutowire;
            }
            return;
        }

        CancelDeferredAutowire(element);
        NativeRegionScope.ConfigureView(element, provider);
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

    internal static void CancelDeferredAutowire(DependencyObject view)
    {
        if (view is FrameworkElement element)
        {
            element.Loaded -= OnDeferredAutowire;
            element.ClearValue(DeferredAutowireProperty);
        }
    }

    private static void OnDeferredAutowire(object sender, RoutedEventArgs args)
    {
        var element = (FrameworkElement)sender;
        CancelDeferredAutowire(element);
        NativeRegionScope.RequireProvider(element);
        Autowire(element);
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
