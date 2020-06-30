#this information need updated with you created with DeployAzure button. the "keyname" dont be changed.
param(
    [Parameter(Mandatory = $True)]
    [string]
    $resourceGroupName = '',

    [Parameter(Mandatory = $True)]
    [string]
    $location = 'westus2',

    [Parameter(Mandatory = $True)]
    [string]
    $subscriptionID = ''
)

#CreateRandom 5 digit not to duplicate DNS Name
$surfix = (Get-Random -Maximum 99999).toString().PadLeft(5, "0")

$kuberneteservice = "contosogoodskub$surfix"
$keyvaultname = "contosogoodskv$surfix"
$keyname = "GiftEncKey"
$keycontainerName = "giftkey"
$keyfilename = "keys.xml"
$azurecosmoaccount = "contosogoodssvcdb$surfix"
$azurecosmosaccountapp = "contosogoodsappdb$surfix"
$storageAccountName = "contosogoodsblob$surfix"
$containerregistry = "contosogoodsacr$surfix"
$azureblockchaineservice = "contosoledger$surfix"
$propertiesservice = '{\"location\":\"westus2\", \"properties\":{\"password\":\"DigitalGoods@1\", \"protocol\":\"Quorum\", \"consortium\":\"digitalgoods\", \"consortiumManagementAccountPassword\":\"DigitalGoods@1\"}, \"sku\":{\"name\":\"S0\"}}'
$azurecosmosMicrosofttokensvc = "abtdb$surfix"


Write-Host "Login Azure subscription....." -ForegroundColor Yellow

az login
az account set --subscription $subscriptionID

#--start comment --#
# Write-Host "Creating Resource group - $resourceGroupName" -ForegroundColor Yellow

az group create `
    --location $location `
    --name $resourceGroupName `
    --subscription $subscriptionID

Write-Host "Creating CosmosDB for Token Service - $azurecosmoaccount"   -ForegroundColor Yellow
# Create a MongoDB API Cosmos DB account with consistent prefix (Local) consistency and multi-master enabled
az cosmosdb create `
    --resource-group $resourceGroupName `
    --name $azurecosmoaccount `
    --kind MongoDB `
    `
    --default-consistency-level "ConsistentPrefix" `
    --enable-multiple-write-locations false `
    --subscription $subscriptionID


Write-Host "Creating CosmosDB for Application - $azurecosmosaccountapp" -ForegroundColor Yellow
# Create a MongoDB API Cosmos DB account
az cosmosdb create `
--resource-group $resourceGroupName `
--name $azurecosmosaccountapp `
--kind MongoDB `
--default-consistency-level "ConsistentPrefix" `
--enable-multiple-write-locations false `
--subscription $subscriptionID

Write-Host "Creating CosmosDB for ABT Token Service - $azurecosmosMicrosofttokensvc" -ForegroundColor Yellow
# Create a MongoDB API Cosmos DB account
az cosmosdb create `
--resource-group $resourceGroupName `
--name $azurecosmosMicrosofttokensvc `
--kind MongoDB `
--default-consistency-level "ConsistentPrefix" `
--enable-multiple-write-locations false `
--subscription $subscriptionID

Write-Host "Creating Storage Account - $storageAccountName" -ForegroundColor Yellow
# Create a Storage  account
az storage account create `
    --location $location `
    --name $storageAccountName `
    --resource-group $resourceGroupName `
    --sku "Standard_LRS" `
    --subscription $subscriptionID

# Get Storage Account Key
$storageAccountKey = az storage account keys list -g $resourceGroupName -n $storageAccountName --query "[?keyName == 'key1'].value" -o tsv

# Create Storage Container
az storage container create --name "$keycontainerName" `
    --account-name $storageAccountName `
    --account-key $storageAccountKey `
    --public-access container

# Create keys.xml file & upload key file to Storage Container
$keycontent = '<?xml version="1.0" encoding="utf-8"?><repository></repository>'
$keycontent | Set-Content $keyfilename
az storage blob upload -f .\$keyfilename `
                            --account-key $storageAccountKey `
                            --account-name $storageAccountName `
                            -c $keycontainerName `
                            -n $keyfilename

# Create Service Principle
$spResult = az ad sp create-for-rbac --skip-assignment --name "contosoretailsp$surfix"
$appid = ($spResult | ConvertFrom-Json).appId
$serviceprinciplesecret = ($spResult | ConvertFrom-Json).password

# Create a Container Registry
Write-Host "Create Container Registry - $containerregistry" -ForegroundColor Yellow
az acr create --name $containerregistry `
                --resource-group $resourceGroupName `
                --location $location `
                --subscription $subscriptionID `
                --sku "Standard"

Write-Host "Creating Kubernetes Cluster - $kuberneteservice" -ForegroundColor Yellow

