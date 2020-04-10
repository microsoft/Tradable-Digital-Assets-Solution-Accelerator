# Tradable Digital Assets Solution Accelerator


## About this repository
This accelerator was built to provide developers with all of the resources needed to quickly build an  Tradable Digital Assets Solution. All below BlockChain paradigm and technology (Tokens, Gifts etc...). This is a fist step that contain manage users, login, catalogs, products and gifts Use this accelerator to jump start your development efforts with Blockchain, Cosmos DB and Azure.

This repository contains the steps, scripts, code, references and tools to create a  blockchain application. 00_Resource_Deployment will create the necessary supporting resources in Azure (Storage, Kubernetes, Key Vault and Cosmos DB). 01_Application_Deployment will deploy and host your application either locally or in your subscription. Refence contains some diagrams about the architecture and procces that execute this solution.

## Prerequisites
In order to successfully complete your solution, you will need to have access to and or provisioned the following:
1. Access to an Azure subscription
2. Visual Studio 2017 or 2019
3. PowerShell
4. Azure Cli

Optional
1. Visual Studio Code

## Azure and Blockchain
The directions provided for this repository assume fundemental working knowledge of Azure, Cosmos DB, Azure Storage, Key Vault, Kubernate Services, and BlockChain. 

For additional training and support, please see:
 1. [Kubernetes](https://kubernetes.io/)
 2. [Blockchain](https://azure.microsoft.com/en-us/solutions/blockchain/)
 3. [KeyVault](https://docs.microsoft.com/en-us/azure/key-vault/basic-concepts)
 4. [Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/introduction)

## Getting Started and Process Overview
Clone/download this repo onto your computer and then walk through each of these folders in order, following the steps outlined in each of the README files.  After completion of all steps, you will have a working solution with the following architecture:

![Microservices Architecture](./References/architecture.JPG)


### [00 - Resource Deployment](./00_Resource_Deployment)
The resources in this folder can be used to deploy the required resources into your Azure Subscription. This can be done either via the [Azure Portal](https://portal.azure.com) or by using the [PowerShell scripts](./00_Resource_Deployment/00_DeployDigitalGoods.ps1 , ./00_Resource_Deployment/01_DeploySecrets.ps1) included in the resource deployment folder.

After deployed, you will have a Cosmos DB account and database, Azure storage, KeyVault, BlockChainService and Kubernetes cluster deployed in your specified resource group.

### [01- Application Deployment](./01_Application_Deployment)
This folder contains the .net Solution that contains the API services that provide the funcionality to Sign up and Login to users, Catalog and Products and Gifts Manage like Digital Assets.

## Links
Hosted Site: [Tradable Digital Assets Solution Accelerator](http://healthcare-apphosting.southcentralus.cloudapp.azure.com/login), you can use scripts to consume the endpoints.

## License
Copyright (c) Microsoft Corporation

All rights reserved.

MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow 