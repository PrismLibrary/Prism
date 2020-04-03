Get-ChildItem .\ -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Get-ChildItem .\ -include .mfractor -Attributes Hidden -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Get-ChildItem .\ -include .vs -Attributes Hidden -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Get-ChildItem .\ -include *.csproj.user -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }