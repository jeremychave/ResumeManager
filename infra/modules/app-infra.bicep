param location string
param prefix string

param sqlHost string = environment().suffixes.sqlServerHostname

@secure()
param sqlAdminPassword string = '${newGuid()}-Aa1!'

@secure()
param kvSecretApiKeyValue string = newGuid()

@secure()
param kvSecretSignatureValue string = newGuid()

//
// SQL Server + Database
//
module sqlServer './sql-server.bicep' = {
  name: 'sqlServer'
  params: {
    name: '${prefix}-sql'
    location: location
    administratorLogin: '${prefix}-sqladmin'
    sqlAdminPassword: sqlAdminPassword
  }
}

module sqlDb './sql-database.bicep' = {
  name: 'sqlDb'
  params: {
    serverName: sqlServer.outputs.name
    dbName: '${prefix}db'
    location: location
  }
}

//
// Key Vault
//
resource keyvault 'Microsoft.KeyVault/vaults@2025-05-01' = {
  name: '${prefix}-kv'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    networkAcls: {
      bypass: 'None'
      defaultAction: 'Allow'
      ipRules: []
      virtualNetworkRules: []
    }
    accessPolicies: []
    enableRbacAuthorization: true
    publicNetworkAccess: 'Enabled'
  }
}


module kvSecretApiKey './keyvault-secrets.bicep' = {
  name: 'kvSecretApiKey'
  params: {
    vaultName: keyvault.name
    secretName: 'resume-manager-api-key'
    secretValue: kvSecretApiKeyValue
  }
}

module kvSecretSignature './keyvault-secrets.bicep' = {
  name: 'kvSecretSignature'
  params: {
    vaultName: keyvault.name
    secretName: 'resume-manager-signature-secret'
    secretValue: kvSecretSignatureValue
  }
}

module kvSecretSqlAdmin './keyvault-secrets.bicep' = {
  name: 'kvSecretSqlAdmin'
  params: {
    vaultName: keyvault.name
    secretName: 'sql-admin'
    secretValue: sqlServer.outputs.sqlAdmin
  }
}

module kvSecretSqlAdminPassword './keyvault-secrets.bicep' = {
  name: 'kvSecretSqlAdminPassword'
  params: {
    vaultName: keyvault.name
    secretName: 'sql-admin-password'
    secretValue: sqlAdminPassword
  }
}

//
// Managed Identities
//
module identityMvc './identity.bicep' = {
  name: 'identityMvc'
  params: {
    name: '${prefix}-GitHubAction-mvc'
    location: location
  }
}

module identityfederatedcredentialMvc './identity-federated-credential.bicep' = {
  name: 'identityfederatedcredentialMvc'
  params: {
    identityName: identityMvc.outputs.name
    issuer: 'https://token.actions.githubusercontent.com'
    subject: 'repo:jeremychave/ResumeManager:ref:refs/heads/main'
  }
}

module identityApi './identity.bicep' = {
  name: 'identityApi'
  params: {
    name: '${prefix}-GitHubAction-api'
    location: location
  }
}

module identityfederatedcredentialApi './identity-federated-credential.bicep' = {
  name: 'identityfederatedcredentialApi'
  params: {
    identityName: identityApi.outputs.name
    issuer: 'https://token.actions.githubusercontent.com'
    subject: 'repo:jeremychave/ResumeManager:ref:refs/heads/main'
  }
}

//
// Storage
//
module storage './storage.bicep' = {
  name: 'storage'
  params: {
    name: '${prefix}storage'
    location: location
  }
}

//
// App Service Plan
//
module plan './appservice-plan.bicep' = {
  name: 'plan'
  params: {
    name: '${prefix}-plan'
    location: location
  }
}

//
// Web Apps
//
//module webApi './webapp-api.bicep' = {
//  name: 'webApi'
//  params: {
//    name: '${prefix}-api'
//    location: location
//    planId: plan.outputs.id
//    appSettingResumeManagerApiKey: '@Microsoft.KeyVault(SecretUri=${kvSecretApiKey.outputs.secretUri})'
//    appSettingSignature: '@Microsoft.KeyVault(SecretUri=${kvSecretSignature.outputs.secretUri})'
//    resumeManagerDbConnectionString: 'Server=tcp:${sqlServer.outputs.name}${sqlHost},1433;Initial Catalog=${sqlDb.outputs.dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
//  }
//}

