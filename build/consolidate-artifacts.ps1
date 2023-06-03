# Ensure artifacts directory is clean
Get-ChildItem .\ -Include .\artifacts\binaries -Recurse | ForEach-Object ($_) { Remove-Item $_.Fullname -Force -Recurse }
Get-ChildItem .\ -Include .\artifacts\nuget -Recurse | ForEach-Object ($_) { Remove-Item $_.Fullname -Force -Recurse }
New-Item -Path artifacts\binaries -ItemType Directory -Force
New-Item -Path artifacts\nuget -ItemType Directory -Force
$platforms = @('Forms', 'Wpf', 'Uno', 'Maui')

$core = @("Prism.Core", "Prism.Events", "Prism.dll", "Prism.pdb", "Prism.xml")
$files = Get-ChildItem -Path .\artifacts -Filter "*" -Recurse | Where-Object { (Test-Path -Path $_.FullName -PathType Leaf) -and $_.FullName -notmatch "artifacts\\binaries|artifacts\\nuget" }

foreach($file in $files)
{
    if ($file -match ($platforms -join '|') -and $file -match ($core -join '|'))
    {
        continue
    }
    elseif ($file -like "*.nupkg" -or $file -like "*.snupkg")
    {
        Write-Information "Copying $file to .\artifacts\nuget"
        Copy-Item $file.FullName .\artifacts\nuget
    }
    else
    {
        $parent = Split-Path -Path $file -Parent
        $parentDirName = Split-Path -Path $parent -Leaf

        if($parentDirName -like ($platforms -join '|') -or $parentDirName -like 'Core')
        {
            continue
        }

        $copyPath = Join-Path .\artifacts\binaries -ChildPath $parentDirName

        Write-Information "Creating $copyPath"
        New-Item -Path $copyPath -ItemType Directory -Force
        Write-Information "Copying $file to $copyPath"
        Copy-Item $file.FullName $copyPath
    }
}

Remove-Item artifacts\Core -Recurse
Remove-Item artifacts\Wpf -Recurse
Remove-Item artifacts\Forms -Recurse
Remove-Item artifacts\Uno -Recurse
Remove-Item artifacts\Maui -Recurse

Get-ChildItem .\artifacts\nuget | ForEach-Object { Write-Host $_.FullName }
