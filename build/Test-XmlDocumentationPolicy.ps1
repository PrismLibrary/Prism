param(
    [string] $Framework = 'net10.0'
)

$ErrorActionPreference = 'Stop'
$repository = Split-Path $PSScriptRoot -Parent
$project = Join-Path $repository 'src/Prism.Events/Prism.Events.csproj'
$probe = Join-Path (Split-Path $project -Parent) ("XmlDocumentationPolicyProbe_{0}.cs" -f [Guid]::NewGuid().ToString('N'))
$lateSuppressions = Join-Path ([IO.Path]::GetTempPath()) ("PrismXmlDocumentationSuppressions_{0}.targets" -f [Guid]::NewGuid().ToString('N'))

function Invoke-PolicyBuild {
    param([bool] $SimulateSdkSuppressions = $false)

    $arguments = @('build', $project, '--nologo', '--no-restore', '-m:1', '-f', $Framework,
        '-p:BuildInParallel=false', '-p:GeneratePackageOnBuild=false')
    if ($SimulateSdkSuppressions) {
        $arguments += "-p:CustomAfterMicrosoftCommonTargets=$lateSuppressions"
    }
    $output = & dotnet @arguments 2>&1 | Out-String
    $buildExitCode = $LASTEXITCODE
    [PSCustomObject]@{ ExitCode = $buildExitCode; Output = $output }
}

try {
    # Use a real library project and its normal imports, rather than duplicating
    # WarningsAsErrors in a throwaway project or passing it on the command line.
    $baseline = Invoke-PolicyBuild
    if ($baseline.ExitCode -ne 0) {
        throw "Prism.Events must build before probing documentation policy.`n$($baseline.Output)"
    }

    $cases = @(
        @{ Diagnostic = 'CS1591'; Source = 'namespace Prism.DocumentationPolicyRegression { public sealed class MissingDocumentation { } }' },
        @{ Diagnostic = 'CS1570'; Source = @'
namespace Prism.DocumentationPolicyRegression
{
    /// <summary>Intentionally unclosed XML.
    public sealed class MalformedDocumentation { }
}
'@ }
        @{ Diagnostic = 'CS1573'; Source = @'
namespace Prism.DocumentationPolicyRegression
{
    /// <summary>Provides a parameter-documentation regression probe.</summary>
    public static class ParameterDocumentation
    {
        /// <summary>Accepts two strings to exercise parameter documentation.</summary>
        /// <param name="first">The first string.</param>
        public static void Configure(string first, string second) { }
    }
}
'@ }
    )

    # Reproduce an SDK import adding NoWarn after Directory.Build.props. Include
    # numeric and prefixed forms, while leaving unrelated suppressions intact.
    [IO.File]::WriteAllText($lateSuppressions, @'
<Project>
  <PropertyGroup>
    <NoWarn>$(NoWarn);CS1591;CS1573;1570;CS0219</NoWarn>
  </PropertyGroup>
</Project>
'@)

    $effectiveNoWarn = & dotnet msbuild $project --nologo -t:EnforceXmlDocumentationDiagnostics `
        "-p:TargetFramework=$Framework" "-p:CustomAfterMicrosoftCommonTargets=$lateSuppressions" `
        -getProperty:NoWarn 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0 -or $effectiveNoWarn -notmatch '\bCS0219\b' -or
        $effectiveNoWarn -match '\b(?:CS)?(?:1570|1573|1591)\b') {
        throw "The documentation policy must remove only documentation suppressions.`n$effectiveNoWarn"
    }
    Write-Host 'PASS: The policy preserves unrelated suppressions and removes documentation suppressions.'

    foreach ($simulateSdkSuppressions in @($false, $true)) {
        foreach ($case in $cases) {
            [IO.File]::WriteAllText($probe, $case.Source)
            $result = Invoke-PolicyBuild -SimulateSdkSuppressions $simulateSdkSuppressions
            if ($result.ExitCode -eq 0 -or $result.Output -notmatch "error $($case.Diagnostic)\b") {
                throw "Expected $($case.Diagnostic) to fail the build as an error (SDK suppressions: $simulateSdkSuppressions).`n$($result.Output)"
            }
            Write-Host "PASS: $($case.Diagnostic) fails the library build as an error (SDK suppressions: $simulateSdkSuppressions)."
        }
    }
}
finally {
    if (Test-Path -LiteralPath $probe) {
        Remove-Item -LiteralPath $probe
    }
    if (Test-Path -LiteralPath $lateSuppressions) {
        Remove-Item -LiteralPath $lateSuppressions
    }
}

$restored = Invoke-PolicyBuild
if ($restored.ExitCode -ne 0) {
    throw "Prism.Events did not build after removing the probes.`n$($restored.Output)"
}
Write-Host 'PASS: Prism.Events builds after removing the temporary probes.'
