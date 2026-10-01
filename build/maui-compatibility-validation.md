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

## Android demo smoke (2026-10-01)

Existing .NET10 Android demo builds with 0 errors using SDK11 Preview4; its neutral modules needed a shared Microsoft.Maui.Controls import and explicit MauiXaml items (separate .NET10 sample fix). Restore both `net10.0;net10.0-android` using `-p:TargetFrameworks=net10.0%3Bnet10.0-android -r android-x64`, then build `-f net10.0-android -r android-x64 --no-restore`.

Signed APK installed successfully on existing pixel_7_-_api_35 (emulator-5554). Launcher event succeeded; PID5493 and resumed MainActivity confirmed. Crash buffer was empty. This is a .NET10 launch smoke, not .NET11 runtime validation, UI interaction coverage, or AOT evidence. Raw logs: compat-demo-net10-android.log, compat-demo-crash.log; UI dump: compat-demo-ui.xml, beside worktree.

User subsequently required latest .NET11 RC; Preview4 matrix is baseline evidence only. Latest RC toolchain/API validation and rerunning matrix are pending and must precede any RC support claim.

## Latest official RC1 checkpoint (2026-10-01)

Official metadata https://builds.dotnet.microsoft.com/dotnet/release-metadata/11.0/releases.json reports latest SDK11.0.100-rc.1.26425.128. https://github.com/dotnet/maui/releases reports MAUI11.0.0-rc.1.26451.6 and workload set11.0.100-rc.1.26458.5. Source/test NuGet and CI pins now use these RC versions.

SDK ZIP SHA512 verified against official metadata: c3eed164874adc524c36345f74b0d865520f44278c1e429daea5b54b94d38bb8f386b75daeb689a0d6bf85ea07f7959c8999ebc5e1e95f2c1665054689823c2f. Isolated host `.toolchains/dotnet11-rc1/dotnet.exe`; workloads installed FileBased into isolated SDK: maui-android and maui-windows, exact set11.0.100-rc.1.26458.5. System SDKs unchanged. Ignored .toolchains directory must not be archived into source handoffs.

RC1 results: Prism.Maui.Tests195 passed; Prism.DryIoc.Maui.Tests88 passed/1 existing skipped; Windows library0errors. Layout<View> obsolete compatibility adapter still compiles against actual RC1; retain existing public API. Logs: compat-rc1-sdk.log, compat-rc1-workload-install.log, compat-rc1-maui-tests.log, compat-rc1-dryioc-tests.log, compat-rc1-windows.log.

Default net11.0-android fails XA5207 because RC Android SDK37.0.0-rc.1.2257 requests android-37.0/android.jar, while machine native platforms are36/36.1. MAUI RC release notes listAPI36, but this installed SDK defaultsAPI37.0. JDK21.0.8 and command-line tools19 are installed. Read-only sdkmanager listing confirms stable public platforms;android-37.0 rev2 and build-tools;37.0.0 are available; neither installed at this checkpoint. Log compat-rc1-android.log and compat-rc1-android-packages.log. Explicit net11.0-android36.0 fails NETSDK1140: this RC Android SDK accepts only TargetPlatformVersion37.0. Log compat-rc1-android36.log. It does not replace default Android failure. RC emulator runtime remains unverified. Apple checks remain unverified while Mac offline.

No root SDK pin/rollForward is committed: MAUI CI selects SDK per matrix; local RC checks use the explicit isolated host. GitHub CI execution remains pending. .NET10 baseline test/build and Android smoke results above remain separate from RC1 evidence.
