$resourceGroupName = 'Blockchain_TradableAsset_Accelerator'
$location = 'West US 2'
 $subscriptionID = '8bcea3a6-66c0-4cb4-be1f-21e3b44efaaf'
# $subscriptionID = 'dbd9230d-b929-45a9-99c3-f2c003d9c1b8'
$azurecosmoaccount = 'digitalgoods'
$azurecosmosaccountapp = 'digitalgoods-app'
$storageAccountName = 'digitalgoodstorage' 
$keyvaultname = 'ContosoDigitalGoodsVaults'
$containerregistry = 'ContosoDigitalGoods'
$kubernateservice = 'kubeContosoDigitalGoods'
$azureblockchaineservice = 'member01digitalgoods'
$propertiesservice = '{\"location\":\"westus2\", \"properties\":{\"password\":\"DigitalGoods@1\", \"protocol\":\"Quorum\", \"consortium\":\"digitalgoodsconsotiumsample\", \"consortiumManagementAccountPassword\":\"DigitalGoods@1\"}, \"sku\":{\"name\":\"S0\"}}'

az login
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
   
    # Create a key Vault   
az keyvault create --name $keyvaultname --resource-group $resourceGroupName --location $location --subscription $subscriptionID

 # Create a Container Registry   
az acr create --name $containerregistry --resource-group $resourceGroupName --location $location --subscription $subscriptionID --sku "Standard"

 # Create a member  
az resource create --resource-group $resourceGroupName `
--name $azureblockchaineservice `
--resource-type Microsoft.Blockchain/blockchainMembers `
--is-full-object `
--properties $propertiesservice

 # Create a Aks    
az aks create --name $kubernateservice `
    --resource-group $resourceGroupName `
    --location $location  `
    --generate-ssh-keys `
    --disable-rbac `
    --enable-addons http_application_routing