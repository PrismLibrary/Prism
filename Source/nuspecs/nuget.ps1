$nugetOutputDirectory = '../Build'

$releaseNotesUri = 'https://github.com/PrismLibrary/Prism/wiki/Release-Notes-'
$coreFileVersion = '1.0.0'
$nugetFileName = 'nuget.exe'

Write-Host "Packing $env:solution_name"

function Get-FileVersion
{
    [OutputType([string])]
    Param ([string]$assemblyPath)

    if(!$assemblyPath)
    {
        return ""
    }

    if((Test-Path $assemblyPath))
    {
        $fileInfo = Get-Item $assemblyPath
        return $fileInfo.VersionInfo.ProductVersion
    }

    throw "Could not locate the assembly '$assemblyPath'"
}

function ConvertTo-NuGetExpression
{
    [OutputType([string])]
    Param (
        [string]$nuspecPath,
        [string]$wpfVersion,
        [string]$uwpVersion
    )

    $fileVersion = $wpfVersion

    if(!$fileVersion)
    {
        $fileVersion = $uwpVersion
    }

    $expression = ".\$($nugetFileName) pack $($nuspecPath) -OutputDirectory $nugetOutputDirectory -Prop 'version=$($fileVersion)' -Prop 'coreVersion=$($fileVersion)' -Prop 'releaseNotes=$($releaseNotesUri)$fileVersion'"

    if($wpfVersion)
    {
        $expression = "$expression -Prop 'wpfVersion=$($wpfVersion)'"
    }

    if($uwpVersion)
    {
        $expression = "$expression -Prop 'uwpVersion=$($uwpVersion)'"
    }

    Write-Host "Finished Expression: $expression"
    return $expression
}

function Save-NuGetPackage ($project) 
{
    $nuspecPath = $project.NuSpec
    $wpfAssemblyPath = $project.Files.Wpf
    $uwpAssemblyPath = $project.Files.UWP

    Write-Host "NuSpec: $nuspecPath"
    Write-Host "WPF Assembly: $wpfAssemblyPath"
    Write-Host "UWP Assembly: $uwpAssemblyPath"

    $wpfVersion = Get-FileVersion -assemblyPath $wpfAssemblyPath
    $uwpVersion = Get-FileVersion -assemblyPath $uwpAssemblyPath

    Write-Host "WPF Version: $wpfVersion"
    Write-Host "UWP Version: $uwpVersion"

    if($wpfVersion -eq '' -and $uwpVersion -eq '')
    {
        Write-Host "Something seems to be wrong, we couldn't locate any binaries for $($project.Name)"
        return
    }

    Invoke-Expression "$(ConvertTo-NuGetExpression -nuspecPath $nuspecPath -wpfVersion $wpfVersion -uwpVersion $uwpVersion)"
}

if(Test-Path ./Source)
{
    $returnPath = "../.."
    Set-Location ./Source/nuspecs
}

if (!(Test-Path $nugetFileName))
{
    Write-Host 'Downloading Nuget.exe ...'
    Invoke-WebRequest -Uri "http://nuget.org/nuget.exe" -OutFile $nugetFileName
}

$projectsJson = Get-Content -Raw -Path projects.json | ConvertFrom-Json
$coreFileVersion = Get-FileVersion -assemblyPath $projectsJson.Core

if($IsWindows -or $PSEdition -eq "Desktop")
{
    foreach ($project in $projectsJson.Projects) 
    {
        Write-Host "Building package for $($project.Name)"
        Save-NuGetPackage -project $project
    }
}
else
{
    Write-Host "This script must be executed on Windows"
}

if($returnPath)
{
    Set-Location $returnPath
}