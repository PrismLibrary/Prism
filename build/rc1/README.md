# Reproduce the RC1 checks

Run `pwsh -File build/rc1/Validate.ps1` from the repository root. Add `-Demo` to build the Android x64 demo. The default host is the normal installed `%ProgramFiles%/dotnet/dotnet.exe`; it must contain the exact RC SDK and matching workload set. Custom tooling locations are supported through `-DotnetPath`, `-AndroidSdkDirectory`, and `-JavaSdkDirectory`.

The script runs from this folder so global.json selects exactly SDK **11.0.100-rc.1.26425.128** (`rollForward: disable`) and workload set **11.0.100-rc.1.26458.5**. It verifies both versions before doing work and temporarily points test/runtime processes at the selected exact SDK. It restores process environment and location afterwards. The root global.json stays unchanged, preserving the other platform workflows.

Install the official RC SDK and the `maui-android` and `maui-windows` workloads into the normal machine SDK using the supported installer. An isolated SDK in `.toolchains/dotnet11-rc1` remains an explicit optional override. Provide JDK21.0.8 and stable native Android platform37.0/build-tools37.0.0 in `.toolchains/android-rc1` (or specify an existing compatible SDK). The script does not install tooling or accept licenses. Source pins Microsoft.Maui.Controls11.0.0-rc.1.26451.6 for net11; net10 keeps its separate package version.

Official sources: https://builds.dotnet.microsoft.com/dotnet/release-metadata/11.0/releases.json and https://github.com/dotnet/maui/releases. `.toolchains` is ignored and must be excluded from source archives. Android API37 is required by the actual RC Android SDK even though the MAUI release overview lists API36.

Apple builds and visual/runtime checks require a capable Mac. GitHub CI has been prepared but has not been executed locally. Demo launch smoke does not establish navigation interaction, full UI compatibility, or Native AOT behavior.
