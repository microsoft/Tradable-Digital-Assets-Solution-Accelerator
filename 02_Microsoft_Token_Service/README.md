# Application Deployment

After review the previous sections. You have a deployed resources in azure, a source code that now you know how to deploy and run in Visual Studio/Azure and Docker. You have how to access to the endpoints of Solution (Application and Blockchain Actions).

Now in this part you can learn and discover the endpoint of Microsoft Token Service that provide to Blockchain actions the logic to manage Tokens (Aprove, Deploy and Propierties), Service Managment (BlockChain Network, Parties, Users)

## Prerequisites
1. [Postman](https://www.postman.com/)
2. Updated scripts
3. Visual Studio


## How to run the solution

* Open the Solution [MIcrosoft.TokenService.sln]()

* Go to propierties of Solution

 ![Token](./Solution.png)
* Select single start up for Microsoft.TokenService.API project

 ![Token](./Set.png)
* In the webbrowser that Visual Studio Launch go to https://localhost:44386/swagger/index.html

* You can view the actions and consume with the scripts.
![Token](./url.png)

## How to Deploy the solution

Other option is build a image with  build of docker and deploy to the container and we have create a pipeline.

This pipeline can you execute in automatic with azure devops or manually installing Docker and execute all in command line and helping with visual studio.

The steps by step have the next points:

* Build a Image (With a Docker build)
* Push the Image to Container Registry
* Publish the Artifact
* Download the Artifact
* Create a Image
* Deploy Image to Kubernates

In our case we need create this operation for our  services **Microsoft.TokenService.API**

In our project we have the files necesaries to create a pipeline.

A sample of [pipeline](./azure-pipelines.yml) that we use in azure devops. 

This file have dependencies the DockerFiles for each project:

A sample of Docker File build a image are in each project [Microsoft.TokenService.API](./src
/Microsoft.TokenService.API/Dockerfile)

An manifest files that indicates the [Deployment](./manifests/deployment.yml) setings that indicate where to deploy the versions. And [Service](./manifests/service.yml) Settings that indicate the number of nodes.

You can create a Docker Project like our Solution Digital Good to run with image and docker and give more context.

## How to consume the endpoint

Before to consume our [endpoint](http://51.143.111.232/swagger/index.html) we can review the actions.


# Token Actions

  ![Token](./Token.png)

  # Managment Actions 

   ![Managment Service Actions](./ServiceMgn.png)

Now when we see the actions we can test and consume the endpoint with the next step help us with Postman.

1. Open Postman
2. Go to the button Import and select the files from [scripts](./Postman.zip).

***Enviroment File**

_Contoso DigitalGoods Environment.postman_environment.json_.

***Collection File**

 _Contoso DigitalGoods Environment.postman_collection.json_.

  ![Token](./Import.png)
3. Select Collections and select our collection
  ![Token](./Collections.png)
4. Select one operation and enviroment correctly
  ![Token](./Enviroment.png)
5. Press Send button and view the result.

Note: You can configure the enviroment variables to changue the url and more parameters.