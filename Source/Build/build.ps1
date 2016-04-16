[CmdletBinding()]
Param(
    [string]$configuration = 'Release-Signed',
    [string]$configurationTest = 'Test',
    [string]$releaseNotes = 'None',
    [bool]$clean = $false
    )

# Declare the folder that powershell is executing in
$buildFolder = Split-Path -parent $Script:MyInvocation.MyCommand.Definition
$sourceFolder = (Get-Item $buildFolder).Parent.FullName;
$modules = Join-Path $buildFolder 'modules'
$packages = Join-Path $sourceFolder 'packages'
$msbuild = Join-Path "${env:ProgramFiles(x86)}" "msbuild\14.0\bin\msbuild.exe"
$nuspecs = Join-Path $sourceFolder 'nuspecs'

# xUnit runner
$xunit = Join-Path $packages 'xunit.runner.console.2.1.0\tools\xunit.console.exe'

# artifacts
$nupkgFolder = $(Join-Path $buildFolder '.nuget')
$binFolder = $(Join-Path $buildFolder 'bin')

Import-Module -Force $(Join-Path $modules 'nuget.psm1')
Import-Module -Force $(Join-Path $modules 'common.psm1')

function Clean
{
    if(Test-Path $nupkgFolder)
    {
        Write-Info "Remove directory $nupkgFolder"
        rm -r $nupkgFolder -Force
    }
    if(Test-Path $binFolder)
    {
        Write-Info "Remove directory $binFolder"
        rm -r $binFolder -Force
    }
}

function Get-ProductVersion ([string]$assemblyPath)
{
    if(-not (Test-Path $assemblyPath))
    {
        Write-Warning "Could not find file '$assemblyPath'"
        return
    }
	(Get-Item $assemblyPath).VersionInfo.ProductVersion
}

function Build-Project
{
	param(
		[string]$projectPath,
		[string]$outputPath,
		[string]$configuration
	)

	Create-Folder $outputPath
	$params = @(
		$projectPath,
		'/nologo',
		"/p:Configuration=$configuration;OutputPath=$outputPath",
		'/m',
		'/v:m',
		'/t:Build'
	)
	& $msbuild $params
}

function Run-Tests
{
	param(
		[string]$binFolder,
		[string]$exe
	)

	$tests = Get-ChildItem $binFolder "*Tests.dll" | % {
        #Run-Test $_.FullName $exe
        Write-Info "Running test for $_.Name"
	    & $exe $_.FullName
	    if($LASTEXITCODE -gt 0)
	    {
		    #throw "Tests failed for $_.FulleName"
	    }
    }
}

function Pack-Nugets {
    # TODO(joacar) How to resolve properties for packing differente nuspec fils
      
    $wpfVersion = Get-ProductVersion $(Join-Path $binFolder 'Prism.Wpf.dll')
    $formsVersion = Get-ProductVersion $(Join-Path $binFolder 'Prism.Forms.dll')
    $coreVersion = Get-ProductVersion $(Join-Path $binFolder 'Prism.dll')
    pushd $nuspecs
    dir "*.nuspec" | % {
        $dll = $_.Name -ireplace '.nuspec', '.dll'
        # Handle Prism.Core.nuspec since it does not match Prism.dll by "convention"
        $dll = $dll -ireplace '.Core', ''
        $version = Get-ProductVersion $(Join-Path $binFolder $dll)
        if(-not $version)
        {
            Write-Error "Can not pack nuget '$_.Name' since file '$dll' could not be found"
        }
        else
        {
            Pack-Nuget $_.FullName $nupkgFolder "version=$version;coreVersion=$coreVersion;wpfVersion=$wpfVersion;formsVersion=$formsVersion;bin=$binFolder;releaseNotes=$releaseNotes"
        }
    }

    popd
}

Clean
if($clean)
{
    # just run clean command
    exit
}
pushd $sourceFolder
# build all solutions
dir "*.sln" | % {
    Restore-Nuget $_.FullName
    Write-Info "Building project $_.Name"
    # TODO(joacar) Testing custom xamarin.forms applications require defining constant TEST
    Build-Project $_.FullName $binFolder $configuration
}
Run-Tests $binFolder $xunit
# TODO(joacar) Should just pack the projects whose tests passed
Pack-Nugets

Publish-Nugets $nupkgFolder