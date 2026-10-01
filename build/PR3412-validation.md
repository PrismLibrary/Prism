# PR #3412 assessment

Reviewed public PR: [#3412](https://github.com/PrismLibrary/Prism/pull/3412), immutable head `d17f6e605730bed6e0e74108edfeadb5c3f153c1`. The fix was preserved separately as cherry-pick `44ef09c4` in local validation branch `codex/pr3412-navigation-validation`, tip `97e2955609a80cc7e02c45893e05cddf0539eb01`, based on `e7cf2b9d38df6c51a1e15d83e3083525f52b4777`. It has not been merged into this compatibility branch.

The fix cancels a device-origin modal pop only when Prism will handle it through its current dialog stack or the modal's container. An unowned MAUI modal proceeds without looking up a missing container. This is a narrow modal-navigation fix; it is not a CommunityToolkit adapter or a rewrite of global dialog ownership.

## Managed validation

Installed SDK10.0.401, .NET10: 15 focused tests passed, the DryIoc suite passed 97 with one existing skip, and the MAUI suite passed 195. Coverage includes unowned/managed modal pops, synchronous and genuinely asynchronous confirmation allow/veto, one exit/root-return lifecycle pair, subsequent navigation, dialog command allow/veto, and NavigationService/DialogService source bypass. The existing MAUI fixture needed an external Hosting import; no compatibility change was committed to the PR validation branch.

## Actual native Android Back

An isolated Android15/API35 x64 application used MAUI10.0.10 and the exact branch's portable .NET10 Prism binaries, built with existing SDK10.0.401. `Prism.Maui.dll` SHA256: `145B3F55D1B2DE17D6DA6FB2641E1EB3E658A3BE45A31A106BC670820DD32013`. Android `KEYCODE_BACK` was injected after native UI preparation.

Unowned modal dismissal, managed allow/veto, releasing veto, one lifecycle callback set and subsequent navigation passed. Genuine native async confirmation also passed: allow suspended for 10.109 seconds, veto for 10.002 seconds, and release for 10.005 seconds. During each await the modal remained visible, confirmation count was one and exit callbacks had not run. Allow/release then exited once; veto stayed visible with a cancelled GoBack result. The original single-Back async run did not establish repeated-Back behavior while pending.

### Repeated Back while async confirmation is pending: FAILED

The 2026-10-01 follow-up used the same installed harness and reviewed binaries. Three Back events during async allow dismissed the modal before confirmation completed; final UI reached the launcher. The await ran from 14:11:57.441 to 14:12:07.586 device-local time. At completion `modalCount=0`, but `modalFrom=0` and `rootTo=1` were unchanged; GoBack returned false twice.

Two Back events during async veto also bypassed the veto: await 14:13:43.495 to 14:13:53.715 (10.220 seconds) returned false with `cancel=true`, yet the modal was already gone (`modalCount=0`, confirmation count one, `modalFrom=0`, `rootTo=1`). Pending and final UI showed the root. This is a real native failure, not a passing confirmation or expected dismissal.

Source diagnosis at the reviewed revision: `PageNavigationService.GoBackInternal` sets `NavigationSource.NavigationService` before awaiting confirmation; `PrismWindow` handles modal popping only when the source is Device. Another native Back while that await is outstanding can therefore bypass Prism's interception. A bounded repair should keep device-origin interception active while confirmation awaits, coalesce pending device pops, and switch source only around the actual Prism-controlled pop. It needs native allow/veto/repeated-Back regression tests and single lifecycle counts. No speculative source patch was committed or merged during this assessment.

The PR is not fully ready on the strength of the earlier single-Back passes. Review this failure before approving it. Evidence: `pr3412-repeated-native-logcat.txt`, pending/final XML and failure screenshots, retained with the harness source in the review archive.

Evidence retained for review: harness sources, `PR3412-ANDROID.md`, native logcat, pending/final UI XML, screenshots and hashes. This is native Android hosting of the reviewed navigation implementation, not an Apple test or a full Android Native AOT claim.

## Boundaries

No PR update, merge, package release or GitHub CI/check-run result is claimed by this assessment. iOS sheet gestures, multiwindow and unrelated-modal/global-DialogStack coexistence remain unverified. Dialog command/source bypass has managed coverage but was not exercised by this native harness. The pinned Plugins Prism9.1.86-pre service owns DialogStack entries, while newer Prism source moves ownership to containers; a wider Toolkit/Plugins integration needs a coordinated dependency boundary plan first.
