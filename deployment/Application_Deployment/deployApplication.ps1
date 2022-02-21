# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license.
#login azure
Write-Host "Log in to Azure"
az Login

$subscriptionId = Read-Host "subscription Id"
$resourceGroupName = Read-Host "resource group name"
$containerRegistryName = Read-Host "container registry name"
$kubernetesName = Read-Host "kubernetes name"
$databaseAccountName = Read-Host "database account name"
$managedIdentityId = Read-Host "managed client identity"
$keyStorage = Read-Host "key storage URL"
$keyIdentifier =Read-Host "key identifier URL"
$contosoID = Read-Host "contoso Id"
$partyID = Read-Host "party Id"
$blockchainNetworkID = Read-Host "blockchain network Id"
$tokenID = Read-Host "token Id"
$contosoProductManager = Read-Host "contoso product manager Id"
$nftServiceEndpoint = Read-Host "NFT token service URL"

az account set --subscription $subscriptionId 

$resourceGroup = az group exists -n $resourceGroupName
if ($resourceGroup -eq $false) {
    throw "The Resource group '$resourceGroupName' is not exist`r`n Please check resource name and try it again"
}

Write-Host "Setup Azure Container Registry....."

az acr update --name $containerRegistryName --admin-enabled true

Write-Host "Retrieving configuration setting in Container Registry..."

$acrList = az acr list | Where-Object { $_ -match $containerRegistryName + ".*" }
$loginServer = $acrList[1].Split(":")[1].Replace('"', '').Replace(',', '').Replace(' ', '')
$registryName = $acrList[2].Split(":")[1].Replace('"', '').Replace(',', '').Replace(' ', '')

$userName = $registryName
$password = ( az acr credential show --name $userName | ConvertFrom-Json).passwords.value.Split(" ")[1] 

Write-Host "..... loginServer: '$loginServer'"
Write-Host "..... registryName: '$registryName'"
Write-Host "..... userName: '$userName'"
Write-Host "..... userPassword: '$password'"

Write-Host "Setup Azure Kubernetes Service and Azure Container Service..."

az aks update -n $kubernetesName -g $resourceGroupName --enable-managed-identity

az aks update -n $kubernetesName -g $resourceGroupName --attach-acr $containerRegistryName

Write-Host "Preparing Kubernetes Deployment.....`r`n"

# Set up Deployment yaml file to deploy APIs
((Get-Content -path manifests\deployApplicationAPI.yml.template -Raw) -replace '{acrname}', $containerRegistryName) | Set-Content -Path manifests\deployApplicationAPI.yml
((Get-Content -path manifests\deployTokenService.yml.template -Raw) -replace '{acrname}', $containerRegistryName) | Set-Content -Path manifests\deployTokenService.yml

Write-Host "Deploy applicaitons to the Kubernetes....`r`n"

az aks get-credentials -g $resourceGroupName -n $kubernetesName

kubectl create namespace contoso-digitalgoods
kubectl apply -f .\manifests\deployApplicationAPI.yml --namespace contoso-digitalgoods
kubectl apply -f .\manifests\deployTokenService.yml --namespace contoso-digitalgoods


$appApiPublicIp = $null
$tokenServiceIp = $null

#wait till public ip has been established
Start-Sleep -s 30

while ($null -eq $appApiPublicIp) {
    Write-Host "Waiting to be get public address for Contoso DigitalGoods App"
    $appApiPublicIp = kubectl get svc -n contoso-digitalgoods -o jsonpath="{.items[?(.metadata.name == 'contosodigitalgoodsapp')].status.loadBalancer.ingress[0].ip}"
    Write-Host $appApiPublicIp
}

while ($null -eq $tokenServiceIp) {
    Write-Host "Waiting to be get public address for Contoso DigitalGoods API"
    $tokenServiceIp = kubectl get svc -n contoso-digitalgoods -o jsonpath="{.items[?(.metadata.name == 'contosodigitalgoods')].status.loadBalancer.ingress[0].ip}"
    Write-Host $tokenServiceIp
}

#Get Kubernetes Resource information
$kubernetesResourceGroup = az aks show -g $resourceGroupName -n $kubernetesName

#Get KubernetesNode Resource Information
$kubeNodeResourceGroupName = ($kubernetesResourceGroup | ConvertFrom-Json).nodeResourceGroup

Write-Host "Get public ip network from Azure`r`n"
#Get Public Network
$jmespath = "[?contains(ipAddress, '$appApiPublicIp')].name"
$jmespathTokenServiceAPI = "[?contains(ipAddress, '$tokenServiceIp')].name"

$appApipublicNetwork = az network public-ip list `
                        -g $kubeNodeResourceGroupName `
                        --query $jmespath -o tsv

$appApipublicNetworkAPI = az network public-ip list `
                        -g $kubeNodeResourceGroupName `
                        --query $jmespathTokenServiceAPI -o tsv

Write-Host "Assign A record($appApipublicNetwork) to dns `r`n"
Write-Host "Assign B record($appApipublicNetworkAPI) to dns `r`n"

#CreateRandom 8 digit not to duplicate DNS Name
$ipSurfix = (Get-Random -Maximum 10000000).toString().PadLeft(8, "0")
$ipSurfixAPI = (Get-Random -Maximum 10000000).toString().PadLeft(8, "0")

$publicEndpoint = az network public-ip update --dns-name token$ipSurfix -n $appApipublicNetwork -g $kubeNodeResourceGroupName --query dnsSettings.fqdn -o tsv

$publicEndpointAPI = az network public-ip update --dns-name api$ipSurfixAPI -n $appApipublicNetworkAPI -g $kubeNodeResourceGroupName --query dnsSettings.fqdn -o tsv

Set-location "..\..\src"

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{tokenAPIURL}', $publicEndpointAPI) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{ContosoID}', $contosoID) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{GiftURL}', $publicEndpoint) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{PartyID}', $partyID) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{BlockchainNetworkID}', $blockchainNetworkID) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{KeyStorage}', $keyStorage) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{KeyIdentifier}', $keyIdentifier) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{SubscriptionId}', $subscriptionId) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{ResourceGroupName}', $resourceGroupName) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{DatabaseAccountName}', $databaseAccountName) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{ManagedIdentityId}', $managedIdentityId) `
| Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json


((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{TokenID}', $tokenID) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{ContosoID}', $contosoID) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{ContosoProductManager}', $contosoProductManager) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{NFTServiceEndpoint}', $nftServiceEndpoint) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{PartyID}', $partyID) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{BlockchainNetworkID}', $blockchainNetworkID) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{SubscriptionId}', $subscriptionId) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{ResourceGroupName}', $resourceGroupName) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{DatabaseAccountName}', $databaseAccountName) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{ManagedIdentityId}', $managedIdentityId) `
| Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json

Write-Host "Contoso Application configuration has been updated. now it will be deployed again in Kuberentes"

docker build -f .\Contoso.DigitalGoods.Application.API\Dockerfile  --rm -t 'contoso/digitalgoods/application' .
docker tag 'contoso/digitalgoods/application' "$containerRegistryName.azurecr.io/contoso/digitalgoods/application"

docker build -f .\Contoso.DigitalGoods.TokenService.API\Dockerfile  --rm -t 'contoso/digitalgoods/tokenservice' .
docker tag 'contoso/digitalgoods/tokenservice' "$containerRegistryName.azurecr.io/contoso/digitalgoods/tokenservice"

Set-location "..\deployment\Application_Deployment"

Write-Host "Login to ACS `r`n"
docker login "$containerRegistryName.azurecr.io" -u $userName -p $password

Write-Host "Push Images to ACS`r`n"
docker push "$containerRegistryName.azurecr.io/contoso/digitalgoods/application"
docker push "$containerRegistryName.azurecr.io/contoso/digitalgoods/tokenservice"


kubectl apply -f .\manifests\deployApplicationAPI.yml --namespace contoso-digitalgoods
kubectl apply -f .\manifests\deployTokenService.yml --namespace contoso-digitalgoods

Write-Host "You've successfully deployed Tradable Digital Asset Applicaiton!`r`n" -ForegroundColor Green

Write-Host "Copy Contoso Application APP service address is http://$publicEndpoint   `r`n" -ForegroundColor Green
Write-Host "Copy Contoso Token Service API service address is http://$publicEndpointAPI   `r`n" -ForegroundColor Green

