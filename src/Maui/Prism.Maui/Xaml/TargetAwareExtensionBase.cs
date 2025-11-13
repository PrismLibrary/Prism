using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Prism.Behaviors;
using Prism.Extensions;
using Prism.Ioc;
using Prism.Navigation.Xaml;
using Prism.Properties;

namespace Prism.Xaml;

/// <summary>
/// Provides a base class for XAML markup extensions that are aware of their target element and page context.
/// </summary>
/// <typeparam name="T">The type of value provided by the markup extension.</typeparam>
public abstract class TargetAwareExtensionBase<T> : BindableObject, IMarkupExtension<T>
{
    private ILogger _logger;

    /// <summary>
    /// Gets the <see cref="ILogger"/> instance for logging within the extension.
    /// </summary>
    protected ILogger Logger => _logger ??= GetLogger();

    private Page _page;

    /// <summary>
    /// Gets or sets the <see cref="Page"/> that contains the target element.
    /// </summary>
    protected internal Page Page
    {
        get => _page;
        set
        {
            OnPropertyChanging(nameof(Page));
            _page = value;
            OnPropertyChanged(nameof(Page));
        }
    }

    private VisualElement _targetElement;

    /// <summary>
    /// Gets or sets the target <see cref="VisualElement"/> to which the extension is applied.
    /// </summary>
    protected internal VisualElement TargetElement
    {
        get => _targetElement;
        set
        {
            OnPropertyChanging(nameof(TargetElement));
            _targetElement = value;
            OnPropertyChanged(nameof(TargetElement));
        }
    }

    /// <summary>
    /// Gets the <see cref="IContainerProvider"/> associated with the target element.
    /// </summary>
    protected IContainerProvider Container => TargetElement.GetContainerProvider();

    /// <summary>
    /// Gets or sets the strategy for determining the binding context for the target.
    /// </summary>
    public TargetBindingContext TargetBindingContext { get; set; }

    /// <summary>
    /// Provides the value for the markup extension.
    /// </summary>
    /// <param name="serviceProvider">The service provider for the markup extension.</param>
    /// <returns>The value to set on the property where the extension is applied.</returns>
    /// <exception cref="ArgumentException">Thrown if the service provider does not provide an <see cref="IProvideValueTarget"/>.</exception>
    /// <exception cref="Exception">Thrown if the target object is not supported.</exception>
    T IMarkupExtension<T>.ProvideValue(IServiceProvider serviceProvider)
    {
        var valueTargetProvider = serviceProvider.GetService<IProvideValueTarget>();

        if (valueTargetProvider == null)
            throw new ArgumentException(Resources.ServiceProviderDidNotHaveIProvideValueTarget);

        TargetElement = valueTargetProvider.TargetObject as VisualElement;

        //this is handling the scenario of the extension being used within the EventToCommandBehavior
        if (TargetElement is null && valueTargetProvider.TargetObject is BehaviorBase<BindableObject> behavior)
            TargetElement = behavior.AssociatedObject as VisualElement;

        if (TargetElement is null)
            throw new Exception($"{valueTargetProvider.TargetObject} is not supported");

        if (TargetElement.TryGetParentPage(out var page))
            Page = page;
        else
            TargetElement.Behaviors.Add(new ElementParentedCallbackBehavior(() => Page = TargetElement.GetParentPage()));

        return ProvideValue(serviceProvider);
    }

    /// <summary>
    /// Provides the value for the markup extension (non-generic implementation).
    /// </summary>
    /// <param name="serviceProvider">The service provider for the markup extension.</param>
    /// <returns>The value to set on the property where the extension is applied.</returns>
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
        ((IMarkupExtension<T>)this).ProvideValue(serviceProvider);

    /// <summary>
    /// When implemented in a derived class, provides the value for the markup extension.
    /// </summary>
    /// <param name="serviceProvider">The service provider for the markup extension.</param>
    /// <returns>The value to set on the property where the extension is applied.</returns>
    protected abstract T ProvideValue(IServiceProvider serviceProvider);

    /// <summary>
    /// Gets the <see cref="ILogger"/> instance for the current page context.
    /// </summary>
    /// <returns>An <see cref="ILogger"/> instance, or <c>null</c> if the page is not set.</returns>
    private ILogger GetLogger()
    {
        if (Page is null)
            return null;

        var loggerFactory = Page.GetContainerProvider().Resolve<ILoggerFactory>();
        return loggerFactory.CreateLogger(GetType().Name);
    }

    /// <summary>
    /// Called when a property value changes.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(TargetElement) || propertyName == nameof(Page))
        {
            var source = TargetBindingContext == TargetBindingContext.Element ? TargetElement : Page;
            if (source is not null)
                SetBinding(BindingContextProperty, new Binding(nameof(BindingContext), BindingMode.OneWay, source: source));
        }
    }
}
