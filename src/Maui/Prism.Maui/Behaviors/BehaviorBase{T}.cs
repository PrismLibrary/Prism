namespace Prism.Behaviors;

/// <summary>
/// Tracks the attached bindable object and synchronizes the behavior's binding context.
/// </summary>
/// <typeparam name="T">The bindable object type to which the behavior can attach.</typeparam>
public class BehaviorBase<T> : Behavior<T> where T : BindableObject
{
    /// <summary>
    /// The Object associated with the Behavior
    /// </summary>
    public T AssociatedObject { get; private set; }

    /// <inheritDoc />
    protected override void OnAttachedTo(T bindable)
    {
        base.OnAttachedTo(bindable);
        AssociatedObject = bindable;

        if (bindable.BindingContext != null)
        {
            BindingContext = bindable.BindingContext;
        }

        bindable.BindingContextChanged += OnBindingContextChanged;
    }

    /// <inheritDoc />
    protected override void OnDetachingFrom(T bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.BindingContextChanged -= OnBindingContextChanged;
        AssociatedObject = null;
    }

    void OnBindingContextChanged(object sender, EventArgs e)
    {
        OnBindingContextChanged();
    }

    /// <inheritDoc />
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        BindingContext = AssociatedObject.BindingContext;
    }
}
