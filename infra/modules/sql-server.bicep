param name string
param location string

resource sql 'Microsoft.Sql/servers@2024-11-01-preview' = {
  name: name
  location: location
  kind: 'v12.0'
  properties: {
    administratorLogin: 'resumemanageradmin'
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

output name string = sql.name
output id string = sql.id
