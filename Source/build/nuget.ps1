Param(
    [string]$AssembliesPath,
    [string]$AssemblyName,
    [string]$NuspecPath,
    [string]$NugetOutputDirectory,
    [string]$NuGetExe
)

$releaseNotesUri = $env:PackageReleaseNotes
if(!$releaseNotesUri -and $env:APPVEYOR_REPO_COMMIT_MESSAGE_EXTENDED)
{
    $releaseNotesUri = $env:APPVEYOR_REPO_COMMIT_MESSAGE_EXTENDED
}

Write-Host "Packing $AssemblyName"

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

if (!(Test-Path $NuGetExe))
{
    Write-Host 'Downloading Nuget.exe ...'
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $NuGetExe
    while (!(Test-Path $NuGetExe)) { Start-Sleep 3 }
}

if($AssemblyName -eq 'Prism.Wpf')
{
    $coreFileVersion = Get-FileVersion -assemblyPath "$AssembliesPath/Prism.dll"
    $packVersion = Get-FileVersion -assemblyPath "$AssembliesPath/$AssemblyName.dll"
    Write-Host "Prism.Core Version: $coreFileVersion"
    Write-Host "Prism.Wpf Version: $packVersion"
    $expression = "$($NuGetExe) pack $($NuspecPath) -OutputDirectory $NugetOutputDirectory -Prop 'version=$($packVersion)' -Prop 'coreVersion=$($coreFileVersion)' -Prop 'releaseNotes=$($releaseNotesUri)'"
    Invoke-Expression "$expression"
}
else 
{
    $wpfFileVersion = Get-FileVersion -assemblyPath "$AssembliesPath/Prism.Wpf.dll"
    $packVersion = Get-FileVersion -assemblyPath "$AssembliesPath/$AssemblyName.dll"
    Write-Host "Prism.Wpf Version: $wpfFileVersion"
    Write-Host "$AssemblyName Version: $packVersion"
    $expression = "$($NuGetExe) pack $($NuspecPath) -OutputDirectory $NugetOutputDirectory -Prop 'version=$($packVersion)' -Prop 'wpfVersion=$($wpfFileVersion)' -Prop 'releaseNotes=$($releaseNotesUri)'"
    Invoke-Expression "$expression"
}