# Create a Kubernetes Service
az aks create --name $kuberneteservice `
    --resource-group $resourceGroupName `
    --location $location  `
    --kubernetes-version 1.17.5 `
    --node-vm-size Standard_D2_v2 `
    --node-count 1 `
    --service-principal "$appid" `
    --client-secret "$serviceprinciplesecret" `
    --generate-ssh-keys


# Get Container Registry Resource ID
$registryRole = az acr show `
    --name $containerregistry `
    --resource-group $resourceGroupName `
    --query "id" `
    --output tsv

# Assigning the Reader role to Service Principle
az role assignment create --assignee $appid --role Reader --scope $registryRole

Write-Host "Creating KeyVault - $keyvaultname" -ForegroundColor Yellow
# Create a Key Vault
az keyvault create --name $keyvaultname --resource-group $resourceGroupName --location $location --subscription $subscriptionID

Write-Host "Create the key - $keyvaultname" -ForegroundColor Yellow
# Create Key
az keyvault key create --name $keyname --vault-name $keyvaultname

Write-Host "Set Keyvault Policy to Service Principle" -ForegroundColor Yellow
# Update Key Vault Policy to accept Service Principal request
az keyvault set-policy --name $keyvaultname --spn $appId `
    --certificate-permissions backup create delete deleteissuers get getissuers import list listissuers managecontacts manageissuers purge recover restore setissuers update `
    --key-permission backup create decrypt delete encrypt get import list purge recover restore sign unwrapKey update verify wrapKey `
    --secret-permissions backup delete get list purge recover restore set

Write-Host "Deploy Blockhchain network - $azureblockchaineservice" -ForegroundColor Yellow
# Create a member
az resource create --resource-group $resourceGroupName `
--name $azureblockchaineservice `
--resource-type Microsoft.Blockchain/blockchainMembers `
--is-full-object `
--properties $propertiesservice

Write-Host "Get Cosmos DB for Token App connectionstring" -ForegroundColor Yellow
# Update Connection String information to source code
$connectionStringapp = az cosmosdb keys list `
                        --type connection-strings `
                        --name $azurecosmosaccountapp `
                        --resource-group $resourceGroupName `
                        --query "connectionStrings[?contains(description, 'Primary MongoDB Connection String')].[connectionString]" -o tsv

Write-Host "Update Token App connectionstring to DigitalGoods Application API" -ForegroundColor Yellow
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json.template -Raw) `
                -replace '{connectionstring}', $connectionStringapp) `
                | Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json

Write-Host "Update Token App connectionstring to DigitalGoods Setup App" -ForegroundColor Yellow
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.SetUp\appsettings.json.template -Raw) `
                -replace '{appconnectionstring}', $connectionStringapp) `
                | Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.SetUp\appsettings.json

Write-Host "Get Cosmos DB for Token service connectionstring" -ForegroundColor Yellow
$connectionStringsvc = az cosmosdb keys list `
                        --type connection-strings `
                        --name $azurecosmoaccount `
                        --resource-group $resourceGroupName `
                        --query "connectionStrings[?contains(description, 'Primary MongoDB Connection String')].[connectionString]" -o tsv

Write-Host "Update Token service connectionstring to DigitalGoods Token Service API" -ForegroundColor Yellow
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.TokenService.API\appsettings.json.template -Raw) `
-replace '{connectionstring}', $connectionStringsvc) `
| Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.TokenService.API\appsettings.json

Write-Host "Update Token service connectionstring to DigitalGoods Setup App" -ForegroundColor Yellow
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.SetUp\appsettings.json -Raw) `
                -replace '{tokensvcconnectionstring}', $connectionStringsvc) `
                | Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.SetUp\appsettings.json

Write-Host "Get Cosmos DB for Microsoft Azure Token Service connectionstring" -ForegroundColor Yellow
$connectionStringabt  = az cosmosdb keys list `
                        --type connection-strings `
                        --name $azurecosmosMicrosofttokensvc `
                        --resource-group $resourceGroupName `
                        --query "connectionStrings[?contains(description, 'Primary MongoDB Connection String')].[connectionString]" -o tsv

Write-Host "Update Microsoft Azure Token service connectionstring to TokenService API" -ForegroundColor Yellow
((Get-Content -path .\02_Microsoft_Token_Service\src\Microsoft.TokenService.API\appsettings.json.template -Raw) `
-replace '{connectionstring}', $connectionStringabt) `
| Set-Content -Path .\02_Microsoft_Token_Service\src\Microsoft.TokenService.API\appsettings.json

Write-Host "Creating SaS Url for key file in Blob" -ForegroundColor Yellow
# Update KeyFile SaS url
$keyFileSaS = az storage blob generate-sas `
                                    --account-key $storageAccountKey `
                                    --account-name $storageAccountName `
                                    -c $keycontainerName `
                                    -n $keyfilename `
                                    --full-uri

