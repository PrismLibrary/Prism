[CmdletBinding()]
param(
    [string] $DotnetPath = (Join-Path $env:ProgramFiles 'dotnet/dotnet.exe'),
    [string] $AndroidSdkDirectory = (Join-Path $PSScriptRoot '../../.toolchains/android-rc1'),
    [string] $JavaSdkDirectory = 'C:/Program Files/Android/openjdk/jdk-21.0.8',
    [switch] $Demo
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$DotnetPath = (Resolve-Path $DotnetPath).Path
$AndroidSdkDirectory = (Resolve-Path $AndroidSdkDirectory).Path
$JavaSdkDirectory = (Resolve-Path $JavaSdkDirectory).Path
$originalLocation = Get-Location
$originalPath = $env:PATH
$originalRoot = $env:DOTNET_ROOT
$originalRootX64 = $env:DOTNET_ROOT_X64
try {
    Set-Location $PSScriptRoot # Resolves the scoped exact SDK/workload global.json.
    $env:DOTNET_ROOT = Split-Path $DotnetPath
    $env:DOTNET_ROOT_X64 = $env:DOTNET_ROOT
    $env:PATH = "$env:DOTNET_ROOT;$originalPath"
    $sdkVersion = (& $DotnetPath --version).Trim()
    if ($LASTEXITCODE -ne 0 -or $sdkVersion -ne '11.0.100-rc.1.26425.128') { throw "Unexpected SDK: $sdkVersion" }
    $workloadVersion = (& $DotnetPath workload --version).Trim()
    if ($LASTEXITCODE -ne 0 -or $workloadVersion -ne '11.0.100-rc.1.26458.5') { throw "Unexpected workload set: $workloadVersion" }
    Write-Host "SDK $sdkVersion; workload set $workloadVersion; Controls 11.0.0-rc.1.26451.6"
    if ($Demo) {
        $project = Join-Path $repoRoot 'e2e/Maui/PrismMauiDemo/PrismMauiDemo.csproj'
        & $DotnetPath restore $project -p:Configuration=Release -p:PublishReadyToRun=true '-p:TargetFrameworks=net10.0%3Bnet11.0%3Bnet11.0-android' -r android-x64 -p:DISABLE_GITVERSIONING=true
        if ($LASTEXITCODE -ne 0) { throw 'Demo restore failed' }
        & $DotnetPath build $project -c Release -f net11.0-android -r android-x64 --no-restore "-p:AndroidSdkDirectory=$AndroidSdkDirectory" "-p:JavaSdkDirectory=$JavaSdkDirectory" -p:DISABLE_GITVERSIONING=true
        if ($LASTEXITCODE -ne 0) { throw 'Demo build failed' }
    } else {
        foreach ($name in @('Prism.Maui.Tests', 'Prism.DryIoc.Maui.Tests')) {
            & $DotnetPath test (Join-Path $repoRoot "tests/Maui/$name/$name.csproj") -c Release -f net11.0 -p:TargetFrameworks=net11.0 -p:DISABLE_GITVERSIONING=true
            if ($LASTEXITCODE -ne 0) { throw "Tests failed: $name" }
        }
        & $DotnetPath build (Join-Path $repoRoot 'src/Maui/Prism.Maui.Rx/Prism.Maui.Rx.csproj') -c Release -f net11.0 -p:TargetFrameworks=net11.0 -p:DISABLE_GITVERSIONING=true
        if ($LASTEXITCODE -ne 0) { throw 'Rx build failed' }
        foreach ($framework in @('net11.0-android', 'net11.0-windows10.0.19041')) {
            & $DotnetPath build (Join-Path $repoRoot 'src/Maui/Prism.Maui/Prism.Maui.csproj') -c Release -f $framework "-p:TargetFrameworks=$framework" "-p:AndroidSdkDirectory=$AndroidSdkDirectory" "-p:JavaSdkDirectory=$JavaSdkDirectory" -p:DISABLE_GITVERSIONING=true
            if ($LASTEXITCODE -ne 0) { throw "Build failed: $framework" }
        }
    }
} finally {
    Set-Location $originalLocation
    $env:PATH = $originalPath
    $env:DOTNET_ROOT = $originalRoot
    $env:DOTNET_ROOT_X64 = $originalRootX64
}
