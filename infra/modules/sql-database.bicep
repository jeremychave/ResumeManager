param serverName string
param dbName string
param location string

resource db 'Microsoft.Sql/servers/databases@2024-11-01-preview' = {
  name: '${serverName}/${dbName}'
  location: location
  sku: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 1
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 34359738368
    autoPauseDelay: 60
    minCapacity: json('0.5')
  }
}

output dbName string = dbName
