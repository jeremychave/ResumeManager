param prefix string = 'resumemanager'
param location string = resourceGroup().location

//
// Key Vault
//
module keyvault './modules/keyvault.bicep' = {
  name: 'kv'
  params: {
    name: '${prefix}-kv'
    location: location
  }
}

module kvSecrets './modules/keyvault-secrets.bicep' = {
  name: 'kvSecrets'
  params: {
    vaultName: keyvault.outputs.name
    location: location
  }
}

//
// Managed Identities
//
module identity './modules/identity.bicep' = {
  name: 'identity'
  params: {
    name: '${prefix}-identity'
    location: location
  }
}

module identityOidc './modules/identity-oidc.bicep' = {
  name: 'identityOidc'
  params: {
    identityName: identity.outputs.name
  }
}

//
// SQL Server + Database
//
module sqlServer './modules/sql-server.bicep' = {
  name: 'sqlServer'
  params: {
    name: '${prefix}-sql'
    location: location
  }
}

module sqlDb './modules/sql-database.bicep' = {
  name: 'sqlDb'
  params: {
    serverName: sqlServer.outputs.name
    dbName: '${prefix}db'
    location: location
  }
}

//
// Storage
//
module storage './modules/storage.bicep' = {
  name: 'storage'
  params: {
    name: '${prefix}storage'
    location: location
  }
}

//
// App Service Plan
//
module plan './modules/appservice-plan.bicep' = {
  name: 'plan'
  params: {
    name: '${prefix}-plan'
    location: location
  }
}

//
// Web Apps
//
module webMvc './modules/webapp-mvc.bicep' = {
  name: 'webMvc'
  params: {
    name: '${prefix}-mvc'
    location: location
    planId: plan.outputs.id
  }
}

module webApi './modules/webapp-api.bicep' = {
  name: 'webApi'
  params: {
    name: '${prefix}-api'
    location: location
    planId: plan.outputs.id
  }
}
