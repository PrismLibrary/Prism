# Local NativeAOT binding validation

This is a page in the existing MAUI demo, not a separate smoke application. From
Root Page, open **NativeAOT binding lab**. It uses named Prism navigation and the
app's real container. The binding controls intentionally use ordinary `{Binding}`
rather than compiled bindings to exercise generated member preservation.

## UI checks

1. Open the lab and choose **Run binding checks**. The status must read
   `PASS: 15 rendered binding checks`.
2. The checks read the actual page title, navigation-bar attached property,
   labels, visibility, and realized collection templates. They cover initial
   values; inherited INPC title/visibility changes; replacing the context and
   detaching the old model; null and restored context; and collection item
   changes, additions, and removals. The context inherits from an independent
   INPC base, not `BindableBase`.
3. Choose **Navigate to named detail**. A new lab page must open with initial
   context and `Arrived through Prism navigation`. Run its checks, then **Back**.
   The first page and its restored context must return. Repeat the navigation.
4. Exercise the manual update, replace/restore, null, visibility, and collection
   buttons. **Home** returns to the existing demo menu, even with the navigation
   bar hidden.

## NativeAOT qualification

Set `PrismNativeAotValidation=true` on a Release Android/iOS app build. The app
project maps this opt-in to `PublishAot=true` and `UseInterpreter=false`. Do not
pass `PublishAot` globally: that also reaches referenced libraries.
The opt-in does not set `PublishAot` on libraries. Ordinary sample builds keep
DryIoc. Validation builds select the real `Prism.Container.Microsoft` adapter;
the central package configuration pins Containers **10.0.94-pre**, including
Abstractions and its shared generator in referenced source libraries.
Restore from a configured Prism package feed that contains this published version.
No version override or local container build is needed for this baseline.
`PrismNativeAotContainerVersion` remains available for deliberate matching local
Microsoft/Abstractions package builds; see the optional instructions below.
Do not use stand-in containers or copy implementation types into the demo.

For a supported iOS device toolchain, publish the existing app from the repo root:

```sh
dotnet publish e2e/Maui/PrismMauiDemo/PrismMauiDemo.csproj \
  -c Release -f net10.0-ios -r ios-arm64 \
  -p:PrismNativeAotValidation=true
```

The [MAUI NativeAOT requirements](https://learn.microsoft.com/en-us/dotnet/maui/deployment/nativeaot?view=net-maui-10.0)
require compiled bindings. This lab deliberately tests ordinary bindings with
Prism's preserved metadata; it is an experimental regression test, not a claim
that metadata preservation removes framework limitations. Report any framework
failure rather than replacing these controls with compiled bindings.

Select an Android TFM/RID only after verifying that the installed platform/MAUI
SDK supports NativeAOT for it. Do not substitute an ordinary Android AOT build.

Run on a connected Mac/device with the required SDK, workloads, platform tools,
and a verified NativeAOT-compatible real container configuration. A successful
Debug build, source check, or run on a different runtime does not qualify this
scenario. Record the exact SDK, TFM, RID, container package/version, linker/AOT
warnings, and device results. Do not suppress warnings to obtain a pass.

Mobile publish/device execution has not been validated in the cloud workspace.
No mobile CI jobs or workflow changes are part of this local validation work.

## Build the real local container packages

Optional: only needed when testing further unpublished container changes.

Use a full checkout of `PrismLibrary/Prism.Containers` on
the intended source commit (the published baseline is merge `f3e51fbb`), with the SDKs required by that checkout's
normal supported target frameworks. Do not manufacture netstandard targets or
replace the container with a shim. From that checkout, choose one local-only
prerelease version for both existing projects:

```sh
export PRISM_CONTAINER_VERSION=10.0.95-bindingfixlocal
export PRISM_CONTAINER_FEED="$HOME/prism-native-aot-packages"
dotnet pack src/Prism.Container.Abstractions/Prism.Container.Abstractions.csproj \
  -c Release -p:DISABLE_GITVERSIONING=true \
  -p:PackageVersion="$PRISM_CONTAINER_VERSION" -o "$PRISM_CONTAINER_FEED"
dotnet pack src/Prism.Container.Microsoft/Prism.Container.Microsoft.csproj \
  -c Release -p:DISABLE_GITVERSIONING=true \
  -p:PackageVersion="$PRISM_CONTAINER_VERSION" -o "$PRISM_CONTAINER_FEED"
```

These are unpublished local packages. Pack the projects' normal supported TFMs:
a net10-only package was sufficient for the cloud core/generator unit tests, but
is not sufficient for an unrestricted multi-target application restore. In the
Prism checkout, add
`-p:RestoreAdditionalProjectSources="$PRISM_CONTAINER_FEED"` and
`-p:PrismNativeAotContainerVersion="$PRISM_CONTAINER_VERSION"` to the mobile
publish command above. Both native-validation applications reference the
container-neutral Prism framework directly; no DryIoc adapter is selected or
required by that validation path. The complete mobile package build/restore and
device run remain to be verified on the supported local toolchain.

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
