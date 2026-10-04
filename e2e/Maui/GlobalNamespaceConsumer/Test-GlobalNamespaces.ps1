param(
    [Parameter(Mandatory)] [string] $PackageDirectory,
    [Parameter(Mandatory)] [string] $PackageVersion,
    [string[]] $Frameworks = @('net10.0', 'net11.0'),
    [string[]] $Inflators = @('SourceGen', 'XamlC'),
    [string] $LogDirectory = (Join-Path ([System.IO.Path]::GetTempPath()) ('prism-maui-global-' + [guid]::NewGuid()))
)

$ErrorActionPreference = 'Stop'
$packagePath = (Resolve-Path -LiteralPath $PackageDirectory).Path
$project = Join-Path $PSScriptRoot 'GlobalNamespaceConsumer.csproj'
New-Item -ItemType Directory -Path $LogDirectory -Force | Out-Null
$restorePath = Join-Path (Resolve-Path -LiteralPath $LogDirectory).Path ('packages-' + [guid]::NewGuid())

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead((Join-Path $packagePath "Prism.Maui.$PackageVersion.nupkg"))
try {
    if (-not ($archive.Entries | Where-Object FullName -eq 'buildTransitive/Prism.Maui.targets')) {
        throw 'Prism.Maui is missing its buildTransitive namespace integration.'
    }
}
finally { $archive.Dispose() }

$scenarios = @(
    @{ Name = 'global'; Enabled = 'true'; Canonical = 'false'; Owned = 'false'; Ambiguous = 'false'; Mappings = 9 },
    @{ Name = 'canonical-optout'; Enabled = 'false'; Canonical = 'true'; Owned = 'false'; Ambiguous = 'false'; Mappings = 0 },
    @{ Name = 'owned-optout'; Enabled = 'false'; Canonical = 'false'; Owned = 'true'; Ambiguous = 'false'; Mappings = 3 },
    @{ Name = 'global-optout-negative'; Enabled = 'false'; Canonical = 'false'; Owned = 'false'; Ambiguous = 'false'; Mappings = 0; FailurePattern = '(MAUIX2000|XC0000).*http://schemas.microsoft.com/dotnet/maui/global:(ViewModelLocator|Parameter)' },
    # MAUI 10 SourceGen reports ambiguous elements as a parsing diagnostic.
    @{ Name = 'collision-negative'; Enabled = 'true'; Canonical = 'false'; Owned = 'false'; Ambiguous = 'true'; Mappings = 9; FailurePattern = '((MAUIX2000|XC0000).*EventToCommandBehavior|AmbiguousPage.xaml.*error MAUIG1001)' }
)

foreach ($framework in $Frameworks) {
    $restoreLog = Join-Path $LogDirectory "$framework-restore.log"
    & dotnet restore $project "-p:TargetFrameworks=$framework" "-p:PrismMauiPackageVersion=$PackageVersion" "-p:RestoreAdditionalProjectSources=$packagePath" "-p:RestorePackagesPath=$restorePath" --verbosity quiet *> $restoreLog
    if ($LASTEXITCODE -ne 0) { throw "Consumer restore failed; see $restoreLog" }
    # A fresh cache prevents reused versions from selecting an earlier build.
    # Verify the actual Prism archives, even if another configured feed has the same version.
    foreach ($packageId in @('Prism.Maui', 'Prism.Core', 'Prism.Events')) {
        $expected = Join-Path $packagePath "$packageId.$PackageVersion.nupkg"
        $id = $packageId.ToLowerInvariant()
        $version = $PackageVersion.ToLowerInvariant()
        $installed = Join-Path $restorePath "$id/$version/$id.$version.nupkg"
        if ((Get-FileHash -LiteralPath $expected).Hash -ne (Get-FileHash -LiteralPath $installed).Hash) {
            throw "$packageId was not restored from the supplied package artifact."
        }
    }
    foreach ($inflator in $Inflators) {
        foreach ($scenario in $scenarios) {
            $name = "$framework-$inflator-$($scenario.Name)"
            $log = Join-Path $LogDirectory "$name.log"
            $arguments = @(
                $project, '-c', 'Release', '-f', $framework, '--verbosity', 'quiet',
                "-p:TargetFrameworks=$framework", "-p:PrismMauiPackageVersion=$PackageVersion",
                "-p:RestorePackagesPath=$restorePath", "-p:MauiXamlInflator=$inflator",
                "-p:PrismMauiGlobalXmlns=$($scenario.Enabled)", "-p:CanonicalOnly=$($scenario.Canonical)",
                "-p:OwnGlobalMapping=$($scenario.Owned)", "-p:AmbiguousGlobalNamespace=$($scenario.Ambiguous)"
            )
            & dotnet build @arguments '-t:Rebuild' --no-restore *> $log
            $result = $LASTEXITCODE
            if (-not $scenario.FailurePattern -and $result -eq 0) {
                & dotnet test @arguments --no-build --no-restore *>> $log
                $result = $LASTEXITCODE
            }
            $output = Get-Content -LiteralPath $log -Raw
            if ($scenario.FailurePattern) {
                if ($result -eq 0 -or $output -notmatch $scenario.FailurePattern) {
                    throw "Expected XAML type resolution failure for $name; see $log"
                }
            }
            elseif ($result -ne 0 -or $output -notmatch 'Passed!') {
                throw "Consumer regression failed for $name; see $log"
            }
            $assemblyInfo = Join-Path $PSScriptRoot "obj/Release/$framework/GlobalNamespaceConsumer.AssemblyInfo.cs"
            $metadata = Get-Content -LiteralPath $assemblyInfo -Raw
            $mappings = [regex]::Matches($metadata, 'XmlnsDefinitionAttribute\("http://schemas.microsoft.com/dotnet/maui/global",\s*"(Prism(?:\.[^"]+)?)",\s*AssemblyName\s*=\s*"Prism.Maui"\)')
            $mappingCount = $mappings.Count
            if ($mappingCount -ne $scenario.Mappings) { throw "Unexpected mapping count $mappingCount for $name" }
            $namespaces = @($mappings | ForEach-Object { $_.Groups[1].Value })
            if (@($namespaces | Sort-Object -Unique).Count -ne $mappingCount -or $namespaces -contains 'Prism.Navigation.Xaml') {
                throw "Duplicate or colliding global namespace mapping for $name"
            }
            Write-Output "PASS $name (Prism namespace mappings: $mappingCount)"
        }
    }
}
Write-Output "Logs: $LogDirectory"
exit 0
