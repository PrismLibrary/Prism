# Prism Sandbox for native WinUI (e2e)

Use the Sandbox to try Prism regions, navigation, dialogs, and modules.

The application uses `Microsoft.NET.Sdk`, native WinUI controls, and project
references to `Prism.WinUI` and `Prism.DryIoc.WinUI`. The Windows App SDK and
behaviors package versions come from the repository's central package file.

`HelloWorld` contains the application bootstrap and XAML shell. `Playground.Module`
provides views, view models, and dialogs through `ConfigureModuleCatalog`.
`CreateShell()` resolves the shell from `Container`; `ConfigureWindow(Window)`
sets its title; `OnInitialized()` navigates through `RegionManager`.
The `ILoadableShell` loading overlay disappears when initialization finishes.

## Build and run on Windows

Use the .NET 10 SDK and Windows App SDK WinUI build tools. Restore needs access
to the configured Prism container package feed.

```powershell
dotnet build e2e/WinUI/HelloWorld/HelloWorld.csproj -c Debug -p:Platform=x64 -r win-x64
```

Alternatively open `e2e/WinUI/HelloWorld.slnx` in Visual Studio and start HelloWorld.

Check the Home command, region navigation and back journal, dialog confirmation
and cancellation, and module-provided views. Repeat navigation and dialogs to
check that the loading overlay remains dismissed and services remain usable.
