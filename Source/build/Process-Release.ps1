$nupkg = Get-ChildItem -Path $env:PIPELINE_WORKSPACE -Filter *.nupkg -Recurse | Select-Object -First 1
$nupkg.Name -match '^(.*?)\.((?:\.?[0-9]+){3,}(?:[-a-z]+)?)\.nupkg$'

$VersionName = $Matches[2]
$artifacts = "$env:PIPELINE_WORKSPACE/Release"

Write-Host "Version Name" $VersionName
Write-Host "DLL Artifacts: $artifacts"

Write-Output ("##vso[task.setvariable variable=VersionName;]$VersionName")
Write-Output ("##vso[task.setvariable variable=DLLArtifactsPath;]$artifacts")
Write-Output ("##vso[task.setvariable variable=DLLArtifactsZip;]$artifacts.zip")