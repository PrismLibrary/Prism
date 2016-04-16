$rootFolder = Split-Path -parent $Script:MyInvocation.MyCommand.Definition
$nuget

Import-Module $(Join-Path $rootFolder 'common.psm1')

function Setup-Nuget
{
	$nugetPath = Join-Path "${env:APPDATA}" 'NuGet'
    $env:nugetPath = $nugetPath
	if(-not (Test-Path $nugetPath)) 
	{
		Create-Folder $nugetPath
	}
	$nugetExe = Join-Path $nugetPath 'nuget.exe'
	if(-not (Test-Path $nugetExe)) 
	{
		Write-Host "Downloading nuget.exe to '$nugetPath' ..."
		Invoke-WebRequest 'http://nuget.org/nuget.exe' -OutFile $nugetExe
	}
	$nuget = $nugetExe
    $nugetExe
}

function Pack-Nuget([string]$nuspec, [string]$outDir,[string]$properties)
{
    if(-not (Test-Path $outDir))
    {
        Create-Folder $outDir
    }
	Write-Info "Packaging $nuspec to $outDir with properties '$properties'"
	$params = @(
		"pack",
		"$nuspec",
		"-o",
		"$outDir",
		"-p",
		"$properties"
	)
	& $nuget $params
}

function Publish-Nugets ([string]$nupkgPath)
{
   $key = Get-Content $(Join-Path $env:nugetPath 'key.txt')
   pushd $nupkgPath
   dir "*.nupkg" | % {
        Upload-Nuget $_.Name $key
   }
   popd
}

function Upload-Nuget ([string]$package, [string]$key)
{
 # http://www.jeremyskinner.co.uk/2011/01/12/automating-nuget-package-creation-with-msbuild-and-powershell/
    $yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Upload the packages."
    $no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "Does not upload the packages."
    $options = [System.Management.Automation.Host.ChoiceDescription[]]($no, $yes)
    $result = $host.ui.PromptForChoice("Upload package", "Do you want to publish the NuGet package '$package'?", $options, 0) 
 
  # Cancelled
  if($result -eq 0) {
    "Upload aborted"
  }
  # upload
  elseif($result -eq 1) 
  {
    Write-Info "Uploading $package"
    & $nuget push -source "http://packages.nuget.org/v1/" $package $key | Out-Null
    if($LASTEXITCODE -gt 0)
    {
        Write-Info "Failed to upload $package"
    }
  }
}

$nuget = Setup-Nuget

Export-ModuleMember Pack-Nuget, Upload-Nuget, Publish-Nugets