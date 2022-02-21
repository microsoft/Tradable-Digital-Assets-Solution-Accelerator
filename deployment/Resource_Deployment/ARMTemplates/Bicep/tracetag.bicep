
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT licens

@description('create resource with Tradable Digital Trace ID')
resource traceTag_resource 'Microsoft.Resources/deployments@2021-04-01' = {
  name: 'pid-061ff772-6c53-588a-8308-3c4e26553581'
  properties:{
    mode: 'Incremental'
    template:{
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
      contentVersion: '1.0.0.0'
      resources: []
    }
  }
}
