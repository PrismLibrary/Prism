# Prism Uno playground (e2e)

Single-project Uno sample: **regions** (`NavigationView` + nested `ContentControl`), **dialogs**, **modularity** (`Playground.Module`), **DryIoc**, **`ILoadableShell`** / splash, and **`InvokeCommandAction`** on Home.

The pages preserve their content's natural dimensions and allow horizontal and
vertical scrolling when the available space is smaller. In portrait and
landscape, expand and collapse the navigation pane, then scroll the page to
reach its complete button labels and content. Existing page maximum widths,
font sizes, and binding scenarios stay the same.

## Build from repo root

Prism packages are **project references**; builds compile `src/Uno` as needed.

**Skia / workloads:** This sample omits `SkiaRenderer` in `UnoFeatures` so `dotnet workload restore` succeeds on Android/iOS heads (Uno `ReplaceUnoRuntime` / `RuntimeAssetsSelectorTask` issue when Skia is forced for those TFMs).

### `dotnet build` (CLI-friendly TFMs)

```powershell
dotnet build .\e2e\Uno\HelloWorld\HelloWorld.csproj -f net10.0-desktop
dotnet build .\e2e\Uno\HelloWorld\HelloWorld.csproj -f net10.0-browserwasm
```

### WinUI (`net10.0-windows10.0.*`)

Uno blocks **`dotnet build`** for WinUI class libraries that contain XAML (error **UNOB0008**). Build the **Windows** TFM with **Visual Studio** or **`msbuild`** (same as CI for `Prism.Uno`):

```powershell
msbuild .\e2e\Uno\HelloWorld\HelloWorld.csproj -restore -p:TargetFramework=net10.0-windows10.0.26100
```

### Full solution file

`HelloWorld.slnx` includes Prism library projects. Do **not** pass a single `-f` to the whole solution: that forces every project (including `Prism.Core`) onto one TFM. Prefer building **`HelloWorld.csproj`** per TFM as above.

## Getting started (Uno)

https://aka.platform.uno/get-started

https://aka.platform.uno/using-uno-sdk

## Local NativeAOT binding validation

Open **NativeAOT binding lab** from the existing navigation menu. This uses the
app's real container, named region navigation, ordinary `{Binding}` paths, and
an independent closed generic INPC model base (`NotifyingContext<string>`) with
inherited title/visibility properties.

- **Run binding checks** must report `PASS: 15 rendered binding checks`. It reads
  rendered title/visibility and collection-template controls after initial
  binding, inherited property notifications, context replacement/old-model
  detachment, null/restore, and collection item update/add/remove.
- Choose **Navigate to named detail**. A new lab page must appear and its status
  must identify `BindingLabDetail`. Run the checks there, then **Back**. The
  journal must return to the original lab with its restored context. The view-model
  accepts reuse only for its original named route, so detail and root retain
  independent contexts. Repeat the navigation.
- Manual buttons allow each mutation to be inspected independently. Replace /
  restore recovers after setting the context to null. The existing Home, Region,
  Dialog, and Module pages remain accessible.

`PrismNativeAotValidation=true` opts the Release Android/iOS **app project** into
`PublishAot=true` and `UseInterpreter=false`; ordinary builds are unchanged.
Validation selects the actual `Prism.Container.Microsoft` adapter via
`CreateContainerExtension` on the container-neutral `PrismApplicationBase`. The
validation build references Prism.Uno directly and does not reference the DryIoc
adapter. Central package configuration pins Microsoft and Abstractions, including
its shared analyzer in referenced source libraries, to published **10.0.94-pre**.
Restore from a configured Prism package feed containing this version. No version
override or local container build is needed for this baseline. The optional
`PrismNativeAotContainerVersion` property selects matching real local packages
when testing further unpublished container changes. Do not substitute a container
shim. Resolve package/framework compatibility before native publishing. Do
not pass `PublishAot` globally, since that propagates to referenced libraries. NativeAOT qualification requires a supported SDK/platform/RID and a
verified NativeAOT-compatible real container configuration, followed by a local
publish and device run. Record exact versions and every trimming/AOT warning.
Cloud source/XML checks are not a NativeAOT or device pass. No mobile CI jobs or
workflow changes are included.

For a supported Uno iOS NativeAOT toolchain:

```sh
dotnet publish e2e/Uno/HelloWorld/HelloWorld.csproj \
  -c Release -f net10.0-ios -r ios-arm64 \
  -p:PrismNativeAotValidation=true
```

[Uno 6.6 NativeAOT guidance](https://platform.uno/docs/articles/features/native-aot.html)
includes .NET 10 Android and iOS. Android needs NDK r27 or newer and emits the
experimental XA1040 warning. For Android, use the same command with
`-f net10.0-android -r android-arm64` and inspect the warning and device results.
Never count an ordinary Android AOT build as NativeAOT validation.

For optional unpublished source-package preparation, follow the
[real local container package commands](../../Maui/PrismMauiDemo/NativeAotValidation.md#build-the-real-local-container-packages).
Use both projects' normal supported TFMs, then add
`-p:RestoreAdditionalProjectSources="$PRISM_CONTAINER_FEED"` to the publish
command together with `-p:PrismNativeAotContainerVersion="$PRISM_CONTAINER_VERSION"`. The net10-only package used for cloud generator/core unit tests does
not establish a complete multi-target mobile restore or native execution pass.

The nested action checks bind `Context.ActionButton.Text`, `.Icon`, and `.Command`
and computed `HasActionButton`/`HasTitle` flags. They inspect actual controls after
nested notifications, replacement/detachment, null and restoration, then invoke
the command obtained from the bound control and check the rendered title. The
nested action derives from `BindableBase` without an application-side attribute;
the context still tests the independent-INPC path. Generated property preservation
alone is not proof that Uno generated accessors for every independent INPC type.

The publish commands deliberately use the physical-device `ios-arm64` RID.
Do not infer simulator NativeAOT support from general platform documentation;
check the exact installed SDK's support before selecting another RID. An earlier
native publish, a later non-native simulator login, or an existing generated
metadata file cannot qualify the current source changes. Capture a fresh native
publish and these rendered checks on that exact resulting binary.
