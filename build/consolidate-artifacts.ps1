# Ensure artifacts directory is clean

$executionRoot = Get-Location
$artifactsRoot = $executionRoot, 'artifacts' -join '\'
$binariesRoot = $executionRoot, 'artifacts', 'binaries' -join '\'
$nugetRoot = $executionRoot, 'artifacts', 'nuget' -join '\'
Get-ChildItem .\ -Include $binariesRoot -Recurse | ForEach-Object ($_) { Remove-Item $_.Fullname -Force -Recurse }
Get-ChildItem .\ -Include $nugetRoot -Recurse | ForEach-Object ($_) { Remove-Item $_.Fullname -Force -Recurse }
New-Item -Path $binariesRoot -ItemType Directory -Force
New-Item -Path $nugetRoot -ItemType Directory -Force
$platforms = @('Forms', 'Wpf', 'Uno', 'Maui')

$core = @("Prism.Core", "Prism.Events", "Prism.dll", "Prism.pdb", "Prism.xml")
$files = Get-ChildItem -Path $artifactsRoot -Filter "*" -Recurse | Where-Object {
    (Test-Path -Path $_.FullName -PathType Leaf) -and $_.FullName -notmatch $binariesRoot -and $_.FullName -notmatch $nugetRoot
}

if ($files.Count -eq 0)
{
    Write-Error "Execution Root: $executionRoot"
    Write-Error "Artifacts: $(Get-ChildItem $artifactsRoot)"
    Write-Error "No files found to copy"
    exit 1
}

foreach($file in $files)
{
    if ($file -match ($platforms -join '|') -and $file -match ($core -join '|'))
    {
        # Ignore Prism.Core / Prism.Events built with platforms
        continue
    }
    elseif ($file -like "*.nupkg" -or $file -like "*.snupkg")
    {
        Write-Host "Copying $file to $nugetRoot"
        Copy-Item $file.FullName $nugetRoot
    }
    else
    {
        $parent = Split-Path -Path $file -Parent
        $parentDirName = Split-Path -Path $parent -Leaf

        if($parentDirName -like ($platforms -join '|') -or $parentDirName -like 'Core')
        {
            continue
        }

        $copyPath = Join-Path $binariesRoot -ChildPath $parentDirName

        Write-Host "Creating $copyPath"
        New-Item -Path $copyPath -ItemType Directory -Force
        Write-Host "Copying $file to $copyPath"
        Copy-Item $file.FullName $copyPath
    }
}

Get-ChildItem $artifactsRoot | Where-Object { $_.Name -ne 'binaries' -and $_.Name -ne 'nuget' } | ForEach-Object { Remove-Item $_.FullName -Force -Recurse }
