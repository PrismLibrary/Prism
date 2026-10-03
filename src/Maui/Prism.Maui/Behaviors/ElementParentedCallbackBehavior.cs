using System.ComponentModel;
using Prism.Extensions;
using Prism.Navigation.Xaml;

namespace Prism.Behaviors;

internal class ElementParentedCallbackBehavior : Behavior<VisualElement>
{
    private readonly Action _callback;
    private readonly bool _includeSelf;
    private readonly List<Element> _observedElements = new();
    private VisualElement? _target;

    public ElementParentedCallbackBehavior(Action callback, bool includeSelf = false)
    {
        _callback = callback;
        _includeSelf = includeSelf;
    }

    protected override void OnAttachedTo(VisualElement view)
    {
        _target = view;
        TryInvokeCallback();
    }

    protected override void OnDetachingFrom(VisualElement view)
    {
        StopObserving();
        _target = null;
        base.OnDetachingFrom(view);
    }

    private void OnParentChanged(object sender, EventArgs e) => TryInvokeCallback();

    private void TryInvokeCallback()
    {
        StopObserving();

        // An ancestor may have raised the event; always resolve the original target's page.
        var view = _target;
        if (view is null)
            return;

        var page = GetPage(view);
        var container = page?.GetContainerProvider();
        if (container is not null)
        {
            // Pages already own a scope, or inherit the flyout scope without owning it.
            if (view is not Page)
                view.SetContainerProvider(container);

            _callback();
            return;
        }

        // A flyout menu inherits its FlyoutPage's scope, whose notification
        // does not propagate to the menu's attached property (#3161).
        var scopePage = page;
        while (scopePage?.Parent is FlyoutPage flyout && flyout.Flyout == scopePage)
            scopePage = flyout;

        for (Element element = view; element is not null; element = element.Parent)
        {
            _observedElements.Add(element);
            element.ParentChanged += OnParentChanged;

            if (element is Page parentPage)
                parentPage.PropertyChanged += PagePropertyChanged;

            if (element == scopePage)
                break;
        }
    }

    private Page GetPage(VisualElement view) =>
        _includeSelf && view is Page page ? page : view.GetParentPage();

    private void PagePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == Navigation.Xaml.Navigation.PrismContainerProvider)
            TryInvokeCallback();
    }

    private void StopObserving()
    {
        foreach (var element in _observedElements)
        {
            element.ParentChanged -= OnParentChanged;
            if (element is Page page)
                page.PropertyChanged -= PagePropertyChanged;
        }

        _observedElements.Clear();
    }
}
