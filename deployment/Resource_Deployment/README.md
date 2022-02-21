# Resource Deployment

This folder contains a PowerShell script that can be used to provision the Azure resources required to build your Blockchain solution.  You may skip this folder if you prefer to provision your Azure resources via the Azure Portal.

The PowerShell script will provision the following resources to your Azure subscription and will also update **appsettings.json** with values required to communicate with these resources.

**Remember to write down all of the output values printed on the screen. These are required when deploying the Contoso Application.**

 
| Resource                                                                              | Usage                                                                                  |
| ------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/)                   | The catalog, gifts, and assets information stored as a document                            |
| [Azure Storage Account](https://azure.microsoft.com/en-us/services/storage/?v=18.24)       | Storage for data related to Gifts and Assets                                                             |
| [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/)                   | Stores the Secret and Key to Encrypt Data and generate token for auth and communication |
| [Azure Container Registry](https://azure.microsoft.com/en-us/services/container-registry/) | Storage for container images related to the blockchain app                                                 |
| [Azure Kubernetes Services](https://azure.microsoft.com/en-us/services/kubernetes-service/)| Manage distributed compute resources                                                                            |

## Prerequisites

1. [Azure Subscription](http://portal.azure.com) - Required to deploy compute resources
2. [PowerShell 7.1](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.1) - Required to run deployment scripts
3. [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) installed - Required to run deployment scripts
4. [User Access Administrator](https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#user-access-administrator) Role - Assigned to the Azure Subscription user

Execute the following steps to deploy Azure resources:

## Step 1. Download Files

Clone or download this repository, if you have not already done so.

Check here for more information on [cloning a repository](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository).

## Step 2. Deploy Azure Resources
1. Run [PowerShell 7.1](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.1)

     **This script will also update appsettings.json with values required to communicate with your resources.**

    **Remember to write down all of the output values printed on the screen. These are required when deploying the application.**

2. Run Change Directory command to Navigate to the Path using the resourcedeployment.ps1 location **deployment/Resource_Deployment/ARMTemplates/Bicep/**
    ```console
    PS C:\Users\>cd <directory path>
    ```

3. Run the **resourcedeployment.ps1** with the following parameters:
```.\resourcedeployment.ps1 <SubscriptionId> <location>```

    ```
    SubscriptionId : The subscription ID for where you want to manage your resources
    location : Azure Data Center Region where resources will be deployed
    ```

    - Stay alert for the possibility of this error when running the PowerShell script:

        ![alt text](/documents/media/Resources/ResourceDeploymentError.png)

    - To resolve the above issue run the following command:
        ```Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass```

    - Then run again ```.\resourcedeployment.ps1 <SubscriptionId> <location>```


    - After the completion of the script, check to see that all of the Azure resources deployed successfully. Your resource groups should look similar to the image below.

        ![alt text](  /documents/media/Resources/Resources.png)


## Step 3. Configure Managed Identity 

**Note: The managed identity name will differ by deployment. Ex. ContosoGoodsUserIdentity-XXXXX**

### Assign Managed Identity to Azure Kubernetes Service
1. Step into the Azure Kubernetes Service VM scale set

    ![alt text]( /documents/media/Resources/AksVmScaleSet.png) 

2. Under settings click on Identity

    ![alt text]( /documents/media/Resources/AksIdentity.png)

3. Click on the User Assigned tab and click on add and select ContosoGoodsUserIdentity-XXXXX

    ![alt text]( /documents/media/Resources/AksAssignIdentity.png)

4. Refresh to confirm identity assignment
 
    ![alt text]( /documents/media/Resources/AksAssignIdentityConfirm.png)

### Assign Managed Identity to Azure Key Vault
1. Step into the key vault

    ![alt text]( /documents/media/Resources/KeyVault.png) 

2. Click on  Access control(IAM) and click on Add role assignment

    ![alt text]( /documents/media/Resources/KeyVaultAccess.png)

3. Search for the "Key Vault Crypto Officer" in the search box given. Select the "Key Vault Crypto Officer" role and click Next

    ![alt text]( /documents/media/Resources/KeyVaultAccessSelect.png)

4. Click on Select members and select ContosoGoodsUserIdentity-XXXXX

    ![alt text]( /documents/media/Resources/KeyVaultAccessSelectID.png)

5. Click on Review + assign to add the assigned role

    ![alt text]( /documents/media/Resources/KeyVaultIdReveiw.png)

6. Refresh to confirm role assignment
 
    ![alt text]( /documents/media/Resources/KeyVaultIdVerify.png)

### Assign Managed Identity to Azure Cosmos DB
1. Step into the Cosmos DB account

    ![alt text]( /documents/media/Resources/CosmosDb.png) 

2. Click on Access control(IAM) and click on Add role assignment

    ![alt text]( /documents/media/Resources/CosmosDBAccess.png)

3. Search for the "DocumentDB Account Contributor" in the search box given. Select the "DocumentDB Account Contributor" role and click Next

    ![alt text]( /documents/media/Resources/CosmosDBAccessSelect.png)

4. Click on the Select members and select ContosoGoodsUserIdentity-XXXXX

    ![alt text]( /documents/media/Resources/CosmosDBAccessSelectID.png)

5. Click on Review + assign to add the assigned role 

    ![alt text]( /documents/media/Resources/CosmosDbIdReveiw.png)

6. Refresh to confirm role assignment
 
    ![alt text]( /documents/media/Resources/CosmosDbIdVerify.png)

### Step 4. Upload file to Azure Storage Account (**required only if you have deployed resources using Azure links**)
1. Step into the Azure Storage Account

    ![alt text]( /documents/media/Resources/StorageAccount.png) 

2. Click on the Containers and click on giftkey

    ![alt text]( /documents/media/Resources/StorageContainer.png)

3. Click on upload. select the **keys.xml** file located in **deployment/Resource_Deployment** and click on upload

    ![alt text]( /documents/media/Resources/StorageUpload.png)

4. Refresh to confirm the file is uploaded

    ![alt text]( /documents/media/Resources/StorageUploadFileVerify.png)

**You've successfully deployed all the resources!**

### Obtain Key Identifier URL

1. Assign yourself key management access permission to see the keys stored in this Key Vault
    ![alt text]( /documents/media/Resources/AzureKeyvault_02.PNG)

2. Step into Azure KeyVault

    ![alt text]( /documents/media/Resources/AzureKeyvault_01.PNG)

3. Click on Keys and then click on the GiftEncKey
    
    ![alt text]( /documents/media/Resources/AzureKeyvault_05.PNG)

4. Click on Current Version
    
    ![alt text]( /documents/media/Resources/AzureKeyvault_06.PNG)

5. Copy **Key Identifier** URL and store it safely. This will be needed later
    
    ![alt text]( /documents/media/Resources/AzureKeyvault_07.PNG)

### Obtain Key Storage URL

1. Step into Azure Storage and click on Container
    
    ![alt text]( /documents/media/Resources/keystorage_01.PNG)

2. Click on giftkey
    
    ![alt text]( /documents/media/Resources/keystorage_02.PNG)

3. Click on keys.xml and the ellipsis (three dots) for additional options
    
    ![alt text]( /documents/media/Resources/keystorage_03.PNG)

4. Click on Generate SAS token and URL 
    
    ![alt text]( /documents/media/Resources/keystorage_04.PNG)

5. Select Read/Write permissions and set the expiry date based on your needs, and click on **Generate SAS token and URL**. Copy the Blob SAS URL and store it safely. This will be required later.
    
    ![alt text]( /documents/media/Resources/keystorage_05.PNG)


For next steps, go to [Application Deployment](/deployment/Application_Deployment/README.md).