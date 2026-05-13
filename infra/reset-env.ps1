param(
    [string]$Prefix = "resumemanager",
    [string]$Location = "canadacentral",
    [string]$ResourceGroup = "ResumeManagerTestRG"
)

Write-Host "Deleting Resource Group (if it exists)..."
az group delete --name $ResourceGroup --yes --no-wait 2>$null

Write-Host "Waiting for the Resource Group to be fully deleted..."

$exists = (az group exists --name $ResourceGroup).Trim()

while ($exists -eq "true") {
    Write-Host "  The Resource Group still exists (Deleting)..."
    Start-Sleep -Seconds 5
    $exists = (az group exists --name $ResourceGroup).Trim()
}

Write-Host "Resource Group deleted."


Write-Host ""
Write-Host "Searching for deleted Key Vaults..."
$deletedKv = az keyvault list-deleted -o json | ConvertFrom-Json | Where-Object { $_.name -like "*$Prefix*" }

if ($deletedKv) {
    Write-Host "Purging deleted Key Vaults..."
    foreach ($kv in $deletedKv) {
        Write-Host "  Purging Key Vault: $($kv.name)"
        az keyvault purge --name $kv.name --location $Location
    }
} else {
    Write-Host "No deleted Key Vaults found."
}

Write-Host ""
Write-Host "Searching for deleted SQL Servers..."
$deletedSql = az sql server list -o json | ConvertFrom-Json | Where-Object { $_.name -like "*$Prefix*" }

if ($deletedSql) {
    Write-Host "Purging SQL Servers..."
    foreach ($sql in $deletedSql) {
        Write-Host "  Force deleting SQL Server: $($sql.name)"
        az sql server delete --name $sql.name --resource-group $ResourceGroup --yes --force 2>$null
    }
} else {
    Write-Host "No active SQL Servers found."
}

Write-Host ""
$storageName = "${Prefix}storage"

Write-Host "Attempting to purge deleted Storage Account: $storageName"
az storage account purge --name $storageName --location $Location 2>$null


Write-Host ""
Write-Host "Environment fully cleaned."
Write-Host "You can now safely redeploy your IaC."
