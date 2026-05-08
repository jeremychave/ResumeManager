param vaultName string
param location string

resource secretApi 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  name: '${vaultName}/resume-manager-api-key'
  location: location
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource secretSig 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  name: '${vaultName}/resume-manager-signature-secret'
  location: location
  properties: {
    attributes: {
      enabled: true
    }
  }
}
