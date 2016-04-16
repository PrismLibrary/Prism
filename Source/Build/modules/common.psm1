function Create-Folder ([string]$folder)
{
	if(-not (Test-Path -Path $folder))
	{
		New-Item $folder -ItemType Directory -Force | Out-Null
	}
}

function Write-Info ([string]$message)
{
	Write-Host $message -ForegroundColor Green
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