param location string
param prefix string

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
module keyvault './keyvault.bicep' = {
  name: 'kv'
  params: {
    name: '${prefix}-kv'
    location: location
  }
}

module kvSecretApiKey './keyvault-secrets.bicep' = {
  name: 'kvSecretApiKey'
  params: {
    vaultName: keyvault.outputs.name
    secretName: 'resume-manager-api-key'
    secretValue: kvSecretApiKeyValue
  }
}

module kvSecretSignature './keyvault-secrets.bicep' = {
  name: 'kvSecretSignature'
  params: {
    vaultName: keyvault.outputs.name
    secretName: 'resume-manager-signature-secret'
    secretValue: kvSecretSignatureValue
  }
}

module kvSecretSqlAdmin './keyvault-secrets.bicep' = {
  name: 'kvSecretSqlAdmin'
  params: {
    vaultName: keyvault.outputs.name
    secretName: 'sql-admin'
    secretValue: sqlServer.outputs.sqlAdmin
  }
}

module kvSecretSqlAdminPassword './keyvault-secrets.bicep' = {
  name: 'kvSecretSqlAdminPassword'
  params: {
    vaultName: keyvault.outputs.name
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
module webMvc './webapp-mvc.bicep' = {
  name: 'webMvc'
  params: {
    name: '${prefix}-mvc'
    location: location
    planId: plan.outputs.id
  }
}

module webApi './webapp-api.bicep' = {
  name: 'webApi'
  params: {
    name: '${prefix}-api'
    location: location
    planId: plan.outputs.id
  }
}