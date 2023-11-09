$prefix              = 'INH-'+ (Get-Random -Minimum 1000 -Maximum 9999)

$resource_group_name = "Tentamen-Serverside"
$template            = "./function.bicep"

$parameters = @{
    prefix      = $prefix
    serviceTag  = "STBVDB" 
    environment = "D"
    regionTag   = "AZWE"
}

$parameters = $parameters.Keys.ForEach({"$_=$($parameters[$_])"}) -join ' '

Write-Host "Deploying resources in $resource_group_name"

az group create -l westeurope -n $resource_group_name

$cmd = "az deployment group create --mode Incremental --resource-group $resource_group_name --template-file $template --parameters $parameters"
Write-Host $cmd
Invoke-Expression  $cmd