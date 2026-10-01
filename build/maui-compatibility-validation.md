# MAUI compatibility validation (dev-01, 2026-10-01)

Branch codex/maui-net11-compatibility, based on e7cf2b9d. This report supersedes the earlier Preview4 and missing-API37 checkpoints.

| Check | .NET10 baseline (Debug) | Latest official .NET11 RC1 (Release) |
|---|---|---|
| SDK | 10.0.401 | 11.0.100-rc.1.26425.128 |
| Controls package | 10.0.10 | 11.0.0-rc.1.26451.6 |
| Workload set | Existing machine installation | 11.0.100-rc.1.26458.5 |
| Prism.Maui.Tests | 195 passed | 195 passed |
| Prism.DryIoc.Maui.Tests | 88 passed, 1 existing skip | 88 passed, 1 existing skip |
| Prism.Maui.Rx neutral build | Covered by DryIoc suite | Explicit Release build, 0 errors |
| Default Android library build | 0 errors | 0 errors, native API37.0 |
| Windows library build | 0 errors | 0 errors |
| Android demo | APK built, API35 emulator launch confirmed | Release APK0errors; API35 emulator launch confirmed |

Current RC Release matrix log: ../compat-rc1-release-matrix.log. It was run through build/rc1/Validate.ps1 with the isolated official RC host and native SDK. Earlier RC Debug results also pass (../compat-rc1-pinned-matrix.log). Before/after comparison is deliberately labelled by configuration. Raw .NET10 logs: compat-net10-tests.log, compat-dryioc-net10.log, compat-net10-android.log, compat-net10-windows.log, compat-demo-net10-android.log.

## Reproducible version selection

build/rc1/global.json pins SDK11.0.100-rc.1.26425.128 with rollForward: disable and workloadVersion11.0.100-rc.1.26458.5. Validate.ps1 defaults to the normal installed ProgramFiles/dotnet/dotnet.exe, verifies exact SDK/workload versions, temporarily selects its runtime host, then restores process environment/location. Explicit DotnetPath overrides support the isolated SDK. The root global.json stays unpinned to avoid changing other platform workflows. The user subsequently requested normal RC installation and removal of Preview4; a separate installation worker owns that machine transition. Its final verification is not implied by this branch's isolated SDK tests.

Run `pwsh -File build/rc1/Validate.ps1` for Release tests, Rx, Android and Windows builds. Add `-Demo` for the RC Android x64 demo. Demo restore includes net10 Core references and Release ReadyToRun assets. The script does not install tooling or accept licenses. CI prepares explicit SDK10/11 jobs, exact RC workload set, JDK21.0.8 and stable native API37; YAML parses successfully, GitHub execution pending.

## Tooling and API evidence

Official sources: https://builds.dotnet.microsoft.com/dotnet/release-metadata/11.0/releases.json and https://github.com/dotnet/maui/releases. SDK ZIP SHA512 matched official metadata: c3eed164874adc524c36345f74b0d865520f44278c1e429daea5b54b94d38bb8f386b75daeb689a0d6bf85ea07f7959c8999ebc5e1e95f2c1665054689823c2f. Isolated host .toolchains/dotnet11-rc1/dotnet.exe installed exact maui-android/maui-windows workload set FileBased. Logs: compat-rc1-sdk.log and compat-rc1-workload-install.log.

The RC Android SDK37.0.0-rc.1.2257 requires platform37.0; platform36 is rejected with NETSDK1140 despite MAUI release overview listing API36. Initial default build failed XA5207; that blocker is resolved by stable official platform37.0 rev2 and build-tools37.0.0 in .toolchains/android-rc1. SDKmanager confirms PreviewSdkInt=0. Existing accepted license files were copied; no --licenses command, affirmative license input, or new legal acceptance occurred. JDK21.0.8 and command-line tools19 were already installed. Native SDK installation log: compat-rc1-native-install.log. .toolchains is ignored and must be excluded from source archives.

Actual RC builds still expose obsolete Compatibility.Layout<View>; retain the public region adapter rather than remove it based on the standalone compatibility package's removal. net11 Android minimum24; net9/net10 retain21. PageDialogService uses Async APIs under NET10_0_OR_GREATER and retains net9 calls. Sample .NET10 fixes (Controls using and explicit module XAML items) were committed separately from RC targets.

## Runtime and remaining checks

.NET10 signed APK installed and launched on pixel_7_-_api_35, emulator-5554: PID5493, resumed MainActivity, empty crash buffer; UI dump compat-demo-ui.xml. This establishes launch smoke only.

RC demo targets and package references now use net11/RC1; Release module source-generated XAML compiles. Release APK with default ReadyToRun processing builds with 0 errors/32 warnings (compat-rc1-release-demo.log). Signed APK installed successfully on existing API35 emulator5554 and remained running: PID6104, resumed MainActivity, empty crash buffer; UI hierarchy compat-rc1-demo-ui.xml and crash log compat-rc1-demo-crash.log. This establishes startup smoke only. Normal-machine RC installer is a separate worker task; its pending UAC prompt is not bypassed and systemhost validation must wait for verified installation completion.

Apple compilation and iOS visual/runtime checks remain unverified while Mac offline. CI has not executed on GitHub. Android launch smoke does not establish full navigation interactions or Native AOT. No source push, merge, package publication or production deployment occurred. Existing SourceLink dependency Microsoft.Build.Tasks.Git10.0.300 emits NU1902; its version was not changed here.
