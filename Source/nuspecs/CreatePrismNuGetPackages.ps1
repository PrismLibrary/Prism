### This is just the initial script to get the nuget packages out.  We need to refactor this script to make it easier to maintain and update
### One idea is to force a Visual Studio build using the Release-Signed build configuration before packing the nuspecs

$releaseNotesUri = 'https://github.com/PrismLibrary/Prism/wiki/Release-Notes--Jan-10,-2016'

$nugetFileName = 'nuget.exe'

if (!(Test-Path $nugetFileName))
{
    Write-Host 'Downloading Nuget.exe ...'

    (New-Object System.Net.WebClient).DownloadFile('http://nuget.org/nuget.exe', $nugetFileName)
}



##########################
######  Prism.Core  ######
##########################
$coreNuspecPath = 'Prism.Core.nuspec'
$coreAssemblyPath = '../Prism/bin/Release-Signed/Prism.dll'
if ((Test-Path $coreAssemblyPath))
{
    $fileInfo = Get-Item $coreAssemblyPath
    $coreFileVersion = $fileInfo.VersionInfo.ProductVersion

    Invoke-Expression ".\$($nugetFileName) pack $($coreNuspecPath) -Prop version=$($coreFileVersion) -Prop releaseNotes=$($releaseNotesUri)" 
}
else
{
    Write-Host 'Prism.dll not found'
}



