param name string
param location string
param planId string

resource app 'Microsoft.Web/sites@2024-11-01' = {
  name: name
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    serverFarmId: planId
    httpsOnly: true
    siteConfig: {
      numberOfWorkers: 1
      alwaysOn: false
      http20Enabled: false
    }
  }
}
