# Ensure artifacts directory is clean

$executionRoot = Get-Location
$artifactsRoot = $executionRoot, 'artifacts' -join '\'
$binariesRoot = $executionRoot, 'artifacts', 'binaries' -join '\'
$nugetRoot = $executionRoot, 'artifacts', 'nuget' -join '\'
Get-ChildItem .\ -Include $binariesRoot -Recurse | ForEach-Object ($_) { Remove-Item $_.Fullname -Force -Recurse }
Get-ChildItem .\ -Include $nugetRoot -Recurse | ForEach-Object ($_) { Remove-Item $_.Fullname -Force -Recurse }
New-Item -Path $binariesRoot -ItemType Directory -Force > $null
New-Item -Path $nugetRoot -ItemType Directory -Force > $null
$platforms = @('Forms', 'Wpf', 'Uno', 'Maui')

$core = @("Prism.Core", "Prism.Events", "Prism.dll", "Prism.pdb", "Prism.xml")
$allowedExtensions = @('.dll', '.pdb', '.xml', '.pri', '.nupkg', '.snupkg')
$files = Get-ChildItem -Path $artifactsRoot -Filter "*" -Recurse | Where-Object { (Test-Path -Path $_.FullName -PathType Leaf) -and $_.FullName.StartsWith($binariesRoot) -eq $false -and $_.FullName.StartsWith($nugetRoot) -eq $false -and ($allowedExtensions -contains [System.IO.Path]::GetExtension($_.FullName)) }

if ($files.Count -eq 0)
{
    $stringBuilder = New-Object System.Text.StringBuilder
    $stringBuilder.AppendLine("Execution Root: $executionRoot")
    $stringBuilder.AppendLine("Artifacts:")
    Get-ChildItem $artifactsRoot | ForEach-Object { $stringBuilder.AppendLine($_.FullName) }
    $stringBuilder.AppendLine("No files found to copy")
    Write-Error -Message $stringBuilder.ToString()
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
        Write-Output "Copying $($file.FullName) to $nugetRoot"
        Copy-Item $file.FullName $nugetRoot
    }
    elseif ($file -like "*.dll" -or $file -like "*.pdb" -or $file -like "*.xml" -or $file -like "*.pri")
    {
        $parent = Split-Path -Path $file -Parent
        if ($parent -eq $null -or $parent -eq '') {
            Write-Error "Parent is null for $($file.FullName)"
            exit 1
        }
        $parentDirName = Split-Path -Path $parent -Leaf

        if($parentDirName -like ($platforms -join '|') -or $parentDirName -like 'Core')
        {
            continue
        }

        $copyPath = Join-Path $binariesRoot -ChildPath $parentDirName

        if((Test-Path -Path $copyPath -PathType Container) -eq $false) {
            Write-Output "Creating $copyPath"
            New-Item -Path $copyPath -ItemType Directory -Force > $null
        }
        Write-Output "Copying $($file.FullName) to $copyPath"
        Copy-Item $file.FullName $copyPath
    }
}

Get-ChildItem $artifactsRoot | Where-Object { $_.Name -ne 'binaries' -and $_.Name -ne 'nuget' } | ForEach-Object { Remove-Item $_.FullName -Force -Recurse }
