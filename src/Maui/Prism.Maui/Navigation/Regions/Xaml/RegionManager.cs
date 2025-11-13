using System.Globalization;
using System.Reflection;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Navigation.Regions.Adapters;
using Prism.Navigation.Regions.Behaviors;
using Prism.Properties;

namespace Prism.Navigation.Regions.Xaml;

/// <summary>
/// A class to provide Bindable Properties and helpers for Regions.
/// </summary>
public static class RegionManager
{
    private static readonly WeakDelegatesManager updatingRegionsListeners = new ();

    private static readonly BindableProperty ObservableRegionProperty =
        BindableProperty.CreateAttached("ObservableRegion", typeof(ObservableObject<IRegion>), typeof(RegionManager), null);

    /// <summary>
    /// Identifies the RegionManager attached property.
    /// </summary>
    /// <remarks>
    /// When a control has both the <see cref="RegionNameProperty"/> and
    /// <see cref="RegionManagerProperty"/> attached properties set to
    /// a value different than <see langword="null" /> and there is a
    /// <see cref="IRegionAdapter"/> mapping registered for the control, it
    /// will create and adapt a new region for that control, and register it
    /// in the <see cref="IRegionManager"/> with the specified region name.
    /// </remarks>
    public static readonly BindableProperty RegionManagerProperty =
        BindableProperty.CreateAttached("RegionManager", typeof(IRegionManager), typeof(RegionManager), null);

    /// <summary>
    /// Identifies the RegionName attached property.
    /// </summary>
    /// <remarks>
    /// When a control has both the <see cref="RegionNameProperty"/> and
    /// <see cref="RegionManagerProperty"/> attached properties set to
    /// a value different than <see langword="null" /> and there is a
    /// <see cref="IRegionAdapter"/> mapping registered for the control, it
    /// will create and adapt a new region for that control, and register it
    /// in the <see cref="IRegionManager"/> with the specified region name.
    /// </remarks>
    public static readonly BindableProperty RegionNameProperty =
        BindableProperty.CreateAttached("RegionName", typeof(string), typeof(RegionManager), null, propertyChanged: OnSetRegionNameCallback);

    /// <summary>
    /// Identifies the RegionContext attached property.
    /// </summary>
    public static readonly BindableProperty RegionContextProperty =
        BindableProperty.CreateAttached("RegionContext", typeof(object), typeof(RegionManager), null, propertyChanged: OnRegionContextChanged);

    /// <summary>
    /// Sets the default view to be displayed in a region when it is created.
    /// </summary>
    public static readonly BindableProperty DefaultViewProperty =
        BindableProperty.CreateAttached("DefaultView", typeof(object), typeof(RegionManager), null);

    /// <summary>
    /// Sets the <see cref="DefaultViewProperty"/> attached property for the specified region target.
    /// </summary>
    /// <param name="regionTarget">The <see cref="VisualElement"/> that will host the default view.</param>
    /// <param name="viewNameTypeOrInstance">
    /// The default view to display in the region. This can be a view name, a type, or an instance of the view.
    /// </param>
    public static void SetDefaultView(VisualElement regionTarget, object viewNameTypeOrInstance) =>
        regionTarget.SetValue(DefaultViewProperty, viewNameTypeOrInstance);

    /// <summary>
    /// Gets the value of the <see cref="DefaultViewProperty"/> attached property for the specified region target.
    /// </summary>
    /// <param name="regionTarget">The <see cref="VisualElement"/> that hosts the default view.</param>
    /// <returns>
    /// The default view associated with the region. This can be a view name, a type, or an instance of the view.
    /// </returns>
    public static object GetDefaultView(VisualElement regionTarget) =>
        regionTarget.GetValue(DefaultViewProperty);

    /// <summary>
    /// Sets the <see cref="RegionNameProperty"/> attached property.
    /// </summary>
    /// <param name="regionTarget">The object to adapt. This is typically a container (i.e a control).</param>
    /// <param name="regionName">The name of the region to register.</param>
    public static void SetRegionName(VisualElement regionTarget, string regionName)
    {
        if (regionTarget == null)
            throw new ArgumentNullException(nameof(regionTarget));

        regionTarget.SetValue(RegionNameProperty, regionName);
    }

    /// <summary>
    /// Gets the value for the <see cref="RegionNameProperty"/> attached property.
    /// </summary>
    /// <param name="regionTarget">The object to adapt. This is typically a container (i.e a control).</param>
    /// <returns>The name of the region that should be created when
    /// <see cref="RegionManagerProperty"/> is also set in this element.</returns>
    public static string GetRegionName(VisualElement regionTarget)
    {
        if (regionTarget == null)
            throw new ArgumentNullException(nameof(regionTarget));

        return regionTarget.GetValue(RegionNameProperty) as string;
    }

