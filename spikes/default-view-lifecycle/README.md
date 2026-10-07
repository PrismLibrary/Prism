# DefaultView lifecycle spike (#3246)

Experimental branch, based on `040330fe0963fdcc340ad3beed8c60bd9022ca64` (master, 2026-10-07).
This is a design/proof branch, not a production-ready change or release proposal.

## Recommendation

Keep default views local to the region host, inside the existing `AutoPopulateRegionBehavior`.
Use `IHostAwareRegionBehavior`, which the adapters already understand, and a method-local
`RegionViewRegistry` to resolve a string or Type through the existing view-discovery APIs.
A supplied instance can be added directly. No new resolver service, default-view behavior,
registration token, singleton dictionary, or disposal layer is needed.

Tradeoff: constructing the temporary standard registry does not honor a user-supplied
`IRegionViewRegistry` replacement or custom `RegionViewRegistry.CreateInstance` override
for defaults. Normal discovery still uses the injected registry. This spike reuses standard
resolution semantics; supporting custom default-resolution policy needs a separate API decision.

The WPF region implementation is already source-linked by Uno and Avalonia. The new
`RegionManager.DefaultView` property therefore covers all three. MAUI keeps its existing
`Navigation.Regions.Xaml.RegionManager` property and adopts the same population pattern.

## Contract explored

- Accept a registered string name, Type, or instance; null means no default.
- Declare the default before region creation. Read it once, after normal discovered views.
- Populate once per region/behavior lifetime, including repeated `Attach()` before or after naming.
- Keep normal adapter activation semantics. A default does not replace an already active discovered view.
- Preserve newly added string defaults as named region views (`GetView(name)`), including existing MAUI behavior.
- Avoid adding the identical instance twice if discovery has already supplied it. Such an existing
  view keeps its original name metadata; a string default does not rename an unnamed singleton.
- Subsequent property changes or removing a view do not navigate, replace, or repopulate it.
- Recreated hosts with the same region name resolve their own default. Two separate managers
  with the same region name do not inherit each other's default.

The temporary registry uses the platform's existing resolution rules:

| Input | WPF / Uno / Avalonia | MAUI |
|---|---|---|
| Registered string | Navigation registry, with existing legacy named-container fallback | Region navigation registry, using the host's scope |
| Type | Last matching navigation registration; existing container/autowire fallback if unregistered | First matching region registration; registration required |
| Instance | Same supplied instance | Same supplied VisualElement |

MAUI review point: an unregistered Type previously did nothing silently. Reusing the existing
MAUI discovery registry now throws its `KeyNotFoundException`. This intentional fail-fast
change should be explicitly agreed before promoting the spike. Invalid MAUI instances also
fail instead of being silently ignored. Duplicate registration selection remains platform-specific.

## Ownership and lifecycle evidence

`IRegionViewRegistry` is append-only. Putting host defaults in the shared registry would
accumulate factories, broadcast them to unrelated same-name regions, and retain captured
instances. Weak event handlers do not weaken registration closures. This prototype never
adds a default registration to that shared registry.

The temporary registry is a method-local object. The only long-lived discovery subscription
is the existing weak subscription to the behavior. Its listener bookkeeping can retain dead
weak wrappers until the next raise/remove; this spike does not claim constant listener-list
size or change `WeakDelegatesManager`.

WPF/Uno/Avalonia region behaviors expose no disposal API. Teardown here means removing views,
removing the region from its manager, and dropping host/region references through the existing
lifecycle. An explicit instance or singleton DI registration intentionally reuses that object;
the feature does not clone it. A live host retains its instance-valued attached property. The GC tests
leave that property set and verify the entire released host/region/view graph can be collected
while the shared registry stays alive. MAUI tests similarly verify discarded target-aware regions.
This is not proof of every native page/window destruction path.

## Why no new Prism.Core extraction

Core already owns `IRegion`, `RegionBehavior`, registry contracts, and generic view creation.
The remaining differences are real: DependencyObject/AvaloniaObject/VisualElement properties,
root versus page-scoped container lookup, accepted view objects, autowiring, and Type fallback.
Moving the new glue to Core would need a new host abstraction or callback-driven base solely
to avoid a small amount of platform code. Reuse the existing boundaries for this spike;
consider wider AutoPopulate consolidation separately if its semantics are deliberately aligned.

