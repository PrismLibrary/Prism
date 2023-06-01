# Ensure artifacts directory is clean
Get-ChildItem .\ -Include artifacts -Recurse | ForEach-Object ($_) { Remove-Item $_.Fullname -Force -Recurse }
New-Item -Path artifacts\binaries -ItemType Directory -Force
New-Item -Path artifacts\nuget -ItemType Directory -Force
$platforms = @('Forms', 'Wpf', 'Uno', 'Maui')
$platformNuGet = $platforms | ForEach-Object { Join-Path -Path .\artifacts -ChildPath "$_\NuGet" }
$platformBinary = $platforms | ForEach-Object { Join-Path -Path .\artifacts -ChildPath "$_\Release" }
Get-ChildItem $platformNuGet -Filter '*.nupkg' | Where-Object { $_.Name.StartsWith('Prism.Core') -eq $false -and $_.Name.StartsWith('Prism.Events') -eq $false } | ForEach-Object { if(!(Test-Path -Path $_.FullName)) { Copy-Item $_.FullName .\artifacts\nuget }else { Write-Warning "File already exists: $($_.FullName)" } }
Get-ChildItem $platformNuGet -Filter '*.snupkg' | Where-Object { $_.Name.StartsWith('Prism.Core') -eq $false -and $_.Name.StartsWith('Prism.Events') -eq $false } | ForEach-Object { if(!(Test-Path -Path $_.FullName)) { Copy-Item $_.FullName .\artifacts\nuget }else { Write-Warning "File already exists: $($_.FullName)" } }
Get-ChildItem $platformBinary | Where-Object { $_.Name -ne 'Core' } | ForEach-Object { if(!(Test-Path -Path $_.FullName)) { Copy-Item $_.FullName .\artifacts\binaries }else { Write-Warning "File already exists: $($_.FullName)" } }
Get-ChildItem .\artifacts\Core\NuGet -Filter '*.nupkg' | ForEach-Object { if(!(Test-Path -Path $_.FullName)) { Copy-Item $_.FullName .\artifacts\nuget }else { Write-Warning "File already exists: $($_.FullName)" } }
Get-ChildItem .\artifacts\Core\NuGet -Filter '*.snupkg' | ForEach-Object { if(!(Test-Path -Path $_.FullName)) { Copy-Item $_.FullName .\artifacts\nuget }else { Write-Warning "File already exists: $($_.FullName)" } }
Get-ChildItem .\artifacts\Core\Release | ForEach-Object { if(!(Test-Path -Path $_.FullName)) { Copy-Item $_.FullName .\artifacts\binaries }else { Write-Warning "File already exists: $($_.FullName)" } }

Remove-Item artifacts\Core -Recurse
Remove-Item artifacts\Wpf -Recurse
Remove-Item artifacts\Forms -Recurse
Remove-Item artifacts\Uno -Recurse
Remove-Item artifacts\Maui -Recurse

Get-ChildItem .\artifacts\nuget | ForEach-Object { Write-Host $_.FullName }
