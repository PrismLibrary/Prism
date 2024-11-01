using System.ComponentModel;
using Prism.Ioc;

#nullable enable
namespace Prism.Navigation.Regions.Behaviors;

/// <summary>
/// Populates the target region with the views registered to it in the <see cref="IRegionViewRegistry"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of the AutoPopulateRegionBehavior
/// associated with the <see cref="IRegionViewRegistry"/> received.
/// </remarks>
/// <param name="regionViewRegistry"><see cref="IRegionViewRegistry"/> that the behavior will monitor for views to populate the region.</param>
public class AutoPopulateRegionBehavior(IRegionViewRegistry regionViewRegistry) : RegionBehavior
{
    /// <summary>
    /// The key of this behavior.
    /// </summary>
    public const string BehaviorKey = "AutoPopulate";

    /// <summary>
    /// Attaches the AutoPopulateRegionBehavior to the Region.
    /// </summary>
    protected override void OnAttach()
    {
        if (string.IsNullOrEmpty(Region.Name))
        {
            Region.PropertyChanged += Region_PropertyChanged;
        }
        else
        {
            StartPopulatingContent();
        }
    }

    private void StartPopulatingContent()
    {
        foreach (var view in CreateViewsToAutoPopulate().OfType<VisualElement>())
        {
            AddViewIntoRegion(view);
        }

        if (Region is ITargetAwareRegion targetAware && targetAware.TargetElement.GetValue(Xaml.RegionManager.DefaultViewProperty) != null)
        {
            var defaultView = targetAware.TargetElement.GetValue(Xaml.RegionManager.DefaultViewProperty);
            if (defaultView is string targetName)
                Region.Add(targetName);
            else if (defaultView is VisualElement element)
                Region.Add(element);
            else if (defaultView is Type type)
            {
                var container = targetAware.Container;
                var registry = container.Resolve<IRegionNavigationRegistry>();
                var registration = registry.Registrations.FirstOrDefault(x => x.View == type);
                if (registration is not null)
                {
                    var view = registry.CreateView(container, registration.Name);
                    Region.Add(view);
                }
            }
        }

        regionViewRegistry.ContentRegistered += OnViewRegistered;
    }

    /// <summary>
    /// Returns a collection of views that will be added to the
    /// View collection.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<object> CreateViewsToAutoPopulate()
    {
        return regionViewRegistry.GetContents(Region.Name, Region.Container());
    }

    /// <summary>
    /// Adds a view into the views collection of this region.
    /// </summary>
    /// <param name="viewToAdd"></param>
    protected virtual void AddViewIntoRegion(VisualElement viewToAdd)
    {
        Region.Add(viewToAdd);
    }

    private void Region_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Name" && !string.IsNullOrEmpty(Region.Name))
        {
            Region.PropertyChanged -= Region_PropertyChanged;
            StartPopulatingContent();
        }
    }

    /// <summary>
    /// Handler of the event that fires when a new viewtype is registered to the registry.
    /// </summary>
    /// <remarks>Although this is a public method to support Weak Delegates in Silverlight, it should not be called by the user.</remarks>
    /// <param name="sender"></param>
    /// <param name="viewRegisteredEventArgs"></param>
    public virtual void OnViewRegistered(object? sender, ViewRegisteredEventArgs viewRegisteredEventArgs)
    {
        ArgumentNullException.ThrowIfNull(viewRegisteredEventArgs);

        if (viewRegisteredEventArgs.RegionName == Region.Name)
        {
            AddViewIntoRegion((VisualElement)viewRegisteredEventArgs.GetView(Region.Container()));
        }
    }
}