## Verification and limits

The normal dependency graph cannot restore here: `Prism.Container.Abstractions 10.0.82-pre`
is unavailable on public NuGet (latest public candidate observed: 9.0.114), and no private-feed
credentials were supplied. No production package versions or feeds have been changed.

A deliberately separate diagnostic build compiles the actual modified platform and Core
sources with .NET SDK 10.0.401 against public Abstractions 9.0.114, with a narrowly scoped `ContainerAot` compatibility
shim returning false for generated lookups. It exercises ordinary reflection/scoped-container
paths, not native AOT or private-container compatibility. Do not treat it as the official build.

Current measured results:
- Avalonia / .NET 10 diagnostic: 19 passed, 0 skipped in both Debug and Release. Includes real ContentControl adapter
  host wiring, names/types/instances, 20 replacement cycles per input, discovery ownership,
  late registration, repeated attachment, named lookup, and forced-GC tests.
- MAUI / .NET 10 managed diagnostic: 26 passed, 0 skipped in both Debug and Release. Includes real Region/RegionViewRegistry,
  scoped-container calls, naming, missing Type, replacement hosts and forced-GC tests.
- WPF / .NET 10 Windows diagnostic: production source and shared fixture compile successfully.
  Runtime execution is unavailable on this Linux host.
- Uno / .NET 10 diagnostic: restore and C# compilation reached the resource-injection step,
  where `EmbeddedResourceInjectorTask` failed with MSB4216 and local helper socket
  `Permission denied` (also after the permitted retry). Build did not finish; tests did not run.
- Official private 10.x builds, AOT, compiled-XAML smoke tests, native windows/devices and full
  regression suites remain unverified. No GitHub Actions run was triggered by this spike branch.

The shared desktop fixture is linked into the regular WPF, Uno and Avalonia test projects.
The MAUI fixture is in its regular test project. The diagnostic harness is not included in
production solutions or packaging. No native window/device UI or compiled XAML smoke test
has been run yet. Before promotion, run all official platform tests with the authorized private
feed, plus real loaded/unloaded hosts and MAUI navigation teardown on their supported runtimes.

## Decisions before a production PR

1. Agree on MAUI missing-Type/invalid-instance fail-fast behavior.
2. Agree that defaults use the standard registry policy even if discovery is customized.
3. Keep initial-only semantics and the existing-view metadata rule, or design a separate
   dynamic replacement/renaming contract with appropriate destruction semantics.
4. Run the official platform/private-container and native lifecycle gates above.

No production PR, merge, deployment, or release is part of this spike.

## Reproducing the diagnostic runs

From the repository root, use .NET SDK 10 and writable CLI/NuGet cache directories. Set
`NUGET_PACKAGES` with a trailing slash. Run platform projects serially because they share
production-project restore outputs.

```sh
props="$PWD/spikes/default-view-lifecycle/PublicContracts.props"
dotnet test spikes/default-view-lifecycle/Prism.Avalonia.DefaultView.Spike.Tests.csproj \
  -f net10.0 -m:1 -nr:false -p:UseSharedCompilation=false \
  -p:DirectoryPackagesPropsPath="$props" -p:TargetFrameworks=net10.0 \
  -p:DISABLE_GITVERSIONING=true -p:GeneratePackageOnBuild=false -p:GenerateSBOM=false

dotnet test spikes/default-view-lifecycle/maui/Prism.Maui.DefaultView.Spike.Tests.csproj \
  -f net10.0 -m:1 -nr:false -p:UseSharedCompilation=false -p:UseMaui=false \
  -p:DirectoryPackagesPropsPath="$props" -p:TargetFrameworks=net10.0 \
  -p:DISABLE_GITVERSIONING=true -p:GeneratePackageOnBuild=false -p:GenerateSBOM=false
```

Do not use `PublicContracts.props` or `ContainerAotCompatibility.cs` for official validation,
packaging, deployment, native-AOT claims, or releases. Both exist only to make this spike's
managed lifecycle experiments reproducible without private dependencies.
