$subscriptionID = '8bcea3a6-66c0-4cb4-be1f-21e3b44efaaf'
$keyvaultname = 'ContosoDigitalGoodsV'
$keyname = 'GiftEncKey'
$secretvalue = 'np-3ADurkOSU0hn=@.MtGhnhQEVqqw17'
az login
az account set --subscription $subscriptionID

# az keyvault secret purge --vault-name $keyvaultname --name $keyname
# az keyvault key purge --vault-name $keyvaultname --name $keyname
az keyvault key create --name $keyname --vault-name $keyvaultname

az keyvault secret set --name $keyname --vault-name $keyvaultname --value $secretvalue
