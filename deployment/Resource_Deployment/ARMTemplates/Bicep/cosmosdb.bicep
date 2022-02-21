// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

@description('Cosmos Application DB account name, max length 44 characters, lowercase')
var cosmosAppAccountName = 'cosmos-contosogoodsappdb-${take(uniqueString(resourceGroup().id),5)}'

@description('Location for the Cosmos DB account.')
var location = resourceGroup().location

resource cosmosAppAccountName_resource 'Microsoft.DocumentDB/databaseAccounts@2021-10-15' = {
  name: cosmosAppAccountName
  location: location
  kind:'MongoDB'
  properties: {
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    isVirtualNetworkFilterEnabled: false
    virtualNetworkRules: []
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'ConsistentPrefix'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
    }
    locations: [
      {
        locationName: location
        failoverPriority:0
        isZoneRedundant:false
      }
    ]
    capabilities:[
      {
        name: 'EnableMongo'
      }
    ]
  }
}


output createdCosmosDBName string = cosmosAppAccountName_resource.name
