using System.ComponentModel;
using Prism.Extensions;
using Prism.Navigation.Xaml;

namespace Prism.Behaviors;

internal class ElementParentedCallbackBehavior : Behavior<VisualElement>
{
    private readonly Action _callback;
    private readonly bool _includeSelf;
    private VisualElement? _target;

    public ElementParentedCallbackBehavior(Action callback, bool includeSelf = false)
    {
        _callback = callback;
        _includeSelf = includeSelf;
    }

    protected override void OnAttachedTo(VisualElement view)
    {
        _target = view;

        var page = GetPage(view);
        if (page is not null)
        {
            var container = page.GetContainerProvider();
            if (container is null)
            {
                page.PropertyChanged -= PagePropertyChanged;
                page.PropertyChanged += PagePropertyChanged;
            }
            else
            {
                view.SetContainerProvider(container);
                _callback();
            }
        }
        else
        {
            view.ParentChanged += OnParentChanged;
        }
    }

    protected override void OnDetachingFrom(VisualElement view)
    {
        view.ParentChanged -= OnParentChanged;
        if (view.Parent is VisualElement directParent)
            directParent.ParentChanged -= OnParentChanged;

        var page = GetPage(view);
        if (page is not null)
            page.PropertyChanged -= PagePropertyChanged;

        _target = null;
        base.OnDetachingFrom(view);
    }

    private void OnParentChanged(object sender, EventArgs e)
    {
        // Use _target: when listening on Parent.ParentChanged, sender is the parent, not the region host (#3332).
        var view = _target;
        if (view?.Parent is null)
            return;

        var page = GetPage(view);
        if (page is not null)
        {
            if (page.GetContainerProvider() is not null)
            {
                view.ParentChanged -= OnParentChanged;
                if (view.Parent is VisualElement directParent)
                    directParent.ParentChanged -= OnParentChanged;

                _callback();
                return;
            }

            page.PropertyChanged -= PagePropertyChanged;
            page.PropertyChanged += PagePropertyChanged;
        }
        else if (view.Parent is VisualElement parent)
        {
            parent.ParentChanged -= OnParentChanged;
            parent.ParentChanged += OnParentChanged;
        }

        view.ParentChanged -= OnParentChanged;
    }

    private Page GetPage(VisualElement view) =>
        _includeSelf && view is Page page ? page : view.GetParentPage();

    private void PagePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is not Page page || e.PropertyName != Navigation.Xaml.Navigation.PrismContainerProvider)
            return;

        var container = page.GetContainerProvider();

        if (container is not null)
        {
            page.PropertyChanged -= PagePropertyChanged;
            _callback();
        }
    }
}
