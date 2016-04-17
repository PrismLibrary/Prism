[CmdletBinding()]
Param(
    [string]$releaseNotes = 'None',
    [bool]$clean = $false
    )

# Constants
$ConfigurationRelease = 'Release'
$ConfigurationSigned = 'Release-Signed'
$ConfigurationFormsTest = 'Test'

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

$artifacts = $nupkgFolder, $binFolder

Import-Module -Force $(Join-Path $modules 'nuget.psm1')
Import-Module -Force $(Join-Path $modules 'common.psm1')

function Clean
{
    $artifacts | % {
        if(Test-Path $_)
        {
            Write-Info "Remove directory '$_'"
            rm -r $_ -Force
        }
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
	& $msbuild $params | Out-Null
    if($LASTEXITCODE -gt 0)
    {
        $projectPath
        return
    }
}

function Run-Tests
{
	param(
		[string]$binFolder,
		[string]$exe
	)

	$tests = Get-ChildItem $binFolder "*Tests*.dll" | % {
        Write-Info "Running test for $_"
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
    $uwpVersion = Get-ProductVersion $(Join-Path $binFolder 'Prism.Windows.dll')
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
            Pack-Nuget $_.FullName $nupkgFolder "version=$version;coreVersion=$coreVersion;wpfVersion=$wpfVersion;formsVersion=$formsVersion;uwpVersion=$uwpVersion;bin=$binFolder;releaseNotes=$releaseNotes"
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
# restore nuget packages for solutions
dir "*.sln" | % {
    Write-Info "Restore nugets for solution $($_.Name)"
    Restore-Nuget $_.FullName
}

$projects = dir "*.csproj" -Recurse

# run all tests
Write-Info "Building test projects"
$errors = @()
$projects | Where-Object -FilterScript { $_.Name -match "Tests?" } | % {
    $configuration = $ConfigurationRelease
    if($_.Name -match "Forms\.Tests?")
    {
        $configuration = $ConfigurationFormsTest
    }
    Write-Info "Building project $($_.Name) with configuration $configuration"
    # TODO(joacar) Testing custom xamarin.forms applications require defining constant TEST
    $errors += Build-Project $_.FullName $binFolder $configuration
}
if($errors)
{
    $errors | % { Write-Error "Failed to build project $(Get-FileName $_)" }
}
Run-Tests $binFolder $xunit

Clean

$errors = @() # clear errors
# Rebuild projects that should be packed using release signed
$projects | Where-Object -FilterScript { $_.Name -notmatch "Tests?" } | % {
    Write-Info "Building project $($_.Name) with configuration $ConfigurationSigned"
    # TODO(joacar) Testing custom xamarin.forms applications require defining constant TEST
    $errors += Build-Project $_.FullName $binFolder $ConfigurationSigned
}

if($errors)
{
    Write-Info "Build projects that could not be built using $ConfigurationSigned"
    $errors |% {
        Write-Info "Build project $(Get-FileName $_) with configuration $ConfigurationRelease"
        Build-Project $_ $binFolder $ConfigurationRelease
    }
}

# TODO(joacar) Zip assemblies (?)

# TODO(joacar) Should just pack the projects whose tests passed
Pack-Nugets

Publish-Nugets $nupkgFolder