<#
Script that signs the DryIoc package assemblies during build if they are not already signed. This is needed becuse a signed assembly can only reference other signed assemblies.
#>
Param
(
    [string]$projectFilePath,
    [string]$projectPath
)
#read project file
$proj = [xml](Get-Content $projectFilePath)

#find DryIoc reference assemblies paths
$dryIocReferencesPaths = $proj | Select-Xml '//*[local-name()="HintPath"]' | where {$_ -like '*Dryioc*'} | % {Join-Path $projectPath $_} | Resolve-Path | % {$_.ToString()}
#Write-Host $dryIocReferencesPaths

#determine NuGet packages base dir
$packagesDir = $dryIocReferencesPaths[0] | % {$_.substring(0, $_.IndexOf("\DryIoc.")) }
#Write-Host $packagesDir

#find DryIoc key file path
foreach ($dryIocReferencesPath in $dryIocReferencesPaths)
{
    $packagePath = $dryIocReferencesPath | % {$_.substring(0, $_.IndexOf("\lib\")) }
    #Write-Host $packagePath
    $dryIocKeyPath = Get-ChildItem $packagePath -Include "DryIoc.snk" -Recurse | % { $_.FullName }
    if ($dryIocKeyPath)
    {
        break
    }
}
#Write-Host $dryIocKeyPath

#find Nivot.StrongNaming package
$nivotFolder = Get-ChildItem $packagesDir "Nivot.StrongNaming*" | % { $_.FullName }
#Write-Host $nivotFolder
#import nivot module
Import-Module (Join-Path $nivotFolder "tools\StrongNaming.psd1")
#import DryIoc key file
$keyPair = Import-StrongNameKeyPair $dryIocKeyPath
#sign all unsigned assemblies
foreach ($dryIocReferencesPath in $dryIocReferencesPaths)
{
   if (-not (Test-StrongName $dryIocReferencesPath))
   {
        Set-StrongName $dryIocReferencesPath -KeyPair $keyPair -Force -NoBackup
        Write-Host "Signed Assembly:" $dryIocReferencesPath
   }
}