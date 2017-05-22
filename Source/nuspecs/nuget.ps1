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

    $expression = ".\$($nugetFileName) pack $($nuspecPath) -outputdirectory $($nugetOutputDirectory) -Prop version=$($fileVersion) -Prop coreVersion=$($fileVersion) -Prop releaseNotes=$($releaseNotesUri)"

    if($wpfVersion)
    {
        $expression = "$expression -Prop wpfVersion=$($wpfFileVersion)"
    }

    if($uwpVersion)
    {
        $expression = "$expression -Prop uwpVersion=$($uwpFileVersion)"
    }

    return $expression
}

function Save-NuGetPackage ($project) {
    $nuspecPath = $project["nuspec"]
    $wpfAssemblyPath = $project["projects"]["wpf"]
    $uwpAssemblyPath = $project["projects"]["uwp"]

    $wpfVersion = Get-FileVersion -assemblyPath $wpfAssemblyPath
    $uwpVersion = Get-FileVersion -assemblyPath $uwpAssemblyPath

    Invoke-Expression | ConvertTo-NuGetExpression -nuspecPath $nuspecPath -wpfVersion $wpfVersion -uwpVersion $uwpVersion
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
$coreFileVersion = Get-FileVersion -assemblyPath $projectsJson["core"]

Write-Host "Is Windows: $IsWindows"
Write-Host "Is CoreCLR: $IsCoreCLR"
Write-Host "Is Linux: $IsLinux"
Write-Host "Is OSX: $IsOSX"

if($IsWindows -or $PSEdition -eq "Desktop")
{
    foreach ($project in $projectsJson["projects"].GetEnumerator()) 
    {
        Write-Host "Building package for $($project.Name)"
        Save-NuGetPackage -project $project.Value
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