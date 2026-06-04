targetScope = 'subscription'

param resourceGroupName string = 'ResumeManagerTestRG'
param location string = 'canadacentral'
param prefix string = 'resumemanager'

// Creation of Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

// module containing ressources of Resource Group
module appInfra './modules/app-infra.bicep' = {
  name: 'appInfra'
  scope: rg
  params: {
    location: location
    prefix: prefix
  }
}

output sqlServerName string = appInfra.outputs.sqlServerName
output sqlDatabaseName string = appInfra.outputs.sqlDatabaseName
output identityName string = appInfra.outputs.identityName
output identityObjectId string = appInfra.outputs.identityObjectId
