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
        del -Force -Recurse $nupkgFolder
    }
    if(Test-Path $binFolder)
    {
        Write-Info "Remove directory $binFolder"
        del -Force -Recurse $binFolder
    }
}

function Get-ProductVersion
{
	param(
		[string]$assemblyPath
	)
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
    $coreVersion = Get-ProductVersion $(Join-Path $binFolder 'Prism.Forms.dll')
    Pack-Nuget $(Join-Path $buildFolder 'Prism.Unity.Forms.nuspec') $nupkgFolder "version=6.1.0;coreVersion=6.1.0;formsVersion=2.2.0.4;bin=$binFolder;releaseNotes=$releaseNotes"
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
    Write-Info "Building project $_.Name"
    # TODO(joacar) Testing custom xamarin.forms applications require defining constant TEST
    Build-Project $_.FullName $binFolder $configuration
}
Pack-Nugets
Run-Tests $binFolder $xunit
Publish-Nugets $nupkgFolder