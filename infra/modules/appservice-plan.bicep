param name string
param location string

resource plan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: name
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
    size: 'F1'
    family: 'F'
    capacity: 0
  }
  kind: 'app'
}

output id string = plan.id
