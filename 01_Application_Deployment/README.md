# Application Deployment

After following all previous steps, you will have a resource group containing a pair of CosmosDB, a Storage Account,kev Vault, BlockChain Servie, Container and a Kubernetes cluster. The Kubernetes cluster will be hosting a our solution.

There are a few modifications to the [Contoso.DigitalGoodsToken.sln](./src/./src/Contoso.DigitalGoodsToken.sln) that need to be made to work with your infrastructure. The connection strings of Cosmos DB and Key Vaults.

<!-- To update the necessary appsettings.json connection strings, run the deploy.ps1 script. -->

## Prerequisites
1. Infrastructure deployed in the folder [00_Resource_Deployment](../00_Resource_Deployment)
2. [Visual Studio](https://visualstudio.microsoft.com/)
3. Updated connection strings

## Options to Run/Deploy our Code

**1.- Local with visual Studio**

# Steps
1. Open [Contoso.DigitalGoodsToken.sln](./src/Contoso.DigitalGoodsToken.sln) in Visual Studio (as Admin)
2. Build Project
3. Run the **Contoso.DigitalGoods.Application.API** project or **Microsoft.Azure.TokenService** project to activate the different endpoints. In other case yo can configure the execution for both projects.

In the Next images we have the specif steps to execute the solution:

* Go to Properties of Solution.

![1 ](./Local/1.png)

* Select Multiple Starup and Select the projects Contoso.DigitalGood.Application.API and Contoso.Digital.TokenService.API.

![2 ](./Local/2.png)

* Run

![3 ](./Local/3.png)

* You can see two Browsers with a message 404. Yo need Modify and delete the word "weatherforecast" and replace by "swagger"

![4 ](./Local/4.png)

* You can see the operations avalilable and test with the scripts or consume in other way

![6 ](./Local/6.png)

![7 ](./Local/7.png)

* You need configure the connections strings, secrets and configure JWT information for each project in the appsetting.json.


**2.- Create a Image and Deploy in our Container to Kubernates**

Other option is build a image with  build of docker and deploy to the container and we have create a pipeline.

This pipeline can you execute in automatic with azure devops or manually installing Docker and execute all in command line and helping with visual studio.

The steps by step have the next points:

* Build a Image (With a Docker build)
* Push the Image to Container Registry
* Publish the Artifact
* Download the Artifact
* Create a Image
* Deploy Image to Kubernates

In our case we need create this operation for our two services **Token.API** and  **Application.API**.

In our project we have the files necesaries to create a pipeline.

A sample of [pipeline](./azure-pipelines.yml) that we use in azure devops. 

This file have dependencies the DockerFiles for each project:

A sample of Docker File build a image are in each project [ApplicationApi](./src/Contoso.DigitalGoods.Application.API/Dockerfile) and [TokenApi](./src/Contoso.DigitalGoods.TokenService.API/Dockerfile)

An manifest files that indicates the [Deployment](./manifests/deployment.yml) setings that indicate where to deploy the versions. And [Service](./manifests/service.yml) Settings that indicate the number of nodes.

The Visual Studio Solution have a project Docker that create a image and launch in Web. Do you need have install Docker Desktop to run this project but you can take like reference to generate files in Azure DevOps.


  ![Docker](./Local/DockerCompose.png)











## Components
This project contains a number of components described below.

| Resource              | Usage                                                                                     |
|-----------------------|-------------------------------------------------------------------------------------------|
| Application Api  | Provide the Actions in the endpoint to manage users, catalogs and gifts        |
| Blockchain Api  |Provide actions to manage Digital Locker, Token Managment and Communicate with Token Service|                                                     |


 # Using the Application  

 
 

  # _Application Api_ Endpoints


To use the services of this application we attach some [scripts](./Scripts.zip) of sample to execute that operation in Postman. Each operation is specified in the next images.

  ![App Endpoints](../Reference/Apis/ApplicationApi.png)


  # Blockchain Service Api

  ![Token Endpoints](../Reference/Apis/BlockchainApi.png)