###########################
######   Prism.Wpf   ######
###########################
$wpfNuspecPath = 'Prism.Wpf.nuspec'
$wpfAssemblyPath = '../Wpf/Prism.Wpf/bin/Release-Signed/Prism.Wpf.dll'
if ((Test-Path $wpfAssemblyPath))
{
    $fileInfo = Get-Item $wpfAssemblyPath
    $wpfFileVersion = $fileInfo.VersionInfo.ProductVersion

    Invoke-Expression ".\$($nugetFileName) pack $($wpfNuspecPath) -Prop version=$($wpfFileVersion) -Prop coreVersion=$($coreFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}
else
{
    Write-Host 'Prism.Wpf.dll not found'
}



###########################
#####  Prism.Windows  #####
###########################
$uwpNuspecPath = 'Prism.Windows.nuspec'
$uwpAssemblyPath = '../Windows10/Prism.Windows/bin/Release-Signed/Prism.Windows.dll'
if ((Test-Path $uwpAssemblyPath))
{
    $fileInfo = Get-Item $uwpAssemblyPath
    $uwpFileVersion = $fileInfo.VersionInfo.ProductVersion

    Invoke-Expression ".\$($nugetFileName) pack $($uwpNuspecPath) -Prop version=$($uwpFileVersion) -Prop coreVersion=$($coreFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}
else
{
    Write-Host 'Prism.Windows.dll not found'
}



###########################
######  Prism.Forms  ######
###########################
$formsNuspecPath = 'Prism.Forms.nuspec'
$formsAssemblyPath = '../Xamarin/Prism.Forms/bin/Release-Signed/Prism.Forms.dll'
if ((Test-Path $formsAssemblyPath))
{
    $fileInfo = Get-Item $formsAssemblyPath
    $formsFileVersion = $fileInfo.VersionInfo.ProductVersion

    Invoke-Expression ".\$($nugetFileName) pack $($formsNuspecPath) -Prop version=$($formsFileVersion) -Prop coreVersion=$($coreFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}
else
{
    Write-Host 'Prism.Forms.dll not found'
}



###########################
######  Prism.Unity  ######
###########################
$unityNuspecPath = 'Prism.Unity.nuspec'
$unityWpfAssemblyPath = '../Wpf/Prism.Unity.Wpf/bin/Release-Signed/Prism.Unity.Wpf.dll'
$unityUwpAssemblyPath = '../Windows10/Prism.Unity.Windows/bin/Release-Signed/Prism.Unity.Windows.dll'
$unityFormsAssemblyPath = '../Xamarin/Prism.Unity.Forms/bin/Release-Signed/Prism.Unity.Forms.dll'
if (!(Test-Path $unityWpfAssemblyPath))
{
    Write-Host 'Prism.Unity.Wpf.dll not found'
}
elseif (!(Test-Path $unityUwpAssemblyPath))
{
    Write-Host 'Prism.Unity.Windows.dll not found'
}
elseif (!(Test-Path $unityFormsAssemblyPath))
{
    Write-Host 'Prism.Unity.Forms.dll not found'
}
else
{
    ### all assemblies should be versioned the same, so we can just use the first one ###
    $unityWpfFileInfo = Get-Item $unityWpfAssemblyPath
    $unityFileVersion = $unityWpfFileInfo.VersionInfo.ProductVersion   

    Invoke-Expression ".\$($nugetFileName) pack $($unityNuspecPath) -Prop version=$($unityFileVersion) -Prop wpfVersion=$($wpfFileVersion) -Prop uwpVersion=$($uwpFileVersion) -Prop formsVersion=$($formsFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}

###########################
######  Prism.Unity.Forms  ######
###########################
$unityFormsNuspecPath = 'Prism.Unity.Forms.nuspec'
$unityFormsAssemblyPath = '../Xamarin/Prism.Unity.Forms/bin/Release-Signed/Prism.Unity.Forms.dll'
if (!(Test-Path $unityFormsAssemblyPath))
{
    Write-Host 'Prism.Unity.Forms.dll not found'
}
else
{
    $unityFormsFileInfo = Get-Item $unityFormsAssemblyPath
    $unityFileVersion = $unityFormsFileInfo.VersionInfo.ProductVersion   

    Invoke-Expression ".\$($nugetFileName) pack $($unityFormsNuspecPath) -Prop version=$($unityFileVersion) -Prop formsVersion=$($formsFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}



###########################
#####  Prism.Autofac  #####
###########################
$autofacNuspecPath = 'Prism.Autofac.nuspec'
$autofacWpfAssemblyPath = '../Wpf/Prism.Autofac.Wpf/bin/Release-Signed/Prism.Autofac.Wpf.dll'
$autofacUwpAssemblyPath = '../Windows10/Prism.Autofac.Windows/bin/Release-Signed/Prism.Autofac.Windows.dll'
if (!(Test-Path $autofacWpfAssemblyPath))
{
    Write-Host 'Prism.Autofac.Wpf.dll not found'
}
elseif (!(Test-Path $autofacUwpAssemblyPath))
{
    Write-Host 'Prism.Autofac.Windows.dll not found'
}
else
{
    ### all assemblies should be versioned the same, so we can just use the first one ###
    $autofacWpfFileInfo = Get-Item $autofacWpfAssemblyPath
    $autofacFileVersion = $autofacWpfFileInfo.VersionInfo.ProductVersion
    

    Invoke-Expression ".\$($nugetFileName) pack $($autofacNuspecPath) -Prop version=$($autofacFileVersion) -Prop wpfVersion=$($wpfFileVersion) -Prop uwpVersion=$($uwpFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}


##################################
#####  Prism.SimpleInjector  #####
##################################
$simpleInjectorNuspecPath = 'Prism.SimpleInjector.nuspec'
$simpleInjectorUwpAssemblyPath = '../Windows10/Prism.SimpleInjector.Windows/bin/Release-Signed/Prism.SimpleInjector.Windows.dll'
if (!(Test-Path $simpleInjectorUwpAssemblyPath))
{
    Write-Host 'Prism.SimpleInjector.Windows.dll not found'
}
else
{
    $simpleInjectorFileInfo = Get-Item $simpleInjectorUwpAssemblyPath
    $simpleInjectorFileVersion = $simpleInjectorFileInfo.VersionInfo.ProductVersion

    Invoke-Expression ".\$($nugetFileName) pack $($simpleInjectorNuspecPath) -Prop version=$($simpleInjectorFileVersion) -Prop coreVersion=$($coreFileVersion) -Prop uwpVersion=$($uwpFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}


###########################
#######  Prism.Mef  #######
###########################
$mefNuspecPath = 'Prism.Mef.nuspec'
$mefAssemblyPath = '../Wpf/Prism.Mef.Wpf/bin/Release-Signed/Prism.Mef.Wpf.dll'
if ((Test-Path $mefAssemblyPath))
{
    $fileInfo = Get-Item $mefAssemblyPath
    $mefFileVersion = $fileInfo.VersionInfo.ProductVersion

    Invoke-Expression ".\$($nugetFileName) pack $($mefNuspecPath) -Prop version=$($mefFileVersion) -Prop wpfVersion=$($wpfFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}
else
{
    Write-Host 'Prism.Mef.Wpf.dll not found'
}



###########################
#####  Prism.Ninject  #####
###########################
$ninjectNuspecPath = 'Prism.Ninject.nuspec'
$ninjectAssemblyPath = '../Wpf/Prism.Ninject.Wpf/bin/Release-Signed/Prism.Ninject.Wpf.dll'
if ((Test-Path $ninjectAssemblyPath))
{
    $fileInfo = Get-Item $ninjectAssemblyPath
    $ninjectFileVersion = $fileInfo.VersionInfo.ProductVersion

    Invoke-Expression ".\$($nugetFileName) pack $($ninjectNuspecPath) -Prop version=$($ninjectFileVersion) -Prop wpfVersion=$($wpfFileVersion) -Prop formsVersion=$($formsFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}
else
{
    Write-Host 'Prism.Ninject.Wpf.dll not found'
}



###########################
##  Prism.StructureMap  ###
###########################
$structureMapNuspecPath = 'Prism.StructureMap.nuspec'
$structureMapAssemblyPath = '../Wpf/Prism.StructureMap.Wpf/bin/Release-Signed/Prism.StructureMap.Wpf.dll'
if ((Test-Path $structureMapAssemblyPath))
{
    $fileInfo = Get-Item $structureMapAssemblyPath
    $structureMapFileVersion = $fileInfo.VersionInfo.ProductVersion

    Invoke-Expression ".\$($nugetFileName) pack $($structureMapNuspecPath) -Prop version=$($structureMapFileVersion) -Prop wpfVersion=$($wpfFileVersion) -Prop releaseNotes=$($releaseNotesUri)"
}
else
{
    Write-Host 'Prism.StructureMap.Wpf.dll not found'
}



Read-Host 'Press Enter to continue'