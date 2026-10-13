# Executed coverage and results

Baseline production source: current master `91b8986c`. All production files restored byte-for-byte after the diagnostic experiment (`git diff --exit-code -- src` succeeds). SDK selected by the unmodified repository: `11.0.100-preview.4.26230.115`; tests below target net10 or .NET Framework 4.7.2. No package versions or global SDK settings changed.

| Host / scenario | Result | Limits |
| --- | --- | --- |
| Core net10 complete existing suite plus new controls; KnownRetention excluded | 351 passed, 0 failed, 0 skipped | Includes 14 new passing lifetime cases. One deliberately failing retention reproduction is run separately. |
| Core net10 cleared nested child, unchanged source | 1 failed, as expected | Former child remains rooted by retained owner/observer node. |
| WPF Windows STA, net472 actual content adapter/default behaviors, retained manager/registry/mock container | 3 passed | 16 region/host/view/model remove/recreate cycles, unnamed abandonment, retained-host ownership control. No real DryIoc/Unity scope or window tested. |
| WPF Windows STA net472 abandoned nonvisual host and disposed manager/custom loader | 2 failed, as expected | Module load is complete/Initialized before Dispose. Bare host collects before behavior assertion. |
| MAUI managed net10 controlled dispatcher / ancestor publisher | 2 passed | 16 cycles each. Mock scope Dispose once; page/model/scope collection after drain. Native handlers, actual window and real container untested. |
| Avalonia net10 managed STA content-control host | 3 passed; 2 expected retention failures | Actual shared adapter/default behaviors and abandoned host/module tests executed. No rendered window, native backend or real container tested. Existing task DLL path supplied locally. |
| Uno managed net10 | 1 passed; 3 host-initialization failures | Unnamed region abandonment passed. Content-control ownership/recreation and bare nonvisual host cases stopped at native dispatcher/control initialization, before GC assertions. These failures do not prove Uno retention. |
| Diagnostic observer root-release experiment | 15 passed | Includes the formerly failing cleared-child reproduction and 14 lifetime controls. Temporary source change only. |
| Diagnostic WPF tracker/subscription root-release experiment | 5 passed | Includes both formerly failing reproductions plus 3 controls. Temporary source changes only. |

The causal diagnostic patch is saved as `diagnostic-root-release.patch`. It is evidence for root-chain hypotheses, not a shipped fix. Existing backing fields/event subscriptions were released in the experiment; normal source was then restored and freshly recompiled. The Core failure and both WPF failures returned. Both desktop failures also reproduced in Avalonia. This distinguishes actual baseline retention from weak references, dead cycles, requested ownership and cached experimental binaries.

TRX-derived per-test outcomes, with local paths removed, are in `evidence.json`. Full local TRX files remain under each test project's TestResults directory. Experiment summaries are separate from baseline results. Intermediate discovery failures and an invalid static-factory harness attempt are not included as successful qualification: the final static-factory test uses public factory replacement, preserves/restores prior global registration state, and passes.

## Reproduction

Run builds serially with `-m:1 -p:UseSharedCompilation=false`. Do not build the full solution just to reproduce these cases.

```powershell
# Core controls and existing suite
 dotnet test tests/Prism.Core.Tests/Prism.Core.Tests.csproj -c Release -f net10.0 -p:TargetFrameworks=net10.0 -m:1 -p:UseSharedCompilation=false --filter 'Lifetime!=KnownRetention'
# Deliberately failing Core regression
 dotnet test tests/Prism.Core.Tests/Prism.Core.Tests.csproj -c Release -f net10.0 --no-build --no-restore --filter 'Lifetime=KnownRetention'
# Shared WPF controls and deliberate regressions
 dotnet test tests/Wpf/Prism.Wpf.Tests/Prism.Wpf.Tests.csproj -c Release -m:1 -p:UseSharedCompilation=false --filter 'FullyQualifiedName~Prism.Tests.Lifetime'
# MAUI managed-only qualification, without device workloads or package generation
 dotnet test tests/Maui/Prism.Maui.Tests/Prism.Maui.Tests.csproj -c Release -f net10.0 -p:TargetFrameworks=net10.0 -p:GeneratePackageOnBuild=false -m:1 -p:UseSharedCompilation=false --filter 'FullyQualifiedName~MauiLifetimeFixture'
# Uno managed-only qualification
 dotnet test tests/Uno/Prism.Uno.WinUI.Tests/Prism.Uno.WinUI.Tests.csproj -c Release -p:UnoTargetFrameworks=net10.0 -m:1 -p:UseSharedCompilation=false --filter 'FullyQualifiedName~Prism.Tests.Lifetime'
```

This environment needed elevated access to its existing NuGet configuration/package cache. No credentials were created. Avalonia compilation required a command-local `AvaloniaBuildTasksLocation` pointing to the installed package's `tools/netstandard2.0/Avalonia.Build.Tasks.dll`; this is not committed as a machine-specific project setting. Initial zero-test discovery was caused by globally overriding `TargetFrameworks` on the single-target test project; preserving that project setting restored discovery without a package change.

## Untested combinations and blockers

- Core net462/net47/net8/net9; WPF net8/net9/net10 Windows and real containers/windows.
- Avalonia rendered native/headless/Desktop windows, real scopes, and other operating systems untested; managed STA control execution is covered above.
- Uno UI-dependent fixtures are blocked by missing initialized native dispatcher on the managed-only test host. Desktop, native WinUI, browserwasm, Android/iOS/tvOS and real dialogs/native controls remain untested.
- MAUI native Windows/Android/iOS, MacCatalyst, actual dispatcher shutdown, device dismissal, real scoped service/container collection and multi-window app lifetime untested. No Mac/device work was authorized.
- Dialog teardown/fault paths, navigation rollback/context release, actual async timers, MAUI Rx subscriptions and application static-root teardown remain explicit coverage gaps in INVENTORY.md.
- PR3442's exact proposed default-view lifecycle stack and retained default-instance property are outside this baseline: tests should be repeated on that immutable head under separate direction, not silently cherry-picked into this audit.

No finite pass count warrants an entire-API leak-free claim. The highest-priority next work is the three demonstrated roots, real container/host lifecycle tests, and the missing platform execution paths.
