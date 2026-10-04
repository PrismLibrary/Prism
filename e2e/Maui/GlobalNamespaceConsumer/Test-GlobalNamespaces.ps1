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

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead((Join-Path $packagePath "Prism.Maui.$PackageVersion.nupkg"))
try {
    if (-not ($archive.Entries | Where-Object FullName -eq 'buildTransitive/Prism.Maui.targets')) {
        throw 'Prism.Maui is missing its buildTransitive namespace integration.'
    }
}
finally { $archive.Dispose() }

$scenarios = @(
    @{ Name = 'global'; Enabled = 'true'; Canonical = 'false'; Owned = 'false'; Ambiguous = 'false'; Mappings = 1 },
    @{ Name = 'canonical-optout'; Enabled = 'false'; Canonical = 'true'; Owned = 'false'; Ambiguous = 'false'; Mappings = 0 },
    @{ Name = 'owned-optout'; Enabled = 'false'; Canonical = 'false'; Owned = 'true'; Ambiguous = 'false'; Mappings = 1 },
    @{ Name = 'global-optout-negative'; Enabled = 'false'; Canonical = 'false'; Owned = 'false'; Ambiguous = 'false'; Mappings = 0; FailurePattern = '(MAUIX2000|XC0000).*http://schemas.microsoft.com/dotnet/maui/global:(ViewModelLocator|Parameter)' },
    # MAUI 10 SourceGen reports ambiguous elements as a parsing diagnostic.
    @{ Name = 'collision-negative'; Enabled = 'true'; Canonical = 'false'; Owned = 'false'; Ambiguous = 'true'; Mappings = 1; FailurePattern = '((MAUIX2000|XC0000).*EventToCommandBehavior|AmbiguousPage.xaml.*error MAUIG1001)' }
)

foreach ($framework in $Frameworks) {
    foreach ($inflator in $Inflators) {
        foreach ($scenario in $scenarios) {
            $name = "$framework-$inflator-$($scenario.Name)"
            $log = Join-Path $LogDirectory "$name.log"
            $arguments = @(
                $project, '-c', 'Release', '-f', $framework, '--verbosity', 'quiet',
                "-p:TargetFrameworks=$framework", "-p:PrismMauiPackageVersion=$PackageVersion",
                "-p:RestoreAdditionalProjectSources=$packagePath", "-p:MauiXamlInflator=$inflator",
                "-p:PrismMauiGlobalXmlns=$($scenario.Enabled)", "-p:CanonicalOnly=$($scenario.Canonical)",
                "-p:OwnGlobalMapping=$($scenario.Owned)", "-p:AmbiguousGlobalNamespace=$($scenario.Ambiguous)"
            )
            & dotnet build @arguments '-t:Rebuild' *> $log
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
            $mappingCount = [regex]::Matches($metadata, 'XmlnsDefinitionAttribute\("http://schemas.microsoft.com/dotnet/maui/global",\s*"http://prismlibrary.com"\)').Count
            if ($mappingCount -ne $scenario.Mappings) { throw "Unexpected mapping count $mappingCount for $name" }
            Write-Output "PASS $name (canonical mappings: $mappingCount)"
        }
    }
}
Write-Output "Logs: $LogDirectory"
exit 0
