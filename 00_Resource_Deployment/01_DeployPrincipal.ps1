#this information need updated with you created with DeployAzure button. Hte "keyname" dont be changed.

$resourceGroupName = ''
$location = ''
$subscriptionID = ''
$kuberneteservice = ''
$keyvaultname = ''
$keyname = 'GiftEncKey'
$secretvalue = ''


az login
az account set --subscription $subscriptionID

# Create a Kubernetes Service
az aks create --resource-group $resourceGroupName --name $kuberneteservice --node-count 1 --enable-addons monitoring --generate-ssh-keys

# Create a Key Vault   
az keyvault create --name $keyvaultname --resource-group $resourceGroupName --location $location --subscription $subscriptionID

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

    az keyvault key create --name $keyname --vault-name $keyvaultname

az keyvault secret set --name $keyname --vault-name $keyvaultname --value $secretvalue
 # Check access to Service Principal
#az login --service-principal -u $appId --password $password --tenant $tenant

$credentials | Out-File -FilePath .\Credentials.txt

Write-Host -NoNewLine 'Your deployment has finished, press any key to continue...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');