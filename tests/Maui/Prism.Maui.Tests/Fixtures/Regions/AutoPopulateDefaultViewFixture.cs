using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Moq;
using Prism.Ioc;
using Prism.Maui.Tests.Mocks;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Prism.Navigation.Xaml;
using Xunit;
using Region = Prism.Navigation.Regions.Region;
using RegionManager = Prism.Navigation.Regions.Xaml.RegionManager;

namespace Prism.Maui.Tests.Fixtures.Regions;

public class AutoPopulateDefaultViewFixture
{
    private const string RegionName = "MainRegion";
    private const string DefaultViewName = "DefaultRegionView";

    public AutoPopulateDefaultViewFixture() => DispatcherProvider.SetCurrent(TestDispatcher.Provider);

    [Theory]
    [InlineData("name")]
    [InlineData("type")]
    [InlineData("instance")]
    public void DefaultViewPropertyPreservesEachSupportedForm(string form)
    {
        var host = new ContentView();
        var value = DefaultValue(form);

        RegionManager.SetDefaultView(host, value);

        Assert.Same(value, RegionManager.GetDefaultView(host));
    }

    [Theory]
    [InlineData("name")]
    [InlineData("type")]
    public void DefaultViewUsesTheRegionsContainerAndNavigationRegistration(string form)
    {
        var test = new TestRegion(DefaultValue(form));
        var view = new DefaultRegionView();
        test.NavigationRegistry.Setup(r => r.CreateView(test.Scope.Object, DefaultViewName)).Returns(view);

        test.Behavior.Attach();

        Assert.Same(view, Assert.Single(test.Region.Views));
        test.Scope.Verify(c => c.Resolve(typeof(IRegionNavigationRegistry)), Times.Once);
        test.NavigationRegistry.Verify(r => r.CreateView(test.Scope.Object, DefaultViewName), Times.Once);
        if (form == "name")
            Assert.Same(view, test.Region.GetView(DefaultViewName));
    }

    [Fact]
    public void InstanceDefaultIsAddedWithoutContainerResolution()
    {
        var view = new DefaultRegionView();
        var test = new TestRegion(view);

        test.Behavior.Attach();

        Assert.Same(view, Assert.Single(test.Region.Views));
        test.Scope.VerifyNoOtherCalls();
        test.NavigationRegistry.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("name", true)]
    [InlineData("type", true)]
    [InlineData("instance", true)]
    [InlineData("name", false)]
    [InlineData("type", false)]
    [InlineData("instance", false)]
    public void RepeatedAttachPopulatesAndSubscribesOnlyOnce(string form, bool namedBeforeAttach)
    {
        var test = new TestRegion(DefaultValue(form), namedBeforeAttach: namedBeforeAttach);

        test.Behavior.Attach();
        test.Behavior.Attach();
        if (!namedBeforeAttach)
        {
            Assert.Empty(test.Region.Views);
            test.Scope.VerifyNoOtherCalls();
            test.Region.Name = RegionName;
        }
        test.Behavior.Attach();
        // Raising Name again must not start another population pass.
        test.Region.Name = RegionName;

        Assert.Single(test.Region.Views);
        var discovered = new ContentView();
        var discoveryCalls = 0;
        test.Discovery.RegisterViewWithRegion(RegionName, _ =>
        {
            discoveryCalls++;
            return discovered;
        });
        Assert.Equal(1, discoveryCalls);
        Assert.Equal(2, test.Region.Views.Count());
        Assert.Contains(discovered, test.Region.Views);
        if (form != "instance")
            test.NavigationRegistry.Verify(r => r.CreateView(test.Scope.Object, DefaultViewName), Times.Once);
    }

    [Fact]
    public void TypeDefaultWithoutRegionNavigationRegistrationThrows()
    {
        var test = new TestRegion(typeof(DefaultRegionView));
        // A page registration of the same type must not satisfy a region default.
        test.NavigationRegistry.SetupGet(r => r.Registrations).Returns(new[]
        {
            new ViewRegistration { Type = ViewType.Page, View = typeof(DefaultRegionView), Name = DefaultViewName }
        });

        var error = Assert.Throws<KeyNotFoundException>(() => test.Behavior.Attach());

        Assert.Contains(typeof(DefaultRegionView).FullName, error.Message);
        Assert.Empty(test.Region.Views);
        test.NavigationRegistry.Verify(r => r.CreateView(It.IsAny<IContainerProvider>(), It.IsAny<string>()), Times.Never);
        Assert.Empty(test.Discovery.GetContents(RegionName, test.Scope.Object));
    }

    [Fact]
    public void ExistingInstanceIsNotAddedAgainAsDefault()
    {
        var view = new DefaultRegionView();
        var test = new TestRegion(view);
        test.Discovery.RegisterViewWithRegion(RegionName, _ => view);

        test.Behavior.Attach();

        Assert.Same(view, Assert.Single(test.Region.Views));
    }

    [Fact]
    public void TargetElementIsUsedWhenBehaviorHasNoExplicitHost()
    {
        var view = new DefaultRegionView();
        var test = new TestRegion(view);
        test.Behavior.HostControl = null;

        test.Behavior.Attach();

        Assert.Same(view, Assert.Single(test.Region.Views));
    }

