using MauiModule;
using MauiModule.ViewModels;
using MauiRegionsModule;
using PrismMauiDemo.ViewModels;
using PrismMauiDemo.Views;

namespace PrismMauiDemo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        return MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .UsePrism(prism => 
                prism.ConfigureModuleCatalog(moduleCatalog =>
                {
                    moduleCatalog.AddModule<MauiAppModule>();
                    moduleCatalog.AddModule<MauiTestRegionsModule>();
                })
                .RegisterTypes(containerRegistry =>
                {
                    containerRegistry.RegisterForNavigation<MainPage>();
                    containerRegistry.RegisterForNavigation<RootPage>();
                    containerRegistry.RegisterForNavigation<SamplePage>();
                    containerRegistry.RegisterForNavigation<SplashPage>();
                })
                .AddGlobalNavigationObserver(context => context.Subscribe(x =>
                {
                    if (x.Type == NavigationRequestType.Navigate)
                        Console.WriteLine($"Navigation: {x.Uri}");
                    else
                        Console.WriteLine($"Navigation: {x.Type}");

                    var status = x.Cancelled ? "Cancelled" : x.Result.Success ? "Success" : "Failed";
                    Console.WriteLine($"Result: {status}");

                    if (status == "Failed" && !string.IsNullOrEmpty(x.Result?.Exception?.Message))
                        Console.Error.WriteLine(x.Result.Exception.Message);
                }))
                //.CreateWindow(nav => nav.CreateBuilder()
                //    .AddTabbedSegment(page =>
                //        page.CreateTab("ViewC")
                //            .CreateTab(t =>
                //                t.AddNavigationPage()
                //                 .AddSegment("ViewA", s => s.AddParameter("message", "Hello Tab - ViewA"))
                //                 .AddSegment("ViewB", s => s.AddParameter("message", "Hello Tab - ViewB")))
                //            //.CreateTab("ViewC", s => s.AddParameter("message", "Hello Tab - ViewC"))
                //            .SelectedTab("NavigationPage|ViewB"))
                //    .AddParameter("message_global", "This is a Global Message")
                //    .Navigate())
                //.CreateWindow("ViewA/ViewB/ViewC")
                .CreateWindow(navigationService => navigationService.CreateBuilder()
                    .AddSegment<SplashPageViewModel>()
                    .NavigateAsync(HandleNavigationError))
            )
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .Build();
    }

    private static void HandleNavigationError(Exception ex)
    {
        Console.WriteLine(ex);
        System.Diagnostics.Debugger.Break();
    }
}
