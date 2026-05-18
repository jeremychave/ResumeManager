param name string
param location string
param planId string
param appSettingResumeManagerApiKey string
param appSettingSignature string
param resumeManagerApiUrl string

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
      appSettings: [
        {
          name: 'ApiSettings__ResumeManagerApiKey'
          value: appSettingResumeManagerApiKey
        }
        {
          name: 'ApiSettings__SignatureSecret'
          value: appSettingSignature
        }
        {
          name: 'ApiSettings:ResumeManagerWebApi'
          value: resumeManagerApiUrl
        }
      ]
    }
  }
}

output principalId string = app.identity.principalId
