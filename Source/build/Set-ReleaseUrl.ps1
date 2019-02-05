if($env:IS_RELEASE)
{
    $PackageReleaseNotes = "https://github.com/PrismLibrary/Prism/releases/tag/v7.2.0.$env:IS_RELEASE"
    Write-Host "Package Release Notes Url set to: $PackageReleaseNotes"
    Write-Output ("##vso[task.setvariable variable=PackageReleaseNotes;]$PackageReleaseNotes")
}
elseif ($env:IS_PREVIEW)
{
    $PackageReleaseNotes = "https://github.com/PrismLibrary/Prism/releases/tag/v7.2.0.$env:IS_PREVIEW-pre"
    Write-Host "Package Release Notes Url set to: $PackageReleaseNotes"
    Write-Output ("##vso[task.setvariable variable=PackageReleaseNotes;]$PackageReleaseNotes")
}
else
{
    Write-Host "Non-Release build detected"
}