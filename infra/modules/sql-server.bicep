param name string
param location string
param administratorLogin string

@secure()
param sqlAdminPassword string

resource sql 'Microsoft.Sql/servers@2024-11-01-preview' = {
  name: name
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

output name string = sql.name
output id string = sql.id
output sqlAdmin string = administratorLogin
