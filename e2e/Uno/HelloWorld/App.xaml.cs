using HelloWorld.Views;
using HelloWorld.ViewModels;
using Playground.Module;
using Serilog;
using Uno.UI;

namespace HelloWorld;

public partial class App : PlaygroundApplication
{
    public App()
    {
        InitializeComponent();
    }

#if PRISM_NATIVE_AOT_VALIDATION
    protected override IContainerExtension CreateContainerExtension() =>
        new Prism.Container.Microsoft.MicrosoftContainerExtension();
#endif

    protected override UIElement CreateShell()
    {
        return Container.Resolve<Shell>();
    }

    protected override void ConfigureHost(IHostBuilder builder)
    {
        builder
#if DEBUG
            .UseEnvironment(Environments.Development)
#endif
            .UseLogging(configure: (context, logBuilder) =>
            {
                logBuilder.SetMinimumLevel(
                    context.HostingEnvironment.IsDevelopment() ?
                        LogLevel.Information :
                        LogLevel.Warning);
            }, enableUnoLogging: true)
            .ConfigureLogging((context, logging) => logging.AddSerilog(
                new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File(Path.Combine(context.HostingEnvironment.GetAppDataPath(), "HelloWorld.log"))
                    .CreateLogger(), dispose: true))
            .UseSerialization()
            .ConfigureServices((context, services) =>
            {
            });
    }

    protected override void ConfigureWindow(Window window)
    {
#if DEBUG
        window.UseStudio();
#endif
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<BindingLabPage, BindingLabViewModel>("BindingLab");
        containerRegistry.RegisterForNavigation<BindingLabPage, BindingLabViewModel>("BindingLabDetail");
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<PlaygroundModule>();
    }
}
