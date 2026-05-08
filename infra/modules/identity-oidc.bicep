param identityName string

resource oidc 'Microsoft.ManagedIdentity/userAssignedIdentities/federatedIdentityCredentials@2025-01-31-preview' = {
  name: '${identityName}/github-oidc'
  properties: {
    issuer: 'https://token.actions.githubusercontent.com'
    subject: 'repo:jeremychave/ResumeManager:ref:refs/heads/main'
    audiences: [
      'api://AzureADTokenExchange'
    ]
  }
}
