param name string
param location string

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2025-01-31-preview' = {
  name: name
  location: location
}

output name string = identity.name
output id string = identity.id
output clientId string = identity.properties.clientId
output principalId string = identity.properties.principalId
