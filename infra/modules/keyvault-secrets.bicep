param vaultName string
param secretName string

@secure()
param secretValue string


resource secret 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  name: '${vaultName}/${secretName}'
  properties: {
    attributes: {
      enabled: true
    }
    value: secretValue
  }
}