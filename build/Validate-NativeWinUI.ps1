#Requires -Version 5.1
<#
.SYNOPSIS
Builds the native WinUI packages and rejects direct or transitive Uno dependencies.
.DESCRIPTION
Run on Windows with the .NET SDK and Windows SDK required by the WinUI projects:
    powershell -File .\build\Validate-NativeWinUI.ps1

Restores and builds Prism.DryIoc.WinUI (including its project references) in
Release/x64, validates both native projects' resolved NuGet graphs, then packs
and inspects both native packages. Outputs are retained under artifacts/native-winui.
#>
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ([Environment]::OSVersion.Platform -ne [PlatformID]::Win32NT)
{
    throw 'Native WinUI validation requires Windows to run the Windows App SDK XAML build tools.'
}

$dotnet = (Get-Command dotnet -CommandType Application -ErrorAction Stop).Source
$repositoryRoot = Split-Path -Path $PSScriptRoot -Parent
$projects = @(
    @{ Id = 'Prism.WinUI'; Path = Join-Path $repositoryRoot 'src/WinUI/Prism.WinUI/Prism.WinUI.csproj' },
    @{ Id = 'Prism.DryIoc.WinUI'; Path = Join-Path $repositoryRoot 'src/WinUI/Prism.DryIoc.WinUI/Prism.DryIoc.WinUI.csproj' }
)
$properties = @('-p:Configuration=Release', '-p:Platform=x64')

function Invoke-DotNet
{
    param([Parameter(Mandatory = $true)][string[]]$Arguments)

    Write-Host "dotnet $($Arguments -join ' ')"
    & $dotnet @Arguments
    if ($LASTEXITCODE -ne 0)
    {
        throw "dotnet failed with exit code ${LASTEXITCODE}: $($Arguments -join ' ')"
    }
}

function Assert-NoUnoDependencies
{
    param(
        [AllowEmptyCollection()][string[]]$PackageIds,
        [Parameter(Mandatory = $true)][string]$Source
    )

    $unoPackages = @($PackageIds | Where-Object { $_ -match '^Uno(?:\.|$)' } | Sort-Object -Unique)
    if ($unoPackages.Count -gt 0)
    {
        throw "Uno dependencies found in ${Source}: $($unoPackages -join ', ')"
    }
}

function Test-ProjectAssets
{
    param([Parameter(Mandatory = $true)][string]$Project)

    # Ask MSBuild rather than guessing obj/project.assets.json: projects may relocate obj.
    $output = @(& $dotnet msbuild $Project '-nologo' '-getProperty:ProjectAssetsFile' @properties)
    if ($LASTEXITCODE -ne 0)
    {
        throw "Could not determine ProjectAssetsFile for $Project (exit code $LASTEXITCODE)."
    }

    $assetsPath = ($output -join [Environment]::NewLine).Trim()
    if ([string]::IsNullOrWhiteSpace($assetsPath))
    {
        throw "MSBuild did not return ProjectAssetsFile for $Project."
    }
    if (-not [IO.Path]::IsPathRooted($assetsPath))
    {
        $assetsPath = Join-Path (Split-Path -Path $Project -Parent) $assetsPath
    }
    if (-not (Test-Path -LiteralPath $assetsPath -PathType Leaf))
    {
        throw "Restored assets file does not exist: $assetsPath"
    }

    $assets = Get-Content -LiteralPath $assetsPath -Raw | ConvertFrom-Json
    if ([IO.Path]::GetFullPath($assets.project.restore.projectPath) -ne [IO.Path]::GetFullPath($Project))
    {
        throw "Assets file belongs to another project: $assetsPath"
    }

    # libraries is the full resolved closure, not merely the direct PackageReference list.
    # It includes packages from every restored framework and runtime-specific target.
    $packageIds = @(
        foreach ($library in $assets.libraries.PSObject.Properties)
        {
            if ($library.Value.type -eq 'package')
            {
                ($library.Name -split '/', 2)[0]
            }
        }
    )
    if ($packageIds.Count -eq 0 -or @($assets.targets.PSObject.Properties).Count -eq 0)
    {
        throw "Assets file has no resolved package graph: $assetsPath"
    }

    Assert-NoUnoDependencies -PackageIds $packageIds -Source $assetsPath
    Write-Host "PASS: $assetsPath ($($packageIds.Count) resolved package entries; no Uno dependencies)."
}

function Test-NuGetPackage
{
    param(
        [Parameter(Mandatory = $true)][string]$PackagePath,
        [Parameter(Mandatory = $true)][string]$ExpectedId
    )

    $archive = [IO.Compression.ZipFile]::OpenRead($PackagePath)
    try
    {
        $nuspecs = @($archive.Entries | Where-Object { $_.FullName -like '*.nuspec' })
        if ($nuspecs.Count -ne 1)
        {
            throw "Expected one nuspec in $PackagePath; found $($nuspecs.Count)."
        }

        $reader = [IO.StreamReader]::new($nuspecs[0].Open())
        try
        {
            [xml]$nuspec = $reader.ReadToEnd()
        }
        finally
        {
            $reader.Dispose()
        }

        $metadata = $nuspec.SelectSingleNode('/*[local-name()="package"]/*[local-name()="metadata"]')
        if ($null -eq $metadata -or $metadata.id -ne $ExpectedId)
        {
            throw "Package does not have the expected ID ${ExpectedId}: $PackagePath"
        }
        $packageIds = @(
            $metadata.SelectNodes('*[local-name()="dependencies"]//*[local-name()="dependency"]') |
                ForEach-Object { $_.GetAttribute('id') }
        )
        Assert-NoUnoDependencies -PackageIds $packageIds -Source $PackagePath
        Write-Host "PASS: $PackagePath (nuspec dependencies contain no Uno packages)."
    }
    finally
    {
        $archive.Dispose()
    }
}

foreach ($project in $projects)
{
    if (-not (Test-Path -LiteralPath $project.Path -PathType Leaf))
    {
        throw "Native WinUI project not found: $($project.Path)"
    }
}

Add-Type -AssemblyName System.IO.Compression.FileSystem

# A fresh output directory prevents a previous successful pack from masking a failure.
$outputRoot = Join-Path $repositoryRoot "artifacts/native-winui/$([Guid]::NewGuid().ToString('N'))"
New-Item -Path $outputRoot -ItemType Directory -Force | Out-Null

Push-Location $repositoryRoot
try
{
    $entryProject = $projects[1].Path
    Invoke-DotNet -Arguments (@('restore', $entryProject, '--force') + $properties)
    foreach ($project in $projects)
    {
        Test-ProjectAssets -Project $project.Path
    }

    Invoke-DotNet -Arguments (@('build', $entryProject, '--no-restore', '--no-incremental') + $properties)

    # dotnet pack does not pack project references, so pack both public packages explicitly.
    foreach ($project in $projects)
    {
        $packageOutput = Join-Path $outputRoot $project.Id
        Invoke-DotNet -Arguments (@('pack', $project.Path, '--no-build', '--no-restore', '--output', $packageOutput) + $properties)
        $packages = @(Get-ChildItem -LiteralPath $packageOutput -Filter '*.nupkg' -File |
            Where-Object { $_.Name -notlike '*.symbols.nupkg' })
        if ($packages.Count -ne 1)
        {
            throw "Expected one package for $($project.Id); found $($packages.Count) in $packageOutput."
        }
        Test-NuGetPackage -PackagePath $packages[0].FullName -ExpectedId $project.Id
    }

    Write-Host "Native WinUI Release/x64 validation passed. Packages: $outputRoot"
}
finally
{
    Pop-Location
}
