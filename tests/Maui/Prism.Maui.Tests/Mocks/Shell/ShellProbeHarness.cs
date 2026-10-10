using Moq;
using Prism.Behaviors;
using Prism.Common;
using Prism.Container.Microsoft;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Xaml;

namespace Prism.Maui.Tests.Mocks.Shell;

internal sealed class ShellProbeHarness : IDisposable
{
    private readonly List<Page> _pages = new();

    public ShellProbeHarness()
    {
        DispatcherProvider.SetCurrent(new ShellProbeDispatcherProvider());
        ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(_ => null);
        ViewModelLocationProvider.SetDefaultViewToViewModelTypeResolver(view =>
            (view as BindableObject)?.GetValue(ViewModelLocator.ViewModelProperty) as Type);
        ViewModelLocationProvider.SetDefaultViewModelFactory(PrismAppBuilder.DefaultViewModelLocator);

        Container.RegisterScoped<IPageAccessor, PageAccessor>();
        Container.RegisterScoped<ShellProbeDependency>();
        Container.RegisterScoped<INavigationService>(_ => Mock.Of<INavigationService>());
        Container.Register<INavigationRegistry>(provider =>
            new NavigationRegistry(provider.Resolve<IEnumerable<ViewRegistration>>()));
        Container.RegisterPageBehavior<PageScopeBehavior>();
        Container.RegisterPageBehavior<PageLifeCycleAwareBehavior>();
        Container.RegisterForNavigation<ShellProbePage, ShellProbeViewModel>("detail");
    }

    public MicrosoftContainerExtension Container { get; } = new();
    public IServiceProvider Services => Container.Instance;

    public ShellProbePage CreatePage(string key = "detail")
    {
        var page = (ShellProbePage)new ShellRouteFactoryPrototype(key).GetOrCreate(Services);
        return Track(page);
    }

    public ShellProbePage Track(ShellProbePage page)
    {
        if (!_pages.Contains(page))
            _pages.Add(page);

        return page;
    }

    public void Dispose()
    {
        foreach (var page in _pages)
        {
            page.Behaviors.Clear();
            page.SetContainerProvider(null);
            page.BindingContext = null;
        }

        Container.Instance.Dispose();
        ViewModelLocationProvider.Reset();
    }
}
