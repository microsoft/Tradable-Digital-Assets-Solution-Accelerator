// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

@description('Provide Managed Idenity user name.')
param tsaUserIdentityName string = 'ContosoGoodsUserIdentity-${take(uniqueString(resourceGroup().id),5)}'

@description('Location for creating managed identity.')
param location string = resourceGroup().location

resource tsaIdentity_resource 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: tsaUserIdentityName
  location: location
}

output createdTsaUserIdentity string= tsaIdentity_resource.id
output createdTsaUserIdentityId string= tsaIdentity_resource.properties.principalId
output createdTsaUserIdentityClientId string= tsaIdentity_resource.properties.clientId
