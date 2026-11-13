# iOS back confirmation

Open **Back confirmation lab** from the demo's first page.

1. Open synchronous confirmation. Leave Allow back off and tap the native back button. The page stays visible and reports one rejected request.
2. Enable Allow back and tap back. The lab menu returns. The console should report OnNavigatedFrom and Destroy once each.
3. Repeat with async confirmation. Tap back several times during its two-second delay. There should be one request, not one per tap. Check both rejection and approval.
4. Check GoBackAsync and GoBackToRootAsync as well. These must not trigger a second confirmation through the native renderer.
5. Check an edge swipe, both rejected and approved. The renderer cancels the interactive gesture while confirmation runs; an approved request uses Prism's normal pop transition.

## Verification

Manually checked on an iPhone 18 Pro simulator, iOS 27.2, using Xcode 27.2 selected per command:

- .NET 10 / MAUI 10.0.70: native synchronous and async rejection/approval, repeated async taps, managed rejection and managed root approval.
- .NET 11 / MAUI 11.0.0: native synchronous and async rejection/approval and repeated async taps.
- Both: approved async requests logged OnNavigatedFrom and Destroy exactly once. Rejected requests logged neither.

The final disposed/managed-pop guards were rebuilt for both targets and async native rejection/approval was repeated on each.

Edge-swipe interaction and custom ContentPage.OnBackButtonPressed overrides still need manual coverage; they are not claimed as verified here.

The platform-neutral unit suite cannot execute UIKit callbacks. Existing tests ran on both targets: 355 passed and one lifetime test failed (`DeferredDestroyOwnsPageUntilDispatcherDrainedThenReleasesGraph`). The same test also fails on unmodified master at 7b6fcc9a under .NET 10 on this Mac.

The scene manifest/delegate and iOS 15 minimum allow the demo to launch with the installed iOS 27.2 workloads; they are not part of the confirmation implementation.
