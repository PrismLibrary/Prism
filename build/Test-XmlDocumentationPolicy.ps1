param(
    [string] $Framework = 'net10.0'
)

$ErrorActionPreference = 'Stop'
$repository = Split-Path $PSScriptRoot -Parent
$project = Join-Path $repository 'src/Prism.Events/Prism.Events.csproj'
$probe = Join-Path (Split-Path $project -Parent) ("XmlDocumentationPolicyProbe_{0}.cs" -f [Guid]::NewGuid().ToString('N'))

function Invoke-PolicyBuild {
    $output = & dotnet build $project --nologo --no-restore -m:1 -f $Framework `
        -p:BuildInParallel=false -p:GeneratePackageOnBuild=false 2>&1 | Out-String
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
    )

    foreach ($case in $cases) {
        [IO.File]::WriteAllText($probe, $case.Source)
        $result = Invoke-PolicyBuild
        if ($result.ExitCode -eq 0 -or $result.Output -notmatch "error $($case.Diagnostic)\b") {
            throw "Expected $($case.Diagnostic) to fail the build as an error.`n$($result.Output)"
        }
        Write-Host "PASS: $($case.Diagnostic) fails the library build as an error."
    }
}
finally {
    if (Test-Path -LiteralPath $probe) {
        Remove-Item -LiteralPath $probe
    }
}

$restored = Invoke-PolicyBuild
if ($restored.ExitCode -ne 0) {
    throw "Prism.Events did not build after removing the probes.`n$($restored.Output)"
}
Write-Host 'PASS: Prism.Events builds after removing the temporary probes.'
