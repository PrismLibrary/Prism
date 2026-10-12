using DryIoc;
using Microsoft.Maui.Hosting;
using Prism.Container.DryIoc;
using Prism.Ioc;
using Prism.DryIoc.Maui.Tests.Mocks;
using Prism.Events;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Adapters;

namespace Prism.DryIoc.Maui.Tests.Fixtures;

public class PrismAppBuilderCompositionTests
{
    public PrismAppBuilderCompositionTests()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
    }

    [Fact]
    public void RepeatedFactoryCallsReuseBuilderAndConstructOneContainer()
    {
        var builder = MauiApp.CreateBuilder();
        var constructions = 0;
        PrismAppBuilder first = null;
        PrismAppBuilder second = null;
        DryIocContainerExtension container = null;
        IContainerExtension CreateContainer()
        {
            constructions++;
            container = new DryIocContainerExtension();
            return container;
        }

        Assert.Same(builder, builder.UsePrism(CreateContainer, prism => first = prism));
        Assert.Same(builder, builder.UsePrism(CreateContainer, prism => second = prism));

        Assert.Equal(1, constructions);
        Assert.Same(first, second);
        Assert.Same(builder, first.MauiBuilder);
        Assert.Same(container, ContainerLocator.Current);
        Assert.False(container.Instance.IsDisposed);
        container.Instance.Dispose();
    }

    [Fact]
    public void ApplicationDisposesSelectedContainerAndServicesOnce()
    {
        var builder = MauiApp.CreateBuilder();
        var container = new DryIocContainerExtension();
        builder.UsePrism(container, prism => prism.RegisterTypes(registry =>
            registry.RegisterSingleton<PrismDisposalProbe>()));
        builder.UsePrism(() => throw new InvalidOperationException("Factory must not run twice."), _ => { });
        var app = builder.Build();
        var probe = container.Resolve<PrismDisposalProbe>();

        Assert.False(container.Instance.IsDisposed);
        Assert.Equal(0, probe.DisposeCount);
        app.Dispose();
        Assert.True(container.Instance.IsDisposed);
        Assert.Equal(1, probe.DisposeCount);
        container.Instance.Dispose();
        Assert.Equal(1, probe.DisposeCount);
    }

    [Fact]
    public void RegionConfigurationCallbacksComposeInCallOrder()
    {
        var calls = new List<string>();
        var adapters = new List<RegionAdapterMappings>();
        var behaviors = new List<IRegionBehaviorFactory>();
        var builder = MauiApp.CreateBuilder();
        void Configure(PrismAppBuilder prism, string name)
        {
            prism.ConfigureRegionAdapters(mappings =>
            {
                adapters.Add(mappings);
                calls.Add($"adapters:{name}");
            }).ConfigureRegionBehaviors(factory =>
            {
                behaviors.Add(factory);
                calls.Add($"behaviors:{name}");
            });
        }

        builder.ConfigurePrism(prism => Configure(prism, "library"));
        builder.UsePrism(prism => Configure(prism, "app"));
        using var app = builder.Build();
        app.Services.GetRequiredService<RegionAdapterMappings>();
        app.Services.GetRequiredService<IRegionBehaviorFactory>();

        Assert.Equal(new[] { "adapters:library", "adapters:app", "behaviors:library", "behaviors:app" }, calls);
        Assert.Equal(2, adapters.Count);
        Assert.Equal(2, behaviors.Count);
        Assert.Same(adapters[0], adapters[1]);
        Assert.Same(behaviors[0], behaviors[1]);
    }

    [Fact]
    public void NavigationSubscriptionSurvivesLaterUsePrism()
    {
        var callbacks = 0;
        var builder = MauiApp.CreateBuilder().UsePrism(prism => prism.OnInitialized(container =>
            container.Resolve<IEventAggregator>().GetEvent<NavigationRequestEvent>()
                .Subscribe(_ => callbacks++)));
        builder.UsePrism(prism => prism.RegisterTypes(_ => { }));
        using var app = builder.Build();
        app.Services.GetRequiredService<IEventAggregator>().GetEvent<NavigationRequestEvent>().Publish(null);
        Assert.Equal(1, callbacks);
    }

    [Fact]
    public void PendingLibraryCallbacksAreScopedToTheirApplicationBuilder()
    {
        var firstCalls = 0;
        var secondCalls = 0;
        var first = MauiApp.CreateBuilder().ConfigurePrism(_ => firstCalls++);
        var second = MauiApp.CreateBuilder().ConfigurePrism(_ => secondCalls++);
        second.UsePrism(_ => { });
        var secondContainer = (DryIocContainerExtension)ContainerLocator.Current;
        Assert.Equal(0, firstCalls);
        Assert.Equal(1, secondCalls);
        first.UsePrism(_ => { });
        Assert.Equal(1, firstCalls);
        Assert.Equal(1, secondCalls);
        ((DryIocContainerExtension)ContainerLocator.Current).Instance.Dispose();
        secondContainer.Instance.Dispose();
    }

    [Fact]
    public void InvalidConfigurationDoesNotInvokeContainerFactory()
    {
        var constructions = 0;
        var builder = MauiApp.CreateBuilder();
        IContainerExtension CreateContainer()
        {
            constructions++;
            return new DryIocContainerExtension();
        }

        Assert.Throws<ArgumentNullException>(() => builder.UsePrism(CreateContainer, null));
        Assert.Throws<ArgumentNullException>(() => builder.ConfigurePrism(null));
        Assert.Equal(0, constructions);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DryIocOverloadsReuseExplicitContainer(bool rulesFirst)
    {
        var builder = MauiApp.CreateBuilder();
        var container = new DryIocContainerExtension();
        PrismAppBuilder first = null;
        builder.UsePrism(container, prism => first = prism);

        if (rulesFirst)
            builder.UsePrism(Rules.Default, prism => Assert.Same(first, prism));
        else
            builder.UsePrism(prism => Assert.Same(first, prism));

        builder.UsePrism(Rules.Default, prism => Assert.Same(first, prism));
        builder.UsePrism(prism => Assert.Same(first, prism));
        Assert.Same(container, ContainerLocator.Current);
        Assert.False(container.Instance.IsDisposed);
        container.Instance.Dispose();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DryIocFirstCallPreservesContainerAcrossOverloads(bool rulesFirst)
    {
        var builder = MauiApp.CreateBuilder();
        PrismAppBuilder first = null;
        if (rulesFirst)
            builder.UsePrism(Rules.Default, prism => first = prism);
        else
            builder.UsePrism(prism => first = prism);

        var container = Assert.IsType<DryIocContainerExtension>(ContainerLocator.Current);
        builder.UsePrism(prism => Assert.Same(first, prism));
        builder.UsePrism(Rules.Default, prism => Assert.Same(first, prism));
        Assert.Same(container, ContainerLocator.Current);
        container.Instance.Dispose();
    }

    [Fact]
    public void UnusedExplicitContainerRemainsOwnedByCaller()
    {
        var builder = MauiApp.CreateBuilder();
        var selected = new DryIocContainerExtension();
        var unused = new DryIocContainerExtension();
        builder.UsePrism(selected, _ => { });
        builder.UsePrism(unused, _ => { });

        Assert.Same(selected, ContainerLocator.Current);
        Assert.False(selected.Instance.IsDisposed);
        Assert.False(unused.Instance.IsDisposed);
        Assert.False(unused.IsRegistered(typeof(PrismAppBuilder)));
        selected.Instance.Dispose();
        Assert.False(unused.Instance.IsDisposed);
        unused.Instance.Dispose();
    }

    [Fact]
    public void ConfigurePrismAloneDoesNotEnablePrismOrReplaceLocator()
    {
        var existing = new DryIocContainerExtension();
        ContainerLocator.SetContainerExtension(existing);
        var builder = MauiApp.CreateBuilder();
        var callbacks = 0;
        var serviceCount = builder.Services.Count;
        Assert.Same(builder, builder.ConfigurePrism(_ => callbacks++));
        Assert.Equal(serviceCount, builder.Services.Count);
        using var app = builder.Build();

        Assert.Equal(0, callbacks);
        Assert.Null(app.Services.GetService(typeof(PrismAppBuilder)));
        Assert.Same(existing, ContainerLocator.Current);
        Assert.False(existing.Instance.IsDisposed);
        existing.Instance.Dispose();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LibraryAndApplicationCallbacksComposeAndInitializeOnce(bool libraryFirst)
    {
        var calls = new List<string>();
        var builders = new List<PrismAppBuilder>();
        var providers = new List<IContainerProvider>();
        var builder = MauiApp.CreateBuilder();
        void Configure(PrismAppBuilder prism, string name)
        {
            builders.Add(prism);
            calls.Add($"configure:{name}");
            prism.RegisterTypes(_ => calls.Add($"register:{name}"))
                .ConfigureModuleCatalog(_ => calls.Add($"catalog:{name}"))
                .OnInitialized(container =>
                {
                    calls.Add($"initialize:{name}");
                    providers.Add(container);
                });
        }

        if (libraryFirst)
            builder.ConfigurePrism(prism => Configure(prism, "library"));
        builder.UsePrism(prism => Configure(prism, "app"));
        if (!libraryFirst)
            builder.ConfigurePrism(prism => Configure(prism, "library"));
        builder.UsePrism(prism => Configure(prism, "repeat"));
        using var app = builder.Build();
        builders[0].OnInitialized();
        builders[0].OnInitialized();

        var names = libraryFirst ? new[] { "library", "app", "repeat" } : new[] { "app", "library", "repeat" };
        var expected = new[] { "configure", "register", "catalog", "initialize" }
            .SelectMany(phase => names.Select(name => $"{phase}:{name}"));
        Assert.Equal(expected, calls);
        Assert.All(builders, prism => Assert.Same(builders[0], prism));
        Assert.All(providers, container => Assert.Same(ContainerLocator.Current, container));
        Assert.Same(builders[0], app.Services.GetService(typeof(PrismAppBuilder)));
    }

    [Fact]
    public void RepeatedConfigurationRetainsWindowCallbackAndInitializesOnce()
    {
        var calls = new List<string>();
        PrismAppBuilder prismBuilder = null;
        var builder = MauiApp.CreateBuilder().UseMauiApp<Application>().UsePrism(prism =>
        {
            prismBuilder = prism;
            prism.OnInitialized(() => calls.Add("initialized"))
                .CreateWindow((_, _) =>
                {
                    calls.Add("window");
                    return Task.CompletedTask;
                });
        });
        builder.UsePrism(prism => prism.OnInitialized(() => calls.Add("library")));
        using var app = builder.Build();
        prismBuilder.OnCreateWindow();
        prismBuilder.OnCreateWindow();

        Assert.Equal(new[] { "initialized", "library", "window", "window" }, calls);
    }

    [Fact]
    public void CreateWindowRetainsLastConfiguredCallback()
    {
        var calls = new List<string>();
        PrismAppBuilder prismBuilder = null;
        var builder = MauiApp.CreateBuilder().UseMauiApp<Application>().UsePrism(prism =>
        {
            prismBuilder = prism;
            prism.CreateWindow((_, _) =>
            {
                calls.Add("first");
                return Task.CompletedTask;
            });
        });
        builder.ConfigurePrism(prism => prism.CreateWindow((_, _) =>
        {
            calls.Add("last");
            return Task.CompletedTask;
        }));
        using var app = builder.Build();
        prismBuilder.OnCreateWindow();
        Assert.Equal(new[] { "last" }, calls);
    }

    [Fact]
    public void SeparateBuildersBuildWithTheirOwnContainers()
    {
        var firstContainer = new DryIocContainerExtension();
        var secondContainer = new DryIocContainerExtension();
        PrismAppBuilder firstPrism = null;
        PrismAppBuilder secondPrism = null;
        var first = MauiApp.CreateBuilder().UsePrism(firstContainer, prism => firstPrism = prism);
        var second = MauiApp.CreateBuilder().UsePrism(secondContainer, prism => secondPrism = prism);
        using var firstApp = first.Build();
        using var secondApp = second.Build();

        Assert.NotSame(firstPrism, secondPrism);
        Assert.Same(firstPrism, firstApp.Services.GetService(typeof(PrismAppBuilder)));
        Assert.Same(secondPrism, secondApp.Services.GetService(typeof(PrismAppBuilder)));
    }
}
