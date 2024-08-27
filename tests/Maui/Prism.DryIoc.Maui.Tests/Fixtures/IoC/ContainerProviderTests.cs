using Prism.DryIoc.Maui.Tests.Mocks;
using Prism.DryIoc.Maui.Tests.Mocks.Events;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Events;
using Prism.IoC;

namespace Prism.DryIoc.Maui.Tests.Fixtures.IoC;

public class ContainerProviderTests(ITestOutputHelper testOutputHelper) : TestBase(testOutputHelper)
{
    [Fact]
    public void CanResolveUnamedType()
    {
        var builder = CreateBuilder(prism => { });
        var app = builder.Build();
        var containerProvider = new ContainerProvider<ConcreteTypeMock>();

        ConcreteTypeMock type = (ConcreteTypeMock)containerProvider;

        Assert.NotNull(type);
        Assert.IsType<ConcreteTypeMock>(type);
    }

    [Fact]
    public void CanResolvedNamedType()
    {
        var builder = CreateBuilder(prism => { });
        var app = builder.Build();
        var containerProvider = new ContainerProvider<ConcreteTypeMock>
        {
            Name = ConcreteTypeMock.Key
        };

        ConcreteTypeMock vm = (ConcreteTypeMock)containerProvider;

        Assert.NotNull(vm);
        Assert.IsType<ConcreteTypeMock>(vm);
    }

    [Fact]
    public async Task ProvidesValueFromResourceDictionary()
    {
        var builder = CreateBuilder(prism =>
        {
            prism.RegisterTypes(containerRegistry =>
            {
                containerRegistry.RegisterForNavigation<MockXamlView, MockXamlViewViewModel>();
            })
            .CreateWindow(n =>
                n.CreateBuilder()
                .AddSegment<MockViewAViewModel>()
                .NavigateAsync());
        });
        var app = builder.Build();

        var ea = app.Services.GetService<IEventAggregator>();
        var events = new List<string>();
        ea.GetEvent<TestActionEvent>().Subscribe((m) => events.Add(m));

        var navigation = app.Services.GetService<INavigationService>();
        await navigation.CreateBuilder()
            .AddSegment<MockXamlViewViewModel>()
            .NavigateAsync();

        Assert.Contains(events, e => e == "Convert");
        var window = GetWindow(app);
        Assert.NotNull(window.Page);

        var xamlView = window.Page as MockXamlView;
        var viewModel = xamlView.BindingContext as MockXamlViewViewModel;

        xamlView.TestEntry.Text = "Foo Bar";
        Assert.Contains(events, e => e == "ConvertBack");
    }
}
