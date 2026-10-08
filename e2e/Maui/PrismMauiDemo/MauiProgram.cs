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
#if PRISM_NATIVE_AOT_VALIDATION
            .UsePrism(new Prism.Container.Microsoft.MicrosoftContainerExtension(), prism =>
#else
            .UsePrism(prism =>
#endif
                prism.ConfigureModuleCatalog(moduleCatalog =>
                {
                    moduleCatalog.AddModule<MauiAppModule>();
                    moduleCatalog.AddModule<MauiTestRegionsModule>();
                })
                .RegisterTypes(containerRegistry =>
                {
                    containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
                    containerRegistry.RegisterForNavigation<BindingLabPage, BindingLabViewModel>("BindingLab");
                    containerRegistry.RegisterForNavigation<BindingLabPage, BindingLabViewModel>("BindingLabDetail");
                    containerRegistry.RegisterForNavigation<RootPage, RootPageViewModel>();
                    containerRegistry.RegisterForNavigation<GlobalNamespacesPage>();
                    containerRegistry.RegisterForNavigation<SamplePage>();
                    containerRegistry.RegisterForNavigation<SplashPage, SplashPageViewModel>();
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
#if PRISM_NATIVE_AOT_VALIDATION
                .CreateWindow(navigationService => Environment.GetEnvironmentVariable("PRISM_NATIVEAOT_AUTORUN") == "1"
                    ? navigationService.NavigateAsync("/NavigationPage/BindingLab")
                    : navigationService.CreateBuilder()
#else
                .CreateWindow(navigationService => navigationService.CreateBuilder()
#endif
                    .AddSegment<SplashPageViewModel>()
                //.CreateWindow(nav => nav.CreateBuilder()
                //    .AddTabbedSegment(page =>
                //        page.CreateTab(t => t.AddNavigationPage().AddSegment("ViewA").AddSegment("ViewB"))
                //        .CreateTab(t => t.AddNavigationPage().AddSegment("ViewA"))
                //    )
                    .NavigateAsync(HandleNavigationError))
            )
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
