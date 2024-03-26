using Microsoft.Extensions.Logging;
using Prism.DryIoc.Maui.Tests.Mocks;
using Prism.DryIoc.Maui.Tests.Mocks.Logging;
using Prism.DryIoc.Maui.Tests.Mocks.Navigation;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Events;

namespace Prism.DryIoc.Maui.Tests.Fixtures;

public abstract class TestBase
{
    protected readonly ITestOutputHelper _testOutputHelper;

    protected TestBase(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
    }

    protected MauiAppBuilder CreateBuilder(Action<PrismAppBuilder> configurePrism)
    {
        return MauiApp.CreateBuilder()
            .UseMauiApp<Application>()
            .UsePrism(prism =>
            {
                prism.RegisterTypes(container =>
                {
                    container.RegisterScoped<INavigationService, TestPageNavigationService>();
                    container.RegisterForNavigation<MockHome, MockHomeViewModel>()
                        .RegisterForNavigation<MockViewA, MockViewAViewModel>()
                        .RegisterForNavigation<MockViewB, MockViewBViewModel>()
                        .RegisterForNavigation<MockViewC, MockViewCViewModel>()
                        .RegisterForNavigation<MockViewD, MockViewDViewModel>()
                        .RegisterForNavigation<MockViewE, MockViewEViewModel>();
                })
                .ConfigureLogging(builder =>
                    builder.AddProvider(new XUnitLoggerProvider(_testOutputHelper)))
                .OnInitialized(container =>
                {
                    var ea = container.Resolve<IEventAggregator>();
                    ea.GetEvent<NavigationRequestEvent>().Subscribe(context =>
                    {
                        if (System.Diagnostics.Debugger.IsAttached)
                            System.Diagnostics.Debugger.Break();

                        var logger = container.Resolve<ILoggerFactory>()
                            .CreateLogger(GetType().Name);
                        var message = context.Type == NavigationRequestType.Navigate ? $"{context.Type}: {context.Uri}" : $"{context.Type}";

                        message += context.Cancelled ? " - Cancelled" : context.Result.Exception is null ? " - Success" : " - Error";
                        logger.LogInformation(message);
                        if (!context.Cancelled && context.Result.Exception is not null)
                        {
                            var ex = context.Result.Exception;
                            while(ex is not null)
                            {
                                logger.LogError(ex, "Navigation Error");
                                ex = ex.InnerException;
                            }
                        }
                    });
                });
                configurePrism(prism);
            });
    }

    protected static PrismWindow GetWindow(MauiApp mauiApp)
    {
        var app = mauiApp.Services.GetService<IApplication>();
        Assert.NotNull(app);
        Assert.IsType<Application>(app);

        var state = new ActivationState(new MauiContext(mauiApp.Services));
        var window = app.CreateWindow(state);
        Assert.IsType<PrismWindow>(window);
        var prismWindow = window as PrismWindow;
        Assert.NotNull(prismWindow?.Page);
        return prismWindow;
    }
}
