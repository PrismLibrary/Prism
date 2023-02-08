using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Prism.Behaviors;
using Prism.Extensions;
using Prism.Ioc;
using Prism.Navigation.Xaml;
using Prism.Properties;

namespace Prism.Xaml;

public abstract class TargetAwareExtensionBase<T> : BindableObject, IMarkupExtension<T>
{
    private ILogger _logger;
    public ILogger Logger => _logger ??= GetLogger();

    private Page _page;
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

    protected IContainerProvider Container => TargetElement.GetContainerProvider();

    /// <summary>
    /// Sets the Target BindingContext strategy
    /// </summary>
    public TargetBindingContext TargetBindingContext { get; set; }

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

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => 
        ((IMarkupExtension<T>)this).ProvideValue(serviceProvider);

    protected abstract T ProvideValue(IServiceProvider serviceProvider);

    private ILogger GetLogger()
    {
        if (Page is null)
            return null;

        var loggerFactory = Page.GetContainerProvider().Resolve<ILoggerFactory>();
        return loggerFactory.CreateLogger(GetType().Name);
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if(propertyName == nameof(TargetElement) || propertyName == nameof(Page))
        {
            var source = TargetBindingContext == TargetBindingContext.Element ? TargetElement : Page;
            if(source is not null)
                SetBinding(BindingContextProperty, new Binding(nameof(BindingContext), BindingMode.OneWay, source: source));
        }
    }
}
