# Resource Deployment

This folder contains a PowerShell scripts that can be used to provision the Azure resources required to build your Blockchain solution.  You may skip this folder if you prefer to provision your Azure resources via the Azure Portal.  The PowerShell script will provision the following resources to your Azure subscription:

 
| Resource              | Usage                                                                                     |
|-----------------------|-------------------------------------------------------------------------------------------|
|[Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/)  | The user information stored as a document    
|[Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/)  | The catalog, gifts, assets information stored as a document         |
|[Azure Storage Account](https://azure.microsoft.com/en-us/services/storage/?v=18.24) | Data from Gifts and Assets|    
|[Key Vault ](https://azure.microsoft.com/en-us/services/key-vault/) | Store the Secret and Key to Encript Data and generate token for auth and communication   
|[Container Registry ](https://azure.microsoft.com/en-us/services/container-registry/) | Registry for the app in BlockChain  
|[BlockChain ](https://azure.microsoft.com/en-us/services/blockchain-service/)               | The Blockchain Network                                                    |
|[Kubernetes ](https://azure.microsoft.com/en-us/services/kubernetes-service/)               | K8s Service                                                    |

## Prerequisites
1. Access to an Azure Subscription
2. Azure CLI Installed

## Deploy via Azure Portal
As an alternative to running the PowerShell script, you can deploy the resources manually via the Azure Portal or click the button below to deploy the resources:
 

 <a href="https://azuredeploy.net/?repository=https://github.com/microsoft/Tradable-Digital-Assets-Solution-Accelerator/blob/master/00_Resource_Deployment/" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

**Create Resources**

<a href="https://azuredeploy.net/?repository=https://github.com/microsoft/Tradable-Digital-Assets-Solution-Accelerator/blob/master/00_Resource_Deployment/00_DeployDigitalGoods.ps1" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

**Create Secrets**

<a href="https://azuredeploy.net/?repository=https://github.com/microsoft/Tradable-Digital-Assets-Solution-Accelerator/blob/master/00_Resource_Deployment/01_DeploySecrets.ps1" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>


## Steps for Resource Deployment via PowerShell

To run the [PowerShell script](./00_DeployDigitalGoods.ps1):

1. Modify the parameters at the top of **00_DeployDigitalGoods.ps1** to configure the names of your resources and other settings.   
2. Run the [PowerShell script](./00_DeployDigitalGoods.ps1). If you have PowerShell opened to this folder run the command:
`./00_DeployDigitalGoods.ps1`
3. You will then be prompted to login and provide additional information.

**Important:  For the 00_DeployDigitalGoods yo need Check the names and execute once you need be careful to choose the name of KeyVault**


To run the [PowerShell script](./01_DeploySecrets.ps1):

1. Modify the parameters at the top of **01_DeploySecrets.ps1** to configure the names of your resources and other settings.   
2. Run the [PowerShell script](./01_DeploySecrets.ps1). If you have PowerShell opened to this folder run the command:
`./01_DeploySecrets.ps1`
3. You will then be prompted to login and provide additional information.

**Important:  For the 01_DeploySecrets yo need put the same name of the KeyVault before configured and execute once, you can change the name of key and secret but not is recommendable**

