param(
    [Parameter(Mandatory)] [string] $PackageDirectory,
    [Parameter(Mandatory)] [string] $PackageVersion,
    [string] $UnoSdkVersion,
    [string] $LogDirectory = (Join-Path ([System.IO.Path]::GetTempPath()) ('prism-uno-global-' + [guid]::NewGuid()))
)

$ErrorActionPreference = 'Stop'
$packagePath = (Resolve-Path -LiteralPath $PackageDirectory).Path
New-Item -ItemType Directory -Path $LogDirectory -Force | Out-Null
$consumerDirectory = Join-Path (Resolve-Path -LiteralPath $LogDirectory).Path 'external-consumer'
New-Item -ItemType Directory -Path $consumerDirectory -Force | Out-Null
if (-not $UnoSdkVersion) {
    $config = Get-Content (Join-Path $PSScriptRoot '../../../global.json') -Raw | ConvertFrom-Json
    $UnoSdkVersion = $config.'msbuild-sdks'.'Uno.Sdk'
}
foreach ($file in Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.xaml' -File) {
    Copy-Item -LiteralPath $file.FullName -Destination $consumerDirectory
}
foreach ($directory in @('First', 'Second', 'Properties')) {
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot $directory) -Destination $consumerDirectory -Recurse -Force
}
$project = Join-Path $consumerDirectory 'Prism.Uno.XamlConsumer.csproj'
$projectText = Get-Content (Join-Path $PSScriptRoot 'Prism.Uno.XamlConsumer.csproj') -Raw
$projectText = $projectText.Replace('Sdk="Uno.Sdk"', "Sdk=`"Uno.Sdk/$UnoSdkVersion`"")
$projectText = $projectText.Replace('VersionOverride=', 'Version=')
[System.IO.File]::WriteAllText($project, $projectText)
# Prevent the probe from inheriting repository properties or central package versions.
Set-Content -LiteralPath (Join-Path $consumerDirectory 'Directory.Build.props') -Value '<Project />'
Set-Content -LiteralPath (Join-Path $consumerDirectory 'Directory.Build.targets') -Value '<Project />'
Set-Content -LiteralPath (Join-Path $consumerDirectory 'Directory.Packages.props') -Value '<Project />'
Add-Type -AssemblyName System.IO.Compression.FileSystem
foreach ($id in @('Prism.Uno.WinUI', 'Prism.DryIoc.Uno.WinUI')) {
    $archive = [System.IO.Compression.ZipFile]::OpenRead((Join-Path $packagePath "$id.$PackageVersion.nupkg"))
    try {
        if (-not ($archive.Entries | Where-Object FullName -eq "buildTransitive/$id.targets")) {
            throw "$id is missing its buildTransitive namespace integration."
        }
    }
    finally { $archive.Dispose() }
}

$scenarios = @(
    @{ Name = 'default'; Case = ''; Mappings = 8 },
    @{ Name = 'disabled'; Case = ''; Disabled = $true; Mappings = 0 },
    @{ Name = 'canonical-disabled'; Case = 'ExplicitNamespaces'; Disabled = $true; Mappings = 0 },
    @{ Name = 'global-disabled'; Case = 'ImplicitNamespaces'; Disabled = $true; Failure = $true },
    @{ Name = 'custom-global'; Case = ''; Uri = 'urn:prism-uno-tests:global'; Mappings = 8 },
    @{ Name = 'default-restored'; Case = ''; Mappings = 8 }
)
foreach ($scenario in $scenarios) {
    $log = Join-Path $LogDirectory "$($scenario.Name).log"
    $arguments = @($project, '-f', 'net10.0', '-t:Rebuild', '--verbosity', 'quiet',
        '-p:UnoTargetFrameworks=net10.0', '-p:GeneratePackageOnBuild=false', '-p:DISABLE_GITVERSIONING=true',
        "-p:PrismPackageVersion=$PackageVersion", "-p:RestoreAdditionalProjectSources=$packagePath",
        "-p:XamlConsumerCase=$($scenario.Case)")
    if ($scenario.Disabled) { $arguments += '-p:PrismUnoGlobalXmlns=false' }
    if ($scenario.Uri) { $arguments += "-p:UnoGlobalXamlNamespaceUri=$($scenario.Uri)" }
    & dotnet build @arguments *> $log
    $result = $LASTEXITCODE
    $output = Get-Content -LiteralPath $log -Raw
    if ($scenario.Failure) {
        if ($result -eq 0 -or $output -notmatch 'error UXAML0001: The type .*InvokeCommandAction could not be found') {
            throw "Expected unprefixed Prism XAML failure; see $log"
        }
    }
    else {
        if ($result -ne 0) { throw "Consumer build failed; see $log" }
        $assemblyInfo = Get-Content (Join-Path $consumerDirectory 'obj/Debug/net10.0/Prism.Uno.XamlConsumer.AssemblyInfo.cs') -Raw
        $uri = if ($scenario.Uri) { $scenario.Uri } else { 'http://schemas.microsoft.com/winfx/2006/xaml/presentation/global' }
        $count = [regex]::Matches($assemblyInfo, ('XmlnsDefinitionAttribute\("' + [regex]::Escape($uri) + '",')).Count
        if ($count -ne $scenario.Mappings) { throw "Expected $($scenario.Mappings) generated mappings, found $count; see $log" }
    }
    Write-Output "PASS $($scenario.Name)"
}
