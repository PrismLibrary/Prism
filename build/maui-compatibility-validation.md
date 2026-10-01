# MAUI compatibility validation (dev-01, 2026-10-01)

Branch: codex/maui-net11-compatibility, based on e7cf2b9d.

| Check | .NET 10 | .NET 11 Preview 4 |
|---|---|---|
| SDK | 10.0.401 | 11.0.100-preview.4.26230.115 |
| MAUI package | 10.0.10 | 11.0.0-preview.4.26230.3 |
| Prism.Maui.Tests | 195 passed | 195 passed |
| Prism.DryIoc.Maui.Tests | 88 passed, 1 existing skip | 88 passed, 1 existing skip |
| Prism.Maui Android library build | passed, 0 errors | passed, 0 errors |
| Prism.Maui Windows library build | passed, 0 errors | passed, 0 errors |

Commands use `-f <framework> -p:TargetFrameworks=<framework> -p:DISABLE_GITVERSIONING=true` to restrict restore to installed platform workloads. The SDK was selected temporarily through global.json; the committed root file does not impose a preview SDK on unrelated platform workflows. CI selects each SDK explicitly and runs Android/Windows Release builds and both neutral test suites. CI YAML parsed with PyYAML; GitHub execution remains pending. This replaces the all-target reusable MAUI job with the explicit Windows-hosted matrix; Apple compilation and package aggregation still require a separate capable runner.

Raw local logs live beside this worktree: compat-net10-tests.log, compat-net11-tests.log, compat-dryioc-net10.log, compat-dryioc-net11.log, compat-net10-android.log, compat-net11-android.log, compat-net10-windows.log, compat-net11-windows.log.

API review: https://learn.microsoft.com/en-us/dotnet/maui/whats-new/dotnet-11 and https://github.com/dotnet/maui/releases/tag/11.0.0-preview.4.26230.3. Preview 4 still contains obsolete Compatibility.Layout<View>; retain this public adapter contract rather than apply later-preview removals. Android minimum 24 applies only to net11; net9/net10 keep 21. Page dialogs call Async APIs under NET10_0_OR_GREATER and retain net9 calls.

Apple compilation and iOS visual/runtime checks are unverified (Mac offline). Android API35 AVD exists; emulator/demo runtime smoke is tracked separately and is not implied by the library build results above. No toolchain installs, source pushes, merges or package publication were performed.