    [Fact]
    public void ExplicitHostTakesPrecedenceOverTargetElement()
    {
        var targetDefault = new DefaultRegionView();
        var hostDefault = new DefaultRegionView();
        var test = new TestRegion(targetDefault);
        var otherHost = new ContentView();
        RegionManager.SetDefaultView(otherHost, hostDefault);
        test.Behavior.HostControl = otherHost;

        test.Behavior.Attach();

        Assert.Same(hostDefault, Assert.Single(test.Region.Views));
    }

    [Fact]
    public void HostCannotChangeAfterAttach()
    {
        var test = new TestRegion(new DefaultRegionView(), namedBeforeAttach: false);
        test.Behavior.Attach();

        Assert.Throws<InvalidOperationException>(() => test.Behavior.HostControl = new ContentView());
    }

    [Fact]
    public void ChangingDefaultAfterPopulationDoesNotReplaceOrAddAView()
    {
        var initial = new DefaultRegionView();
        var test = new TestRegion(initial);
        test.Behavior.Attach();

        RegionManager.SetDefaultView(test.Host, new DefaultRegionView());
        test.Behavior.Attach();

        Assert.Same(initial, Assert.Single(test.Region.Views));
    }

    [Theory]
    [InlineData("name")]
    [InlineData("type")]
    [InlineData("instance")]
    public void RecreatedHostsKeepDefaultsLocalAndLeaveSharedDiscoveryUnchanged(string form)
    {
        var discovery = new RegionViewRegistry();
        discovery.RegisterViewWithRegion(RegionName, _ => new ContentView());
        var registrations = 0;
        EventHandler<ViewRegisteredEventArgs> registered = (_, _) => registrations++;
        discovery.ContentRegistered += registered;
        var first = new TestRegion(DefaultValue(form), discovery);
        var second = new TestRegion(DefaultValue(form), discovery);
        var withoutDefault = new TestRegion(null, discovery);

        first.Behavior.Attach();
        var firstDefault = Assert.Single(first.Region.Views.OfType<DefaultRegionView>());
        second.Behavior.Attach();
        var secondDefault = Assert.Single(second.Region.Views.OfType<DefaultRegionView>());
        withoutDefault.Behavior.Attach();

        Assert.NotSame(firstDefault, secondDefault);
        Assert.Equal(2, first.Region.Views.Count());
        Assert.Equal(2, second.Region.Views.Count());
        Assert.IsType<ContentView>(Assert.Single(withoutDefault.Region.Views));
        Assert.IsType<ContentView>(Assert.Single(discovery.GetContents(RegionName, first.Scope.Object)));
        Assert.Equal(0, registrations);
        if (form != "instance")
        {
            first.NavigationRegistry.Verify(r => r.CreateView(first.Scope.Object, DefaultViewName), Times.Once);
            second.NavigationRegistry.Verify(r => r.CreateView(second.Scope.Object, DefaultViewName), Times.Once);
        }
        GC.KeepAlive(registered);
    }

    [Theory]
    [InlineData("name")]
    [InlineData("type")]
    [InlineData("instance")]
    public void SharedDiscoveryDoesNotRetainDiscardedHostRegionBehaviorOrDefault(string form)
    {
        var discovery = new RegionViewRegistry();
        var references = CreateDiscardedRegion(discovery, form);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.All(references, reference => Assert.False(reference.IsAlive));
        Assert.Empty(discovery.GetContents(RegionName, Mock.Of<IContainerProvider>()));
        GC.KeepAlive(discovery);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] CreateDiscardedRegion(RegionViewRegistry discovery, string form)
    {
        var test = new TestRegion(DefaultValue(form), discovery);
        test.Behavior.Attach();
        var view = Assert.Single(test.Region.Views);
        return new[]
        {
            new WeakReference(test.Host),
            new WeakReference(test.Region),
            new WeakReference(test.Behavior),
            new WeakReference(view)
        };
    }

    private static object DefaultValue(string form) => form switch
    {
        "name" => DefaultViewName,
        "type" => typeof(DefaultRegionView),
        "instance" => new DefaultRegionView(),
        _ => throw new ArgumentOutOfRangeException(nameof(form))
    };

    private sealed class DefaultRegionView : ContentView { }

    private sealed class TestRegion
    {
        public ContentView Host { get; } = new();
        public Mock<IContainerProvider> Scope { get; } = new(MockBehavior.Strict);
        public Mock<IRegionNavigationRegistry> NavigationRegistry { get; } = new(MockBehavior.Strict);
        public RegionViewRegistry Discovery { get; }
        public Region Region { get; }
        public AutoPopulateRegionBehavior Behavior { get; }

        public TestRegion(object defaultView, RegionViewRegistry discovery = null, bool namedBeforeAttach = true)
        {
            Discovery = discovery ?? new RegionViewRegistry();
            Scope.Setup(c => c.Resolve(typeof(IRegionNavigationRegistry))).Returns(NavigationRegistry.Object);
            NavigationRegistry.SetupGet(r => r.Registrations).Returns(new[]
            {
                new ViewRegistration { Type = ViewType.Region, View = typeof(DefaultRegionView), Name = DefaultViewName }
            });
            NavigationRegistry.Setup(r => r.CreateView(Scope.Object, DefaultViewName)).Returns(() => new DefaultRegionView());
            Host.SetContainerProvider(Scope.Object);
            RegionManager.SetDefaultView(Host, defaultView);
            Region = new Region(Mock.Of<IRegionNavigationService>()) { TargetElement = Host };
            if (namedBeforeAttach)
                Region.Name = RegionName;
            Behavior = new AutoPopulateRegionBehavior(Discovery) { Region = Region, HostControl = Host };
        }
    }
}
