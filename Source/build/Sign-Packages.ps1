$currentDirectory = split-path $MyInvocation.MyCommand.Definition

# See if we have the ClientSecret available
if([string]::IsNullOrEmpty($env:SignClientSecret)){
    Write-Host "Client Secret not found, not signing packages"
    return;
}

dotnet tool install --tool-path . SignClient

# Setup Variables we need to pass into the sign client tool

$appSettings = "$currentDirectory\appsettings.json"
$fileList = "$currentDirectory\filelist.txt"

if (!(Test-Path $fileList))
{
   New-Item -path $currentDirectory -name filelist.txt -type "file" -value "**"
}

# Sanitize GitHub Repository Names
$repoName = $env:BUILD_REPOSITORY_NAME -replace ".*/",""

$azureAd = @{
    SignClient = @{
        AzureAd = @{
            AADInstance = $env:SignClientAADInstance
            ClientId = $env:SignClientClientId
            TenantId = $env:SignClientTenantId
        }
        Service = @{
            Url = $env:SignServiceUrl
            ResourceId = $env:SignServiceResourceId
        }
    }
}

$azureAd | ConvertTo-Json -Compress | Out-File $appSettings

$nupkgs = Get-ChildItem $env:BUILD_ARTIFACTSTAGINGDIRECTORY\*.nupkg -recurse | Select-Object -ExpandProperty FullName

foreach ($nupkg in $nupkgs){
    Write-Host "Submitting $nupkg for signing"

    .\SignClient 'sign' -c $appSettings -i $nupkg -f $fileList -r $env:SignClientUser -s $env:SignClientSecret -n $repoName -d $repoName -u $env:BUILD_REPOSITORY_URI

    Write-Host "Finished signing $nupkg"
}

Write-Host "Sign-package complete"