Write-Host "Update Keyfile's Url to DigitalGoods Application API" -ForegroundColor Yellow
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
                -replace '{keyfilesas}', $keyFileSaS.Replace('"','')) `
                | Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json

Write-Host "Get Key with version uri from Azure Keyvault" -ForegroundColor Yellow
# Update Key version from Keyvault
$keyversion = az keyvault key list-versions -n $keyname --vault-name $keyvaultname --query [].kid -o tsv

Write-Host "Update Key with version uri to DigitalGoods Application API" -ForegroundColor Yellow
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
                -replace '{keyversion}', $keyversion) `
                | Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json

Write-Host "Get Keyvault uri from Azure Keyvault" -ForegroundColor Yellow
# Update Keyvault Uri
$keyvaulturi = az keyvault list -g $resourceGroupName --query [0].properties.vaultUri -o tsv

Write-Host "Update Keyvault uri to Microsoft Azure Token service API" -ForegroundColor Yellow
((Get-Content -path .\02_Microsoft_Token_Service\src\Microsoft.TokenService.API\appsettings.json -Raw) `
-replace '{keyvaulturi}', $keyvaulturi) `
| Set-Content -Path .\02_Microsoft_Token_Service\src\Microsoft.TokenService.API\appsettings.json

Write-Host "Update Serviceprinciple informations(ip / secret) to applications" -ForegroundColor Yellow

Write-Host "Update to DigitalGoods App API" -ForegroundColor Yellow
# Update serviceprinciple id
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{serviceprincipleid}', $appid) `
| Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json

# Update serviceprinciple secret
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) `
-replace '{serviceprinciplesecret}', $serviceprinciplesecret) `
| Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.Application.API\appsettings.json

Write-Host "Update to DigitalGoods token service API" -ForegroundColor Yellow
# Update serviceprinciple id
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{serviceprincipleid}', $appid) `
| Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.TokenService.API\appsettings.json

# Update serviceprinciple secret
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{serviceprinciplesecret}', $serviceprinciplesecret) `
| Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.TokenService.API\appsettings.json