resource webApi 'Microsoft.Web/sites@2024-11-01' = {
  name: '${prefix}-api'
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    serverFarmId: plan.outputs.id
    httpsOnly: false
    siteConfig: {
      numberOfWorkers: 1
      alwaysOn: false
      http20Enabled: true
      appSettings: [
        {
          name: 'ApiSettings__ResumeManagerApiKey'
          value: '@Microsoft.KeyVault(SecretUri=${kvSecretApiKey.outputs.secretUri})'
        }
        {
          name: 'ApiSettings__SignatureSecret'
          value: '@Microsoft.KeyVault(SecretUri=${kvSecretSignature.outputs.secretUri})'
        }
      ]
      connectionStrings: [
        {
          name: 'ResumeManagerDb'
          type: 'SQLServer'
          connectionString: 'Server=tcp:${sqlServer.outputs.name}${sqlHost},1433;Initial Catalog=${sqlDb.outputs.dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
        }
      ]
    }
  }
}


//module webMvc './webapp-mvc.bicep' = {
//  name: 'webMvc'
//  params: {
//    name: '${prefix}-mvc'
//    location: location
//    planId: plan.outputs.id
//    appSettingResumeManagerApiKey: '@Microsoft.KeyVault(SecretUri=${kvSecretApiKey.outputs.secretUri})'
//    appSettingSignature: '@Microsoft.KeyVault(SecretUri=${kvSecretSignature.outputs.secretUri})'
//    resumeManagerApiUrl: webApi.outputs.apiUrl
//  }
//}

resource webMvc 'Microsoft.Web/sites@2024-11-01' = {
  name: '${prefix}-mvc'
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    serverFarmId: plan.outputs.id
    httpsOnly: true
    siteConfig: {
      numberOfWorkers: 1
      alwaysOn: false
      http20Enabled: false
      appSettings: [
        {
          name: 'ApiSettings__ResumeManagerApiKey'
          value: '@Microsoft.KeyVault(SecretUri=${kvSecretApiKey.outputs.secretUri})'
        }
        {
          name: 'ApiSettings__SignatureSecret'
          value: '@Microsoft.KeyVault(SecretUri=${kvSecretSignature.outputs.secretUri})'
        }
        {
          name: 'ApiSettings:ResumeManagerWebApi'
          value: 'https://${webApi.properties.defaultHostName}/'
        }
      ]
    }
  }
}


//
// Role Assignments
//
resource kvSecretsUserApi 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(webApi.name, 'kv-secrets-user')
  scope: keyvault
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '4633458b-17de-408a-b874-0445c86b69e6' // Key Vault Secrets User guid
    )
    principalId: webApi.identity.principalId
  }
}

resource kvSecretsUserMvc 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(webMvc.name, 'kv-secrets-user')
  scope: keyvault
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '4633458b-17de-408a-b874-0445c86b69e6' // Key Vault Secrets User guid
    )
    principalId: webMvc.identity.principalId
  }
}

resource mvcIdentityWebSiteContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(identityMvc.name, 'web-site-contributor')
  scope: webMvc
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      'de139f84-1756-47ae-9be6-808fbbe84772' // Website Contributor
    )
    principalId: identityMvc.outputs.principalId
  }
}

resource apiIdentityWebSiteContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(identityApi.name, 'web-site-contributor')
  scope: webApi
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      'de139f84-1756-47ae-9be6-808fbbe84772' // Website Contributor
    )
    principalId: identityApi.outputs.principalId
  }
}


output webMvcIdentityClientId string = identityMvc.outputs.clientId
output webMvcIdentityPrincipalId string = identityMvc.outputs.principalId
output webApiIdentityClientId string = identityApi.outputs.clientId
output webApiIdentityPrincipalId string = identityApi.outputs.principalId