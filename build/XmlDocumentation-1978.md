# XML documentation validation for issue #1978

This branch documents authored Prism APIs and promotes XML documentation
diagnostics to build errors. It remains WIP because the external markup generator
does not document its generated public API. No targets are excluded from the
policy, and no new documentation suppression is introduced.

Baseline on master `ac062edd`: portable MAUI net10.0 had 132 CS1591 diagnostics;
portable Uno net10.0 had 26 CS1591 and one CS1574 after exposing its existing
suppression. WPF's five library targets had five CS0419 and 20 CS1587 diagnostic
instances; Avalonia net9.0/net10.0 had two CS1574 and eight CS1587 instances.
Core/Events net10.0 had none. Counts deduplicate build-summary repeats while
retaining each affected project/target instance. Authored documentation gaps in
DryIoc MAUI and Rx MAUI exposed by the new policy were also corrected.

## Generator investigation

The affected project is
`src/Uno/Prism.Uno.Markup/Prism.Uno.WinUI.Markup.csproj`. It directly references
`Uno.Extensions.Markup.Generators` **6.4.11** and `Uno.WinUI.Markup` **6.4.11**
through `Directory.Packages.props`. The latter brings in
`Uno.Extensions.Markup.WinUI` 6.4.11, which defines the assembly generation
attribute. The main Uno and DryIoc Uno libraries do not directly reference the
markup generator.

The generator assembly identifies its repository as
`https://github.com/unoplatform/uno.csharpmarkup`, with source commit
`f3b900f783cc812e20c9d99e010952a0490b7213`. GitHub returns 404 for this repository
with the current account, so its original source and issue tracker could not be
inspected. The published package implementation was inspected locally instead.

Uno's official [Generating Extensions guide](https://github.com/unoplatform/uno.extensions/blob/main/doc/Learn/Markup/GeneratingExtensions.md)
documents assembly generation via `GenerateMarkupForAssembly(typeof(...))`.
The actual public attribute has only a `Type referenceType` constructor and no
configuration properties. The package's build imports expose
`HasMarkupGeneratorsReference` and `EnableUnoCSMUsings`; neither controls XML
documentation. The generator's implementation neither reads XML comments with
`GetDocumentationCommentXml` nor consumes a documentation option. Internal
documentation-builder utilities exist but are not called by the API generators.

The portable net10.0 build emits **20 markup source files with zero XML-comment
lines**, yielding **127 CS1591 errors**:

| Generator | CS1591 declarations |
| --- | ---: |
| AttachedPropertyGenerator | 92 |
| DataTemplateGenerator | 8 |
| DependencyPropertyExtensionGenerator | 27 |

Affected APIs include dialog, view-model locator, region manager and region
behavior property builders, dialog-window template helpers, and dependency
property helpers for `InvokeCommandAction` and `ItemMetadata`.

The inputs already contain valid documentation. For example, the built
`Prism.Uno.xml` documents `Dialog.WindowStyleProperty`, `GetWindowStyle` and
`SetWindowStyle`, including parameters and return information. The generated
`DialogPropertyBuilder.WindowStyle` overloads still have no comments.

A temporary `VersionOverride` tested stable generator **6.7.21** against the
actual Prism project. It produced the same 127 errors; all 20 emitted files were
identical to 6.4.11 after normalizing the generator version marker. The override
was removed and 6.4.11 restored. No package update or third-party fork was
committed. Original source for 6.7.21 is identified by package commit
`895508ce827a969a94c1d2fdb71e6a30f8514f71`.

A temporary source-only partial declaration documented the existing
`DialogPropertyBuilder` type without adding members. That removed its type
warning but left **126 CS1591 errors**. Generated methods are not partial, so
another source declaration cannot attach comments to them without duplicate
members or replacing generation. The probe was removed; no extra public type or
member was committed.

