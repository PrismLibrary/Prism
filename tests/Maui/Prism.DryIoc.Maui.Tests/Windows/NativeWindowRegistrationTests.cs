#if WINDOWS
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using NativeWindow = Microsoft.UI.Xaml.Window;

namespace Prism.DryIoc.Maui.Tests.Windows;

public class NativeWindowRegistrationTests
{
    [Fact]
    public void ApplicationContextDoesNotAutoConstructANativeWindow()
    {
        // Disable MAUI's OS initialization; exercise the real Prism builder and container.
        using var app = MauiApp.CreateBuilder(useDefaults: false).UsePrism(_ => { }).Build();
        var context = new MauiContext(app.Services);

        Assert.Null(context.Services.GetService<NativeWindow>());
    }

    [Fact]
    public void ExistingNativeWindowRegistrationIsPreserved()
    {
        var builder = MauiApp.CreateBuilder(useDefaults: false);
        var resolutions = 0;
        builder.Services.AddSingleton<NativeWindow>(_ =>
        {
            resolutions++;
            return null;
        });
        builder.UsePrism(_ => { });
        using var app = builder.Build();

        Assert.Null(app.Services.GetService<NativeWindow>());
        Assert.Equal(1, resolutions);
    }
}
#endif
