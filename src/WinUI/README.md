# Native WinUI 3

`Prism.WinUI` and `Prism.DryIoc.WinUI` support native Windows App SDK applications.
They share Prism's region navigation, view-model location, dialogs and modularity
implementation without depending on Uno packages or a hosted application builder.

The application follows the same pattern as Prism's Uno implementation:

```csharp
public partial class App : PrismApplication
{
    public App() => InitializeComponent();

    protected override UIElement CreateShell() => Container.Resolve<Shell>();

    protected override void ConfigureWindow(Window window) =>
        window.Title = "My application";

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
    }

    protected override void OnInitialized() =>
        RegionManager.RequestNavigate("ContentRegion", nameof(HomeView));
}
```

Use `CreateWindow()` to supply a native `Microsoft.UI.Xaml.Window` when needed,
and `ConfigureWindow(Window)` to configure it before its shell is attached.
The application container resolves views and Prism services directly. Applications
do not need a Prism window manager or a window-context interface.

`CreateShell()` returns the XAML shell. Prism initializes its regions and modules
before `OnInitialized()`. Shells implementing `ILoadableShell` receive
`FinishLoading()` after initialization, allowing a loading overlay to be dismissed.
Dialogs use the application's native window content for their `XamlRoot`.

## Build and exercise

Use Windows with the .NET 10 SDK and WinUI/Windows SDK build tools:

```powershell
dotnet build src/WinUI/Prism.DryIoc.WinUI/Prism.DryIoc.WinUI.csproj -c Debug -p:Platform=x64
```

The interactive playground is `e2e/WinUI/HelloWorld.slnx`. The automated desktop
smoke application is `tests/WinUI/NativeWinUISmoke`. Neither can provide native
runtime verification on macOS or Linux. See each application's README for its
run command and coverage.
