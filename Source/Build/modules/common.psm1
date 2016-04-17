function Create-Folder ([string]$folder)
{
	if(-not (Test-Path -Path $folder))
	{
		New-Item $folder -ItemType Directory -Force | Out-Null
	}
}

function Get-FileName ($pathToFile)
{
    Split-Path $pathToFile -Leaf
}

function Write-Warning ([string]$message)
{
    Write-Host $message -ForegroundColor Yellow
}

function Write-Info ([string]$message)
{
	Write-Host $message -ForegroundColor Green
}

function Write-Error ([string]$message)
{
    Write-Host $message -ForegroundColor Red
}

function Run-Test ([string]$binary,	[string]$exe)
{
	Write-Info "Running test for $binary"
	& "$exe" $binary
	if($LASTEXITCODE -ne 0)
	{
		throw "Tests failed for $binary"
	}
}