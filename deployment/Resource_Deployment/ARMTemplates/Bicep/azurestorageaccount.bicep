// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

@description('Storage account name, only lowercase letters and numbers, Name must be between 3 and 24 characters')
var tsaStorageName = 'stcontosogoods${take(uniqueString(resourceGroup().id),5)}'

@description('Location for the azure storage account.')
var location = resourceGroup().location

resource tsaStorageName_resource 'Microsoft.Storage/storageAccounts@2021-06-01' ={
  name: tsaStorageName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'

  resource tsaStorageName_resource_default 'blobServices' = {
    name: 'default'
    properties: {
      
    }

    resource tsaStorageName_resource_default_container 'containers' = {
      name: 'giftkey'
      properties: {
        
      }
    }
  }

}

output createdStorageAccName string = tsaStorageName_resource.name

