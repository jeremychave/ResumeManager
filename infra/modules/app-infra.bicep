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
resource sqlServer 'Microsoft.Sql/servers@2024-11-01-preview' = {
  name: '${prefix}-sql'
  location: location
  properties: {
    administratorLogin: '${prefix}-sqladmin'
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

module sqlDb './sql-database.bicep' = {
  name: 'sqlDb'
  params: {
    serverName: sqlServer.name
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
    secretValue: '${prefix}-sqladmin'
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
module identityGitHubAction './identity.bicep' = {
  name: 'identityGitHubAction'
  params: {
    name: '${prefix}-GitHubAction'
    location: location
  }
}

module identityfederatedcredential './identity-federated-credential.bicep' = {
  name: 'identityfederatedcredential'
  params: {
    identityName: identityGitHubAction.outputs.name
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
          connectionString: 'Server=tcp:${sqlServer.name}${sqlHost},1433;Initial Catalog=${sqlDb.outputs.dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
        }
      ]
    }
  }
}

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
  name: guid(identityGitHubAction.name, 'web-site-contributor')
  scope: webMvc
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      'de139f84-1756-47ae-9be6-808fbbe84772' // Website Contributor
    )
    principalId: identityGitHubAction.outputs.principalId
  }
}

resource apiIdentityWebSiteContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(identityGitHubAction.name, 'web-site-contributor')
  scope: webApi
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      'de139f84-1756-47ae-9be6-808fbbe84772' // Website Contributor
    )
    principalId: identityGitHubAction.outputs.principalId
  }
}

resource dbIdentitySqlDbContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(identityGitHubAction.name, 'sql-db-contributor')
  scope: sqlServer
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '9b7fa17d-e63e-47b0-bb0a-15c516ac86ec' // SQL DB Contributor
    )
    principalId: identityGitHubAction.outputs.principalId
  }
}