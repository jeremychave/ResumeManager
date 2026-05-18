param name string
param location string
param planId string
param appSettingResumeManagerApiKey string
param appSettingSignature string
param resumeManagerDbConnectionString string

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
    httpsOnly: false
    siteConfig: {
      numberOfWorkers: 1
      alwaysOn: false
      http20Enabled: true
      appSettings: [
        {
          name: 'ApiSettings__ResumeManagerApiKey'
          value: appSettingResumeManagerApiKey
        }
        {
          name: 'ApiSettings__SignatureSecret'
          value: appSettingSignature
        }
      ]
      connectionStrings: [
        {
          name: 'ResumeManagerDb'
          type: 'SQLServer'
          connectionString: resumeManagerDbConnectionString
        }
      ]
    }
  }
}

output apiUrl string = 'https://${app.properties.defaultHostName}/'
output principalId string = app.identity.principalId