using System.ComponentModel;
using Prism.Extensions;
using Prism.Navigation.Xaml;

namespace Prism.Behaviors;

internal class ElementParentedCallbackBehavior : Behavior<VisualElement>
{
    private Action _callback { get; }

    public ElementParentedCallbackBehavior(Action callback)
    {
        _callback = callback;
    }

    protected override void OnAttachedTo(VisualElement view)
    {
        if (view.TryGetParentPage(out var page))
        {
            var container = page.GetContainerProvider();
            if (container is null)
                page.PropertyChanged += PagePropertyChanged;
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

    private void OnParentChanged(object sender, EventArgs e)
    {
        if (sender is not VisualElement view || view.Parent is null)
            return;
        else if (view.TryGetParentPage(out var page))
        {
            if(page.GetContainerProvider() is not null)
            {
                view.ParentChanged -= OnParentChanged;
                _callback();
                return;
            }
            page.PropertyChanged += PagePropertyChanged;
        }
        else
            view.Parent.ParentChanged += OnParentChanged;

        view.ParentChanged -= OnParentChanged;
    }

    private void PagePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is not Page page || e.PropertyName != Navigation.Xaml.Navigation.PrismContainerProvider)
            return;

        var container = page.GetContainerProvider();

        if(container is not null)
        {
            page.PropertyChanged -= PagePropertyChanged;
            _callback();
        }
    }
}