    /// <summary>
    /// Returns an <see cref="ObservableObject{T}"/> wrapper that can hold an <see cref="IRegion"/>. Using this wrapper
    /// you can detect when an <see cref="IRegion"/> has been created by the <see cref="RegionAdapterBase{T}"/>.
    ///
    /// If the <see cref="ObservableObject{T}"/> wrapper does not yet exist, a new wrapper will be created. When the region
    /// gets created and assigned to the wrapper, you can use the <see cref="ObservableObject{T}"/> event
    /// to get notified of that change.
    /// </summary>
    /// <param name="view">The view that will host the region. </param>
    /// <returns>Wrapper that can hold an <see cref="IRegion"/> value and can notify when the <see cref="IRegion"/> value changes. </returns>
    public static ObservableObject<IRegion> GetObservableRegion(VisualElement view)
    {
        if (view is null) throw new ArgumentNullException(nameof(view));

        if (view.GetValue(ObservableRegionProperty) is not ObservableObject<IRegion> regionWrapper)
        {
            regionWrapper = new ObservableObject<IRegion>();
            view.SetValue(ObservableRegionProperty, regionWrapper);
        }

        return regionWrapper;
    }

    private static void OnSetRegionNameCallback(BindableObject bindable, object oldValue, object newValue)
    {
        if (DesignMode.IsDesignModeEnabled || newValue is null || bindable is not VisualElement view)
            return;

        CreateRegion(view);
    }

    private static void CreateRegion(VisualElement element)
    {
        var regionCreationBehavior = ContainerLocator.Container.Resolve<DelayedRegionCreationBehavior>();
        regionCreationBehavior.TargetElement = element;
        regionCreationBehavior.Attach();
    }

    /// <summary>
    /// Gets the value of the <see cref="RegionNameProperty"/> attached property.
    /// </summary>
    /// <param name="target">The target element.</param>
    /// <returns>The <see cref="IRegionManager"/> attached to the <paramref name="target"/> element.</returns>
    public static IRegionManager GetRegionManager(VisualElement target)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        return (IRegionManager)target.GetValue(RegionManagerProperty);
    }

    /// <summary>
    /// Sets the <see cref="RegionManagerProperty"/> attached property.
    /// </summary>
    /// <param name="target">The target element.</param>
    /// <param name="value">The value.</param>
    public static void SetRegionManager(VisualElement target, IRegionManager value)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        target.SetValue(RegionManagerProperty, value);
    }

    private static void OnRegionContextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue != newValue && bindable is VisualElement view)
        {
            RegionContext.GetObservableContext(view).Value = newValue;
        }
    }

    /// <summary>
    /// Gets the value of the <see cref="RegionContextProperty"/> attached property.
    /// </summary>
    /// <param name="target">The target element.</param>
    /// <returns>The region context to pass to the contained views.</returns>
    public static object GetRegionContext(VisualElement target)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        return target.GetValue(RegionContextProperty);
    }

    /// <summary>
    /// Sets the <see cref="RegionContextProperty"/> attached property.
    /// </summary>
    /// <param name="target">The target element.</param>
    /// <param name="value">The value.</param>
    public static void SetRegionContext(VisualElement target, object value)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        target.SetValue(RegionContextProperty, value);
    }

    /// <summary>
    /// Notification used by attached behaviors to update the region managers appropriately if needed to.
    /// </summary>
    /// <remarks>This event uses weak references to the event handler to prevent this static event of keeping the
    /// target element longer than expected.</remarks>
    public static event EventHandler UpdatingRegions
    {
        add => updatingRegionsListeners.AddListener(value);
        remove => updatingRegionsListeners.RemoveListener(value);
    }

    /// <summary>
    /// Notifies attached behaviors to update the region managers appropriately if needed to.
    /// </summary>
    /// <remarks>
    /// This method is normally called internally, and there is usually no need to call this from user code.
    /// </remarks>
    public static void UpdateRegions()
    {
        try
        {
            updatingRegionsListeners.Raise(null, EventArgs.Empty);
        }
        catch (TargetInvocationException ex)
        {
            Exception rootException = ex.GetRootException();

            throw new UpdateRegionsException(string.Format(CultureInfo.CurrentCulture,
                Resources.UpdateRegionException, rootException), ex.InnerException);
        }
    }
}
