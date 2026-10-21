# Prism native WinUI playground (e2e)

This is the native Windows App SDK counterpart to the repository's other
`HelloWorld` exercise applications. Open it and interact with the playground to
try Prism APIs. It is not an automated smoke-test runner.

The application uses `Microsoft.NET.Sdk`, native WinUI controls, and project
references to `Prism.WinUI` and `Prism.DryIoc.WinUI`. The Windows App SDK and
behaviors package versions come from the repository's central package file.
No Uno SDK, toolkit, hosting, or runtime packages are used.

`HelloWorld` contains the application bootstrap and shell. `Playground.Module`
provides the views, view models, and dialogs, and is registered through
`App.ConfigureModuleCatalog`. Startup resolves the shell, loads the module,
and navigates to Home through its window context. Each additional window gets
its own shell, region manager, navigation journal, and scoped services; module
initialization remains application-wide. A native `ProgressRing` overlay implements
`ILoadableShell`; it disappears after Prism initialization.

## Build and run on Windows

Use a Windows development machine with the .NET 10 SDK and the Windows App SDK
WinUI build tools. Restore must have access to the repository's configured
Prism container package feed, including the centrally pinned
`Prism.Container.Abstractions` and `Prism.Container.DryIoc` packages.

Open `e2e/WinUI/HelloWorld.slnx` in Visual Studio, select `HelloWorld` as the
startup project, and choose x64 or ARM64. The app runs unpackaged and carries
the Windows App SDK runtime with it.

From a Visual Studio developer command prompt at the repository root:

```powershell
msbuild .\e2e\WinUI\HelloWorld\HelloWorld.csproj -restore -p:Configuration=Debug -p:Platform=x64 -p:RuntimeIdentifier=win-x64
```

For ARM64, use `-p:Platform=ARM64 -p:RuntimeIdentifier=win-arm64` instead.
Launch the resulting `HelloWorld.exe` from the app's build output, or use
Visual Studio's debugger. Do not pass a Windows target framework globally to
the solution because the core projects target their own frameworks.

## Manual exercises

- Home: use **Go to region lab (InvokeCommandAction)** to exercise the behaviors
  trigger and Prism command action rather than a direct button command binding.
- Region lab: open **Alpha detail**, then **Beta detail**, then **Back in detail
  region**. The nested `ContentControl` region should return to Alpha through
  its own navigation journal.
- Main navigation: visit Region lab, Dialog lab, and Module through the
  `NavigationView`. Use the shell back button to walk the main `ContentRegion`
  journal independently of the nested detail journal.
- Dialog lab: show the notification and inspect its supplied title/message.
  Open the confirmation dialog twice, choosing OK and Cancel; the page should
  display the corresponding `ButtonResult` each time.
- Module: inspect the module-provided page and its explanation of module
  registration. All lab views and dialogs are registered in `PlaygroundModule`.
- Window lab: use **Open another playground window** in the navigation pane footer.
  Navigate both windows through Region lab, opening different details. Both use
  the same region names but must retain independent content and journals.
- In Window lab, refresh the target list and select the other window.
  **Activate target, then show dialog in this window** must keep the dialog in
  the originating window, despite activation. **Show dialog in selected window**
  must deliberately show it in the selected target instead.
- Close the selected target and try it again without refreshing. An error should
  be reported instead of routing to another window. Refresh to remove closed
  targets. Closing one window must leave the other usable. All windows in this
  exercise use the original WinUI UI thread.
- Resize the window and expand/collapse the navigation pane. Lab content should
  remain reachable through the pages' horizontal and vertical scroll viewers.

The separate automated harness lives at `tests/WinUI/NativeWinUISmoke`.
A successful source or project-file check on another OS does not establish
native XAML compilation or any of the Windows UI results above.
