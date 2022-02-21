// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

@minLength(5)
@maxLength(50)
@description('Provide a globally unique name of your Azure Keyvault')
param akvName string = 'akcontosogoods${take(uniqueString(resourceGroup().id),5)}'

@description('Provide a location for Key Vault.')
param location string = resourceGroup().location

@description('Tradable Digital User Managed Identity')
param tradableDigitalUserIdentityId string

param param_tenantId string = subscription().tenantId

resource kv 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: akvName
  location: location
 
  properties: {
    accessPolicies: [
      {
        permissions: {
          keys:[
            'get'
            'wrapKey'
            'unwrapKey'
          ]
        }
        objectId: tradableDigitalUserIdentityId
        tenantId: param_tenantId
      }
    ]
    createMode: 'default'
    enableSoftDelete: true
    sku: {
      family: 'A'
      name: 'standard'
    }
    softDeleteRetentionInDays: 90
    tenantId: param_tenantId
  }

  resource kv_key 'keys' = {
    name: 'GiftEncKey' 
    properties: {
      attributes:{
        enabled: true
      }
      kty:'RSA'
    }
  }
}


output createdAkvUri string = kv.properties.vaultUri
