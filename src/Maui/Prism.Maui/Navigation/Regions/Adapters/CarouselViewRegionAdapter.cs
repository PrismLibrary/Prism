using System.Collections.Specialized;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Properties;

namespace Prism.Navigation.Regions.Adapters;

/// <summary>
/// Adapter that creates a new <see cref="SingleActiveRegion"/> and monitors its
/// active view to set it on the adapted <see cref="CarouselView"/>.
/// </summary>
public class CarouselViewRegionAdapter : RegionAdapterBase<CarouselView>
{
    /// <summary>
    /// Initializes a new instance of <see cref="CarouselViewRegionAdapter"/>.
    /// </summary>
    /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
    public CarouselViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
        : base(regionBehaviorFactory)
    {
    }

    /// <summary>
    /// Adapts a <see cref="CarouselView"/> to an <see cref="IRegion"/>.
    /// </summary>
    /// <param name="region">The new region being used.</param>
    /// <param name="regionTarget">The object to adapt.</param>
    protected override void Adapt(IRegion region, CarouselView regionTarget)
    {
        if (regionTarget == null)
            throw new ArgumentNullException(nameof(regionTarget));

        bool itemsSourceIsSet = regionTarget.ItemsSource != null || regionTarget.IsSet(ItemsView.ItemsSourceProperty);

        if (itemsSourceIsSet)
            throw new InvalidOperationException(Resources.CarouselViewHasItemsSourceException);


        regionTarget.ItemsSource = region.Views;
        regionTarget.ItemTemplate = new RegionItemsSourceTemplate();
        var regionBehavior = new CarouselRegionBehavior(region);
        regionTarget.Behaviors.Add(regionBehavior);

        region.ActiveViews.CollectionChanged += delegate
        {
            var activeView = region.ActiveViews.OfType<VisualElement>().FirstOrDefault();
            regionBehavior.CurrentView = activeView;
            regionTarget.CurrentItem = activeView;
        };

        void OnFirstItemAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var items = e.NewItems.Cast<object>()
                                      .Where(x => x is VisualElement)
                                      .Cast<VisualElement>()
                                      .ToList();
                if (region.ActiveViews.Count() == 0)
                {
                    region.Activate(items[0]);
                }

                region.Views.CollectionChanged -= OnFirstItemAdded;
            }
        }

        region.Views.CollectionChanged += OnFirstItemAdded;
    }

    /// <summary>
    /// Creates a new instance of <see cref="SingleActiveRegion"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="SingleActiveRegion"/>.</returns>
    protected override IRegion CreateRegion(IContainerProvider container) =>
        container.Resolve<SingleActiveRegion>();

    private class CarouselRegionBehavior : BehaviorBase<CarouselView>
    {
        private IRegion _region { get; }
        public VisualElement CurrentView;

        public CarouselRegionBehavior(IRegion region)
        {
            _region = region;
        }

        protected override void OnAttachedTo(CarouselView carousel)
        {
            base.OnAttachedTo(carousel);
            carousel.CurrentItemChanged += OnCurrentItemChanged;
        }

        protected override void OnDetachingFrom(CarouselView carousel)
        {
            base.OnDetachingFrom(carousel);
            carousel.CurrentItemChanged -= OnCurrentItemChanged;
        }

        private void OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            if (sender is CarouselView carousel && carousel.CurrentItem != CurrentView && carousel.CurrentItem != null && carousel.CurrentItem is VisualElement newActiveView)
            {
                var previousView = CurrentView;
                CurrentView = newActiveView;

                if (!_region.ActiveViews.Contains(newActiveView))
                {
                    _region.Activate(newActiveView);
                }

                var name = newActiveView.GetValue(ViewModelLocator.NavigationNameProperty) as string;
                if (string.IsNullOrEmpty(name))
                {
                    var viewType = newActiveView.GetType();
                    var registry = _region.Container().Resolve<IRegionNavigationRegistry>();
                    var candidate = registry.ViewsOfType(viewType)
                        .Where(x => x.Type == ViewType.Region)
                        .FirstOrDefault(x => x.View == viewType);
                    if (candidate is null)
                        name = viewType.FullName;
                    else
                        name = candidate.Name;
                }

                var context = new NavigationContext(_region.NavigationService, new Uri(name, UriKind.RelativeOrAbsolute));

                MvvmHelpers.OnNavigatedFrom(previousView, context);
                MvvmHelpers.OnNavigatedTo(newActiveView, context);
            }
        }
    }
}
