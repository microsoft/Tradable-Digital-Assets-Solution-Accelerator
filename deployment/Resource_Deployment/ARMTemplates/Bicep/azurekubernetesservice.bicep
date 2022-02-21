// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

@minLength(5)
@maxLength(50)
@description('Provide a globally unique name of your Azure kubernetes Cluster')
param aksName string = 'akscontosogoods${take(uniqueString(resourceGroup().id),5)}'

@description('Provide a location for aks.')
param location string = resourceGroup().location

resource aks 'Microsoft.ContainerService/managedClusters@2021-10-01' = {
  name: aksName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    dnsPrefix: aksName
    enableRBAC: true
    kubernetesVersion: '1.21.9'
    agentPoolProfiles: [
      {
        name: 'nodepool1'
        count: 1
        vmSize: 'Standard_D2_v2'
        mode: 'System'
      }
    ]
  }
}

output createdAksName string = aksName