Public Uno issue searches found related documentation discussions, including
[templates issue #38](https://github.com/unoplatform/uno.templates/issues/38) and
[Resizetizer issue #149](https://github.com/unoplatform/uno.resizetizer/issues/149).
Those concern different components and do not provide a markup-generator fix.
No upstream issue or message was created.

## Repository policy gap fixed

.NET 10 Android's `Xamarin.Android.Bindings.Core.targets` appends
`CS1573;CS1591` to `NoWarn` whenever `DocumentationFile` is set. Those late SDK
defaults overrode Prism's props-level error policy and explained the earlier
Android markup build success. This was verified in the installed Android SDK
36.1.69 and Microsoft's [official Android target source](https://github.com/dotnet/android/blob/main/src/Xamarin.Android.Build.Tasks/MSBuild/Xamarin/Android/Xamarin.Android.Bindings.Core.targets).

`src/Directory.Build.targets` now removes only the documentation diagnostics
from `NoWarn` and reapplies their error policy immediately before compilation.
Unrelated suppressions remain. The regression check uses the actual Prism.Events
project to verify CS1591, CS1570 and CS1573 fail both normally and with a
late-imported suppression list; it also checks unrelated suppressions survive.
Temporary sources and imports are removed in a `finally` block. No smoke
application is committed. Library CI workflows watch both shared props and
targets, and the Core workflow runs the regression check.

All six failure probes pass, unrelated suppressions are preserved, and the clean
Prism.Events build passes after probe cleanup. WPF (all targets), Avalonia/DryIoc
Avalonia (both targets), MAUI net10.0 Android and DryIoc Uno net9.0/net10.0 Android
were rebuilt successfully after strengthening the policy.

## Validation coverage

Builds use one MSBuild node, `BuildInParallel=false`, and
`GeneratePackageOnBuild=false`. Logs were retained in the isolated task workspace.

| Library family | Successful targets |
| --- | --- |
| Core / Events | net462, net47, net8.0, net9.0, net10.0 |
| WPF / DryIoc WPF / Unity WPF | net462, net47, net8.0-windows, net9.0-windows, net10.0-windows |
| Avalonia / DryIoc Avalonia | net9.0, net10.0 |
| MAUI | portable net10.0/net11.0; net10.0-android; net10.0/net11.0 Windows |
| DryIoc MAUI / Rx MAUI | portable net10.0, net11.0 |
| Uno / DryIoc Uno | net9.0/net10.0 portable, desktop, browserwasm, Android, Windows |

Windows Uno builds use Visual Studio full `MSBuild.exe`, as required by WinUI
XAML compilation. For example:

```powershell
MSBuild.exe <project> /restore /t:Build /m:1 /nologo `
  /p:UnoTargetFrameworks=net10.0-windows10.0.19041 `
  /p:BuildInParallel=false /p:GeneratePackageOnBuild=false
```

Do not propagate a global Windows `TargetFramework` to Core/Events references.
Android net9.0 requires a local command-only `CheckEolWorkloads=false` override
to test beyond the SDK's EOL rejection; no override is committed.

Markup fails with 127 undocumented declarations on net9.0/net10.0 portable,
desktop, browserwasm, Windows and Android. The final net10.0 Android build
confirms 127 CS1591 errors after removing the SDK defaults; MAUI net10.0 Android
still builds successfully under the strengthened policy. The markup target's
earlier pass with SDK suppressions is superseded.

| Test suite | Passed | Skipped | Failed |
| --- | ---: | ---: | ---: |
| Core net10.0 | 337 | 0 | 0 |
| DryIoc WPF net472 | 76 | 0 | 0 |
| Unity WPF net472 | 76 | 0 | 0 |
| WPF Modularity net10.0 | 19 | 0 | 0 |
| WPF net472 | 481 | 0 | 1 |
| MAUI net10.0 | 328 | 0 | 0 |
| DryIoc MAUI net10.0 | 258 | 0 | 0 |
| Avalonia net10.0 | 407 | 41 | 0 |
| DryIoc Avalonia net10.0 | 68 | 0 | 0 |
| Uno net10.0 | 426 | 0 | 0 |
| DryIoc Uno net10.0-desktop | 34 | 0 | 0 |
| **Total** | **2,510** | **41** | **1** |

The sole failure is
`DelayedRegionCreationBehaviorFixture.ShouldCleanupBehaviorOnceRegionIsCreated`,
a weak-reference assertion at line 189. The exact focused test also fails on
unchanged master. Uno additionally reports duplicate inventory case IDs.

MAUI net11.0-android fails with CS1061 on `ILifecycleBuilder.AddAndroid` and
CS0246 for `MauiAppCompatActivity` using the installed .NET 11 preview SDK and
repository RC packages. Reproduced on unchanged master. Apple targets were not
built on this Windows host, and offline Mac work was not resumed. GitHub CI has
not run because this remains branch-only, without a PR or workflow dispatch.

## Recommended next decision

The smallest API-preserving fix is for the upstream generator to emit meaningful
summaries, parameters/type parameters and return documentation for its builders
and overloads, then consume an official fixed release. Existing generator inputs
and public configuration cannot add those comments. Source post-processing would
require replacing the normal compiler-generation pipeline. Maintaining generated
Prism markup source instead would introduce ongoing API maintenance and needs
separate approval and cross-target contract verification.

Authorize coordination with Uno for a generator documentation fix/release, or
provide access to its source so a concrete unpublished patch can be prepared.
No contacting maintainers, upstream issue, fork, publication, PR or merge has
been performed as part of this investigation.
