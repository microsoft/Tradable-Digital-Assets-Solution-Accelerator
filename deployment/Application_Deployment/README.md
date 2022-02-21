# Application Deployment

## Prerequisites

This solution accelerator relies on the capabilities developed in the [Azure Non-Fungible Token Solution Accelerator](https://github.com/microsoft/Azure-Non-Fungible-Token-Solution-Accelerator). 
Please ensure this is working before you continue.

In order to successfully deploy the Tradable Digital Assets solution, you will also need to deploy the following resources.
 
1. Azure Cosmos DB
2. Azure Storage Account
3. Azure Key Vault
4. Azure Container Registry
5. Azure Kubernetes Service
6. Docker application

If these are not available, please follow the [Resource Deployment](../Resource_Deployment/README.md) steps. 


### Step 1. Run the setup applicaiton
To run the [source code](../../src):

1. Clone/download the [source code](../../src) onto your computer and open the folder in Visual Studio.  

2. Open the [Contoso.DigitalGoodsToken.sln](../../src/Contoso.DigitalGoodsToken.sln).  

3. Navgate to Contoso.DigitalGoods.SetUp > appsettings.json  

4. Replace following properties:
    - SubscriptionId        : The Subscription ID for where you want to manage your resources
    - ResourceGroupName     : Resource group name where the resources are deployed
    - DatabaseAccountName   : Azure Cosmos DB account name
    - ManagedIdentityId     : User Assigned Identity Client Id
    - NFTServiceEndpoint and BlockchainNetworkTxNode : Which are recevied during deployment of the [Azure Non-Fungible Token Solution Accelerator.](https://github.com/microsoft/Azure-Non-Fungible-Token-Solution-Accelerator)  

    **Note: SubscriptionId, ResourceGroupName, DatabaseAccountName, and ManagedIdentityId should have updated automatically, if the PowerShell script was used to deploy Azure resources.**
    **Also, make sure to use jsonRPC port 8545 for BlockchainNetworkTxNode URL.**

5. Once the application runs successfully, the following values will be received.
   - TokenID                    : 0xc2c769884a9681ca1b7ae9b4fd4c199e81828e77
   - ContosoProductManager ID   : 93541834-6108-4a09-8ba9-533f41cad21d
   - Contoso ID                 : 6ff2f723-2280-4d14-8524-eb15abdde040
   - Party Id                   : db03e70b-50d2-472d-a179-4189b7252962
   - Blockchain Id              : d7bf74ae-5bc1-4623-8cf0-6a9a04f29efd
   
   **Note: These values will change every time we the run Contoso.DigitalGoods.SetUp application.**  

6. Copy these values for use during the following steps.

7. Get **keyStorage URL**  and **key Identifier URL** from [Resource Deployment](/deployment/Resource_Deployment/README.md)


### Step 2. Execute the PowerShell deployment script
1. Run [PowerShell 7.1](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.1)
2. Run the Change Directory command to navigate to the nath where **deployApplication.ps1** is present
    ```console
    PS C:\Users\>cd <driectory path>
    ```
3. Run the .\deployApplication.ps1
    ```console
    Powershell.exe -executionpolicy remotesigned -File .\deployApplication.ps1
    ```
4. Accept the log-in request through your browser  

5. Enter the following details in the console when prompted. These are obtained from the previous script(resourcedeployment.ps1) output and steps:
    ```
    subscription Id: The Subscription ID for where you want to manage your resources
    resource group name: Resource group name where the resources are deployed
    container registry name: Azure Container Registry Name
    kubernetes name: Azure Kubernetes Name
    database account name: Azure Cosmos DB Account Name
    managed client identity: User Assigned Identity Client Id
    key storage URL: Azure Key Storage URL
    key identifier URL: Read-Host Azure Key Identifier URL
    contoso Id: Contoso Id recived from the setup applicaiton
    party Id: Party Id recived from the setup applicaiton
    blockchain network Id: Blockchain network Id recived from the setup applicaiton
    token Id: Token Id recived from the setup applicaiton
    contoso product manager Id: Contoso Product Manager Id recived from the setup applicaiton
    NFT token service URL: Azure non fungible token solution accelerator url
    ```
6. After the successful execution of the script, the appliction will be deployed.  
    
    **You've successfully deployed Tradable Digital Asset Application!**
    
    **Note:** At the end of deployment two URLs will be generated, for both of the Tradable Digital Asset applications. Copy these URLs as they will be needed in the next step

For next steps, please go to [**Windows Client Application**](/documents/README.md).

---

## Description of the deployment script (Optional)
1. Login to the  Azure Portal
    ```
    az login
    ```
2. Set the Azure account Subscription ID
    ```
    az account set --subscription mysubscriptionid
    ```
3. Setup Azure Container Registry
    ```
    az acr update --name $containerRegistryName --admin-enabled true
    ```
4. Retrieve the configuration settings from Azure Container Registry
    ```
    $acrList = az acr list | Where-Object { $_ -match $containerRegistryName + ".*" }
    $loginServer = $acrList[1].Split(":")[1].Replace('"', '').Replace(',', '').Replace(' ', '')
    $registryName = $acrList[2].Split(":")[1].Replace('"', '').Replace(',', '').Replace(' ', '')
    $userName = $registryName
    $password = ( az acr credential show --name $userName | ConvertFrom-Json).passwords.value.Split(" ")[1]
    ```
5. Setup Azure Kubernetes Service and Azure Container Service
    ```
    az aks update -n $kubernetesName -g $resourceGroupName --enable-managed-identity
    az aks update -n $kubernetesName -g $resourceGroupName --attach-acr $containerRegistryName
    ```    
6. Setup deployment yaml files to prepare for deployment to Azure Kubernetes
    ```
    ((Get-Content -path manifests\deployApplicationAPI.yml.template -Raw) -replace '{acrname}', $containerRegistryName) | Set-Content -Path manifests\deployApplicationAPI.yml
    ((Get-Content -path manifests\deployTokenService.yml.template -Raw) -replace '{acrname}', $containerRegistryName) | Set-Content -Path manifests\deployTokenService.yml
    ```    
7. Deploy the application to Azure Kubernetes
    ```
    az aks get-credentials -g $resourceGroupName -n $kubernetesName
    kubectl create namespace contoso-digitalgoods
    kubectl apply -f .\manifests\deployApplicationAPI.yml --namespace contoso-digitalgoods
    kubectl apply -f .\manifests\deployTokenService.yml --namespace contoso-digitalgoods
    ```    
8. Generate the public IP address and setup the DNS name for this address
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
    
    $appApipublicNetwork = az network public-ip list -g $kubeNodeResourceGroupName --query $jmespath -o tsv
    
    $appApipublicNetworkAPI = az network public-ip list -g $kubeNodeResourceGroupName --query $jmespathTokenServiceAPI -o tsv
    
    #CreateRandom 8 digit not to duplicate DNS Name
    $ipSurfix = (Get-Random -Maximum 10000000).toString().PadLeft(8, "0")
    $ipSurfixAPI = (Get-Random -Maximum 10000000).toString().PadLeft(8, "0")

    $publicEndpoint = az network public-ip update --dns-name appapi$ipSurfix -n $appApipublicNetwork -g $kubeNodeResourceGroupName --query dnsSettings.fqdn -o tsv

    $publicEndpointAPI = az network public-ip update --dns-name appapi$ipSurfixAPI -n $appApipublicNetworkAPI -g $kubeNodeResourceGroupName --query dnsSettings.fqdn -o tsv
    ```    
9. Prepare the application for deployment
    ```
    Set-location "..\..\src"
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{tokenAPIURL}', $publicEndpointAPI) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{ContosoID}', $contosoID) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{GiftURL}', $publicEndpoint) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{PartyID}', $partyID) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{BlockchainNetworkID}', $blockchainNetworkID) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{KeyStorage}', $keyStorage) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{KeyIdentifier}', $keyIdentifier) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{SubscriptionId}', $subscriptionId) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{ResourceGroupName}', $resourceGroupName) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{DatabaseAccountName}', $databaseAccountName) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.Application.API\appsettings.json -Raw) -replace '{ManagedIdentityId}', $managedIdentityId) | Set-Content -Path .\Contoso.DigitalGoods.Application.API\appsettings.json
    ```
    ```
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{TokenID}', $tokenID) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{ContosoID}', $contosoID) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{ContosoProductManager}', $contosoProductManager) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{NFTServiceEndpoint}', $nftServiceEndpoint) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{PartyID}', $partyID) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{BlockchainNetworkID}', $blockchainNetworkID) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{SubscriptionId}', $subscriptionId) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{ResourceGroupName}', $resourceGroupName) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{DatabaseAccountName}', $databaseAccountName) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ((Get-Content -path .\Contoso.DigitalGoods.TokenService.API\appsettings.json -Raw) -replace '{ManagedIdentityId}', $managedIdentityId) | Set-Content -Path .\Contoso.DigitalGoods.TokenService.API\appsettings.json
    ```
9. Build the application using the Docker file
    ```
    docker build -f .\Contoso.DigitalGoods.Application.API\Dockerfile  --rm -t 'contoso/digitalgoods/application' .
    docker tag 'contoso/digitalgoods/application' "$containerRegistryName.azurecr.io/contoso/digitalgoods/application"
    
    docker build -f .\Contoso.DigitalGoods.TokenService.API\Dockerfile  --rm -t 'contoso/digitalgoods/tokenservice' .
    docker tag 'contoso/digitalgoods/tokenservice' "$containerRegistryName.azurecr.io/contoso/digitalgoods/tokenservice"
    Set-location "..\deployment\Application_Deployment"
    ```
10. Login to Azure Container Registry
    ```
    docker login "$containerRegistryName.azurecr.io" -u $userName -p $password
    ```
11. Push the image to Azure Container Registry
    ```
    docker push "$containerRegistryName.azurecr.io/contoso/digitalgoods/application"
    docker push "$containerRegistryName.azurecr.io/contoso/digitalgoods/tokenservice"
    ```
12. Deploy the application again to Azure Kubernetes   
    ```
    kubectl apply -f .\manifests\deployApplicationAPI.yml --namespace contoso-digitalgoods
    kubectl apply -f .\manifests\deployTokenService.yml --namespace contoso-digitalgoods
    ```