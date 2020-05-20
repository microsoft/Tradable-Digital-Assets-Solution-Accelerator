#this information need updated with you created with DeployAzure button. Hte "keyname" dont be changed.

$resourceGroupName = ''
$location = 'westus2'
$subscriptionID = ''
$kuberneteservice = ''
$keyvaultname = ''
$keyname = ''
$azurecosmoaccount = ''
$azurecosmosaccountapp = ''
$storageAccountName = '' 
$containerregistry = ''
$azureblockchaineservice = ''
$propertiesservice = '{\"location\":\"westus2\", \"properties\":{\"password\":\"DigitalGoods@1\", \"protocol\":\"Quorum\", \"consortium\":\"digitalgoodsconsotiumsample\", \"consortiumManagementAccountPassword\":\"DigitalGoods@1\"}, \"sku\":{\"name\":\"S0\"}}'


#az login
az account set --subscription $subscriptionID

az group create `
    --location $location `
    --name $resourceGroupName `
    --subscription $subscriptionID

# Create a MongoDB API Cosmos DB account with consistent prefix (Local) consistency and multi-master enabled
az cosmosdb create `
    --resource-group $resourceGroupName `
    --name $azurecosmoaccount `
    --kind MongoDB `
    `
    --default-consistency-level "ConsistentPrefix" `
    --enable-multiple-write-locations false `
    --subscription $subscriptionID

    # Create a MongoDB API Cosmos DB account 
az cosmosdb create `
--resource-group $resourceGroupName `
--name $azurecosmosaccountapp `
--kind MongoDB `
--default-consistency-level "ConsistentPrefix" `
--enable-multiple-write-locations false `
--subscription $subscriptionID

# Create a Storage  account 
az storage account create `
    --location $location `
    --name $storageAccountName `
    --resource-group $resourceGroupName `
    --sku "Standard_LRS" `
    --subscription $subscriptionID
	
	# Create a Container Registry   
az acr create --name $containerregistry --resource-group $resourceGroupName --location $location --subscription $subscriptionID --sku "Standard"

 # Create a member  
az resource create --resource-group $resourceGroupName `
--name $azureblockchaineservice `
--resource-type Microsoft.Blockchain/blockchainMembers `
--is-full-object `
--properties $propertiesservice

# Create a Kubernetes Service
az aks create --resource-group $resourceGroupName --name $kuberneteservice --node-count 1 --enable-addons monitoring --generate-ssh-keys

# Create a Key Vault   
az keyvault create --name $keyvaultname --resource-group $resourceGroupName --location $location --subscription $subscriptionID


az keyvault key create --name $keyname --vault-name $keyvaultname


 # Create a Service Principal
$credentials = $(az ad sp create-for-rbac --role "Contributor" --scopes "/subscriptions/$subscriptionID/resourceGroups/$resourceGroupName" -o json)

 # Save SP Credentials
$jsonCredentials = $credentials | ConvertFrom-Json
$appId = $jsonCredentials.appId
$password = $jsonCredentials.password
$tenant = $jsonCredentials.tenant

# Update Key Vault Policy to accept Service Principal request
az keyvault set-policy --name $keyvaultname --spn $appId `
    --certificate-permissions backup create delete deleteissuers get getissuers import list listissuers managecontacts manageissuers purge recover restore setissuers update `
    --key-permission backup create decrypt delete encrypt get import list purge recover restore sign unwrapKey update verify wrapKey `
    --secret-permissions backup delete get list purge recover restore set

 # Check access to Service Principal
#az login --service-principal -u $appId --password $password --tenant $tenant

$credentials | Out-File -FilePath .\Credentials.txt

Write-Host -NoNewLine 'Your deployment has finished, press any key to continue...';
