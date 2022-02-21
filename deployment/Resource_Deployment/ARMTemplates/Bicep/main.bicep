// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

targetScope = 'subscription'


@description('adding prefix to every resource names')
var resourceprefix = '${take(uniqueString(deployment().name),5)}'

resource rgTradableDigital 'Microsoft.Resources/resourceGroups@2020-10-01' = {
  name: 'tradabledigital-${resourceprefix}'
  location: deployment().location
}

module TsaUserIdentityDeploy 'tradablesauseridentity.bicep' = {
  name: 'UserIdentity-${resourceprefix}'
  scope: rgTradableDigital
  params: {
  }
}

module AzContainerRegistryDeploy 'containerregistry.bicep' = {
  name: 'acr-${resourceprefix}'
  scope: rgTradableDigital
  params:{
  }
}

module AzKubernetesClusterDeploy 'azurekubernetesservice.bicep' = {
  name: 'aks-${resourceprefix}'
  scope: rgTradableDigital
  params:{
  }
}

module AzCosmosDBDeploy 'cosmosdb.bicep' = {
  name: 'cosmos-${resourceprefix}'
  scope: rgTradableDigital
  params: {
  }
}

module AzureKeyVaultDeploy 'azurekeyvault.bicep' = {
  name : 'akv-${resourceprefix}'
  scope: rgTradableDigital
  params: {
    tradableDigitalUserIdentityId: TsaUserIdentityDeploy.outputs.createdTsaUserIdentityId
  }
  dependsOn: [
    TsaUserIdentityDeploy
  ]
}

module AzureStorageDeploy 'azurestorageaccount.bicep' = {
  scope: rgTradableDigital
  name: 'st-${resourceprefix}'
}

module tsatrackingID 'tracetag.bicep' = {
  name: 'pid-${resourceprefix}'   
  scope: rgTradableDigital
  params:{ }
}

output aksName string = AzKubernetesClusterDeploy.outputs.createdAksName
output acrName string = AzContainerRegistryDeploy.outputs.createdAcrName
output cosmosName string = AzCosmosDBDeploy.outputs.createdCosmosDBName
output akvUrl string = AzureKeyVaultDeploy.outputs.createdAkvUri
output tradableDigitalRgName string = rgTradableDigital.name
output tsaUserIdentityClientId string = TsaUserIdentityDeploy.outputs.createdTsaUserIdentityClientId
output storageAccountName string = AzureStorageDeploy.outputs.createdStorageAccName