Write-Host "Update to Microsoft Azure token service API" -ForegroundColor Yellow
# Update serviceprinciple id
((Get-Content -path .\02_Microsoft_Token_Service\src\Microsoft.TokenService.API\appsettings.json -Raw) `
-replace '{serviceprincipleid}', $appid) `
| Set-Content -Path .\02_Microsoft_Token_Service\src\Microsoft.TokenService.API\appsettings.json

# Update serviceprinciple secret
((Get-Content -path .\02_Microsoft_Token_Service\src\Microsoft.TokenService.API\appsettings.json -Raw) `
-replace '{serviceprinciplesecret}', $serviceprinciplesecret) `
| Set-Content -Path .\02_Microsoft_Token_Service\src\Microsoft.TokenService.API\appsettings.json


Write-Host "Build and push, Deploy Microsoft Azure Token Service" -ForegroundColor Yellow
##Build and push images for ABT Service
Set-Location .\02_Microsoft_Token_Service\src

docker build -f .\Microsoft.TokenService.API\Dockerfile  --rm -t 'microsoft/tokenservice/apiservice' .
docker tag 'microsoft/tokenservice/apiservice' "$containerregistry.azurecr.io/microsoft/tokenservice/apiservice"

az acr update -n $containerregistry --admin-enabled

$password = ( az acr credential show --name $containerregistry | ConvertFrom-Json).passwords.value.Split(" ")[1]





Write-Host "Login to ACS `r`n"
docker login "$containerregistry.azurecr.io" -u $containerregistry -p $password

Write-Host "Push Images to ACS`r`n"
docker push "$containerregistry.azurecr.io/microsoft/tokenservice/apiservice"

Set-Location ..\..

az aks Get-Credentials -g $resourceGroupName -n $kuberneteservice

Write-Host "Preparing Kubernetes Deployment.....`r`n"

((Get-Content -path .\02_Microsoft_Token_Service\deployABT.yml.template -Raw) `
-replace '{acrname}', $containerregistry) | Set-Content -Path .\02_Microsoft_Token_Service\deployABT.yml

((Get-Content -path .\01_Application_Deployment\deployApp.yml.template -Raw) `
-replace '{acrname}', $containerregistry) `
| Set-Content -Path .\01_Application_Deployment\deployApp.yml

((Get-Content -path .\01_Application_Deployment\deployTokenService.yml.template -Raw) `
-replace '{acrname}', $containerregistry) `
| Set-Content -Path .\01_Application_Deployment\deployTokenService.yml

((Get-Content -path .\01_Application_Deployment\DeployResources2.ps1.template -Raw) `
-replace '{acrname}', $containerregistry) `
| Set-Content -Path .\01_Application_Deployment\DeployResources2.ps1

((Get-Content -path .\01_Application_Deployment\DeployResources2.ps1 -Raw) `
-replace '{password}', $password) `
| Set-Content -Path .\01_Application_Deployment\DeployResources2.ps1

((Get-Content -path .\01_Application_Deployment\DeployResources2.ps1 -Raw) `
-replace '{resourceGroupName}', $resourceGroupName) `
| Set-Content -Path .\01_Application_Deployment\DeployResources2.ps1

((Get-Content -path .\01_Application_Deployment\DeployResources2.ps1 -Raw) `
-replace '{kuberneteservice}', $kuberneteservice) `
| Set-Content -Path .\01_Application_Deployment\DeployResources2.ps1

Write-Host "Deploy Services from ACR to Kubernetes.....`r`n"
kubectl create ns microsoft-tokenservice
kubectl apply -f .\02_Microsoft_Token_Service\deployABT.yml --namespace microsoft-tokenservice

#---End Comment --#
$abtpublicip = $null

#wait till public ip has been established
Start-Sleep -s 30

while ($null -eq $abtpublicip) {
    Write-Host "Waiting to be get public address for Microsoft Token Service"
    $abtpublicip = kubectl get svc -n microsoft-tokenservice -o jsonpath="{.items[?(.metadata.name == 'abtoptionb')].status.loadBalancer.ingress[0].ip}"
    Write-Host $abtpublicip
}

#Get Kubernetes Resource information
$kubernetesResourceGroup = az aks show -g $resourceGroupName -n $kuberneteservice

#Get KubernetesNode Resource Information
$kubeNodeResourceGroupName = ($kubernetesResourceGroup | ConvertFrom-Json).nodeResourceGroup

Write-Host "Get public ip network from Azure`r`n"
#Get Public Network
$jmespath = "[?contains(ipAddress, '$abtpublicip')].name"

$abtpublicNetwork = az network public-ip list `
                        -g $kubeNodeResourceGroupName `
                        --query $jmespath -o tsv

Write-Host "Assign A record($abtpublicNetwork) to dns fqdn`r`n"
#CreateRandom 8 digit not to duplicate DNS Name
$abtsurfix = (Get-Random -Maximum 10000000).toString().PadLeft(8, "0")

$publicEndpoint = az network public-ip update --dns-name abtsvc$abtsurfix -n $abtpublicNetwork -g $kubeNodeResourceGroupName --query dnsSettings.fqdn -o tsv

Write-Host "ABT service address is http://$publicEndpoint"

Write-Host "Update Token Service URL to DigitalGoods Token Service API" -ForegroundColor Yellow
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) `
-replace '{tokenserviceendpoint}', $publicEndpoint) `
| Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.TokenService.API\appsettings.json

Write-Host "Update Token Service URL to DigitalGoods Setup App" -ForegroundColor Yellow
((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.SetUp\appsettings.json -Raw) `
                -replace '{tokenserviceendpoint}', $publicEndpoint) `
                | Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.SetUp\appsettings.json

Write-Host "Get Blockchain connection string" -ForegroundColor Yellow
#Get Blockchain TxNode ConnectionString
$blockhcainkey = az resource invoke-action `
                    --resource-group $resourceGroupName `
                    --name $azureblockchaineservice `
                    --action "listApiKeys" `
                    --resource-type Microsoft.Blockchain/blockchainMembers `
                    --query keys[0].value -o tsv

#Get Blockchain dns
$blockchainDNS = az resource show -g $resourceGroupName -n $azureblockchaineservice --resource-type Microsoft.Blockchain/blockchainMembers --query properties.dns -o tsv
$transactionNodeUrl = "https://$blockchainDNS`:3200/$blockhcainkey"

Write-Host "Update Blockchain connection string to DigitalGoods Setup" -ForegroundColor Yellow

((Get-Content -path .\01_Application_Deployment\src\Contoso.DigitalGoods.SetUp\appsettings.json -Raw) `
                -replace '{transactionNodeUri}', $transactionNodeUrl) `
                | Set-Content -Path .\01_Application_Deployment\src\Contoso.DigitalGoods.SetUp\appsettings.json

#Configure Application data and settings
dotnet run -p .\01_Application_Deployment\src\Contoso.DigitalGoods.SetUp\Contoso.Digitalgoods.Setup.csproj

Write-Host "First configuration has been completed...."
Write-Host "Open 2 configuration files - `r`n.\01_Application_Deployment\Contoso.DigitalGoods.Application.API\appsettings.json `
                                            `r`n \.01_Application_Deployment\Contoso.DigitalGoods.TokenService.API\appsettings.json `
                                            `n`r then Update values with thoese values. then try to execute .\deploy2.bat files consequently"



