$PackageReleaseNotes = "https://github.com/PrismLibrary/Prism/releases/tag/v7.2.0.$env:BUILD_BUILDID"

if($env:IS_RELEASE)
{
    Write-Host "Package Release Notes Url set to: $PackageReleaseNotes"
    Write-Output ("##vso[task.setvariable variable=PackageReleaseNotes;]$PackageReleaseNotes")
}
elseif ($env:IS_PREVIEW)
{
    $PackageReleaseNotes = "$PackageReleaseNotes-pre"
    Write-Host "Package Release Notes Url set to: $PackageReleaseNotes"
    Write-Output ("##vso[task.setvariable variable=PackageReleaseNotes;]$PackageReleaseNotes")
}
else
{
    Write-Host "Non-Release build detected"
}