using System.ComponentModel;
using Prism.Ioc;
using Prism.Properties;

namespace Prism.Navigation.Regions.Behaviors;

/// <summary>
/// Populates the target region with the views registered to it in the <see cref="IRegionViewRegistry"/>.
/// </summary>
public class AutoPopulateRegionBehavior : RegionBehavior, IHostAwareRegionBehavior
{
    /// <summary>
    /// The key of this behavior.
    /// </summary>
    public const string BehaviorKey = "AutoPopulate";

    private readonly IRegionViewRegistry regionViewRegistry;
    private VisualElement hostControl;
    private bool attachStarted;

    /// <summary>
    /// Gets or sets the host whose DefaultView is used for initial population.
    /// </summary>
    public VisualElement HostControl
    {
        get => hostControl;
        set
        {
            if (IsAttached)
                throw new InvalidOperationException(Resources.HostControlCannotBeSetAfterAttach);

            hostControl = value;
        }
    }

    /// <summary>
    /// Creates a new instance of the AutoPopulateRegionBehavior
    /// associated with the <see cref="IRegionViewRegistry"/> received.
    /// </summary>
    /// <param name="regionViewRegistry"><see cref="IRegionViewRegistry"/> that the behavior will monitor for views to populate the region.</param>
    public AutoPopulateRegionBehavior(IRegionViewRegistry regionViewRegistry)
    {
        this.regionViewRegistry = regionViewRegistry;
    }

    /// <summary>
    /// Attaches the AutoPopulateRegionBehavior to the Region.
    /// </summary>
    protected override void OnAttach()
    {
        // RegionBehavior.Attach itself is not idempotent. Do not populate or subscribe twice.
        if (attachStarted)
            return;

        attachStarted = true;
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
        foreach (VisualElement view in CreateViewsToAutoPopulate())
        {
            AddViewIntoRegion(view);
        }

        PopulateDefaultView();

        regionViewRegistry.ContentRegistered += OnViewRegistered;
    }

    private void PopulateDefaultView()
    {
        // Keep target-aware custom regions working when this behavior is attached directly.
        var host = HostControl ?? (Region as ITargetAwareRegion)?.TargetElement;
        var defaultView = host == null ? null : Xaml.RegionManager.GetDefaultView(host);
        if (defaultView == null)
            return;

        object view = defaultView;
        if (defaultView is string || defaultView is Type)
        {
            // Reuse discovery's scoped resolution rules. This registry is method-local and
            // never retains registrations in the application-wide discovery registry.
            var defaults = new RegionViewRegistry();
            if (defaultView is string name)
                defaults.RegisterViewWithRegion(Region.Name, name);
            else
                defaults.RegisterViewWithRegion(Region.Name, (Type)defaultView);

            view = defaults.GetContents(Region.Name, Region.Container()).Single();
        }

        if (Region.Views.Any(existing => ReferenceEquals(existing, view)))
            return;

        // Preserve the named-view lookup supported by MAUI's existing string default.
        if (defaultView is string viewName)
            Region.Add(view, viewName);
        else
            AddViewIntoRegion((VisualElement)view);
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

    private void Region_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
    /// <param name="e"></param>
    public virtual void OnViewRegistered(object sender, ViewRegisteredEventArgs e)
    {
        if (e == null)
            throw new ArgumentNullException(nameof(e));

        if (e.RegionName == Region.Name)
        {
            AddViewIntoRegion((VisualElement)e.GetView(Region.Container()));
        }
    }
}
