# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license.

param(
    [Parameter(Mandatory= $True,
                HelpMessage='Enter the Azure subscription ID to deploy your resources')]
    [string]
    $subscriptionID = '',

    [Parameter(Mandatory= $True,
                HelpMessage='Enter the Azure Data Center Region to deploy your resources')]
    [string]
    $location = ''
)

Write-Host "Log in to Azure.....`r`n" -ForegroundColor Yellow
az login

az account set --subscription $subscriptionID
Write-Host "Switched subscription to '$subscriptionID' `r`n" -ForegroundColor Yellow

$deploymentName = 'TradableDigitalSADeploy-' + ([string][guid]::NewGuid()).Substring(0,5)

Write-Host "Started deploying Tradable Digital resources.....`r`n" -ForegroundColor Yellow
$deploymentResult = az deployment sub create --template-file .\main.bicep -l $location -n $deploymentName
$joinedString = $deploymentResult -join "" 
$jsonString = ConvertFrom-Json $joinedString

$kubernetesName  = $jsonString.properties.outputs.aksName.value
$containerRegistryName  = $jsonString.properties.outputs.acrName.value
$cosmosName  = $jsonString.properties.outputs.cosmosName.value
$akvUrl  = $jsonString.properties.outputs.akvUrl.value
$resourcegroupName = $jsonString.properties.outputs.tradableDigitalRgName.value
$tsaUserIdentityClientId = $jsonString.properties.outputs.tsaUserIdentityClientId.value
$storageAccountName = $jsonString.properties.outputs.storageAccountName.value

Write-Host "--------------------------------------------`r`n" -ForegroundColor White
Write-Host "Deployment output: `r`n" -ForegroundColor White
Write-Host "Subscription Id: $subscriptionID `r`n" -ForegroundColor Yellow
Write-Host "Tradable Digital resource group: $resourcegroupName `r`n" -ForegroundColor Yellow
Write-Host "Kubernetes Account: $kubernetesName" -ForegroundColor Yellow
Write-Host "Container registry: $containerRegistryName" -ForegroundColor Yellow
Write-Host "Cosmos DB account: $cosmosName" -ForegroundColor Yellow
Write-Host "KeyVault Url: $akvUrl" -ForegroundColor Yellow 
Write-Host "User Assigned Identity Client Id: $tsaUserIdentityClientId " -ForegroundColor Yellow
Write-Host "--------------------------------------------`r`n" -ForegroundColor White

# Get Storage Account Key
$storageAccountKey = az storage account keys list -g $resourceGroupName -n $storageAccountName --query "[?keyName == 'key1'].value" -o tsv

$keyfilename = "keys.xml"
$keycontainerName = "giftkey"

# Create keys.xml file & upload key file to Storage Container
$keycontent = '<?xml version="1.0" encoding="utf-8"?><repository></repository>'
$keycontent | Set-Content $keyfilename
az storage blob upload -f .\$keyfilename `
                            --account-key $storageAccountKey `
                            --account-name $storageAccountName `
                            -c $keycontainerName `
                            -n $keyfilename

Set-location "..\..\..\..\src"
# # Update the settings in console app setting 
((Get-Content -path Contoso.DigitalGoods.SetUp\appsettings.json -Raw) -replace '{SubscriptionId}', $subscriptionID) | Set-Content -Path Contoso.DigitalGoods.SetUp\appsettings.json
((Get-Content -path Contoso.DigitalGoods.SetUp\appsettings.json -Raw) -replace '{ResourceGroupName}', $resourcegroupName) | Set-Content -Path Contoso.DigitalGoods.SetUp\appsettings.json
((Get-Content -path Contoso.DigitalGoods.SetUp\appsettings.json -Raw) -replace '{DatabaseAccountName}', $cosmosName) | Set-Content -Path Contoso.DigitalGoods.SetUp\appsettings.json
((Get-Content -path Contoso.DigitalGoods.SetUp\appsettings.json -Raw) -replace '{ManagedIdentityId}', $tsaUserIdentityClientId) | Set-Content -Path Contoso.DigitalGoods.SetUp\appsettings.json
Write-Host "Updated appsettings.json for Tradable Digital Service.....`r`n" 

Write-Host "All resources are deployed successfully.....`r`n" -ForegroundColor Green