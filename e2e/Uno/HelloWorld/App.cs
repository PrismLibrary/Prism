using HelloWorld.Views;
using ModuleA;
using Uno.UI;

namespace HelloWorld;

public class App : PrismApplication
{
    protected override UIElement CreateShell()
    {
        return Container.Resolve<Shell>();
    }

    protected override void ConfigureHost(IHostBuilder builder)
    {
        builder
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder.SetMinimumLevel(
                        context.HostingEnvironment.IsDevelopment() ?
                            LogLevel.Information :
                            LogLevel.Warning);
                }, enableUnoLogging: true)
                .UseSerilog(consoleLoggingEnabled: true, fileLoggingEnabled: true)
                // Register Json serializers (ISerializer and ISerializer)
                .UseSerialization()
                .ConfigureServices((context, services) =>
                {
                    // TODO: Register your services
                    //services.AddSingleton<IMyService, MyService>();
                });
    }

    protected override void ConfigureWindow(Window window)
    {
#if DEBUG
        window.EnableHotReload();
#endif
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register types with the container or for Navigation
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<ModuleAModule>();
    }
}
