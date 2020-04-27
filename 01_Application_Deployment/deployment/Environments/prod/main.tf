
provider "azurerm" {
  # whilst the `version` attribute is optional, we recommend pinning to a given version of the Provider
  version         = "=2.0.0"
  features {}
  subscription_id = "f7c5fdda-08da-4dcb-b8c5-56198420fd7f"
  tenant_id       = "72f988bf-86f1-41af-91ab-2d7cd011db47"
}
locals {
  azure_env_key = var.environment_keys.production
  azure_region_key = var.location_keys.azure-us
  service_group_name = "ntest"
  service_name = "web-api"
  nike_env = var.environment_types.production
  vnet_block = "172.31.0.0" # Use this space if not to be routable to NIKE WHQ main campus
  vnet_mask = 20 # Gives us 16 class 6 to work with
  subnet_block = "172.31.0.0"
  subnet_mask = 24
  location = var.location
  subscription_id = "f7c5fdda-08da-4dcb-b8c5-56198420fd7f"
  tenant_id       = "72f988bf-86f1-41af-91ab-2d7cd011db47"
  common_tags = { 
        CostCenter: var.nike_costcenter
        Owner: var.nike_owner
        OwningGroup: var.nike_owning_group
        DataClass: var.nike_data_classification
        nike-environment: var.nike_env
        nike-application: var.nike_application
        nike-department: var.nike_department
        nike-domain: var.nike_domain
        nike-costcenter: var.nike_costcenter
        exceptions : "{ \"uptimeschedule\": false,\"deallocated\": true, \"rightsizing\": false, \"blobpublicaccess\" : true, \"keyexpirationdate\" : true}"
  }
  resource_group_tags = { 
        ProjectCode:  var.nike_projectcode
        Expiration: "",
        SuppressOverwrite : false
  } 

}

module "token-api-resourcegroup" {
  source = "../../modules/resourcegroup"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  service_group_name = local.service_group_name
  service_name = local.service_name
  common_tags = local.common_tags
  resource_group_tags = local.resource_group_tags
  location = local.location

}



module "token-api-key-vault" {
  source = "../../modules/key_vault"
  service_group_name = local.service_group_name
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  service_principle_id = var.service_principle_id
  key_vault_ordinal = 1 # Set, don't use default
  tenant_id = var.tenant_id
  common_tags = local.common_tags
}

module "token-api-kubernetes-cluster" {
  source = "../../modules/kubernetes_service"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  dns_prefix = var.dns_prefix
  container_service_ordinal = 1 # Set, don't use default
  service_principle_id = var.service_principle_id
  service_principle_secret = var.service_principle_secret
  container_type = var.container_types.hybrid
  common_tags = local.common_tags
  location = local.location
}

module "token-api-vnet" {
  source = "../../modules/virtual_network"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  service_group_name = local.service_group_name
  vnet_id = 1
  vnet_block = local.vnet_block
  vnet_mask = local.vnet_mask
  resource_group_name = module.token-api-resourcegroup.resource_group_name
}

module "token-api-cosmosdb-account" {
  source = "../../modules/cosmosdb_account"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  service_group_name = local.service_group_name
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  #virtual_network_id = tonumber(module.token-api-vnet.virtual_network_id)
  cosmosdb_ordinal = 1
  common_tags = local.common_tags
}

module "token-api-cosmosdb-account-web" {
  source = "../../modules/cosmosdb_account"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  service_group_name = format("%s%s",local.service_group_name , "web")
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  #virtual_network_id = tonumber(module.token-api-vnet.virtual_network_id)
  cosmosdb_ordinal = 1
  common_tags = local.common_tags
}


module "token-api-container-registry" {
  source = "../../modules/container_registry"
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  kubernetes_cluster_name = module.token-api-kubernetes-cluster.kubernetes_cluster_name
  registry_ordinal = 1 # Set, don't use default
  common_tags = local.common_tags
}

module "azure_blockchain_service" {
  source = "../../modules/azure_blockchain_service"
  location = var.location
 resource = module.token-api-resourcegroup.resource_group_name
 abs_sku = var.abs_sku 
 abs_tier = var.abs_tier 
   service_group_name = local.service_group_name
 consortiumManagementAccountPassword = var.consortiumManagementAccountPassword
abs_password = var.abs_password
  common_tags = local.common_tags 
  azure_region_key = local.azure_region_key
   azure_env_key = local.azure_env_key
 
}



/*
module "token-api-storage-account" {
  source = "../../modules/storage_account"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  storage_name = "files" ## Note, just use generic "files" here
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  storage_account_ordinal = 1 # Set, don't use default
  storage_key = var.storage_types.standard
  storage_type = var.storage_tiers.standard
  common_tags = local.common_tags
}
*/


/* Dont have privs to write to Key Vault...

resource "azurerm_key_vault_secret" "admin" {
  name         = "test"
  value        = "test"
  key_vault_id = module.token-api-key-vault.keyvault_id
  #depends_on = [module.cosmos_admin_account]
}
*/


/*


module "token-api-subnet" {
  source = "../../modules/subnet"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  virtual_network_name = module.token-api-vnet.virtual_network_name
  subnet_block = local.subnet_block
  subnet_mask = local.subnet_mask
}
module "token-api-security-group" {
  source = "../../modules/network_security_group"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  subnet_id = module.token-api-subnet.subnet_id
  security_group_type = var.security_group_types.internal
  common_tags = local.common_tags

}
module "token-api-public-ip-address" {
  source = "../../modules/public_ip_address"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  os_type = var.os_types.linux
  server_type = var.server_types.webserver
  subnet_type = var.subnet_types.dmz
  virtual_machine_ordinal = 1
}
module "token-api-linux-virtual-machine-1" {
  source = "../../modules/linux_virtual_machine"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  public_ip_address_id = module.token-api-public-ip-address.public_ip_address_id
  os_type = var.os_types.linux
  server_type = var.server_types.webserver
  subnet_type = var.subnet_types.dmz
  virtual_machine_ordinal = 1
  public_key_file = "mykey.pub"
  common_tags = local.common_tags
}
module "token-api-cosmosdb-account" {
  source = "../../modules/cosmosdb_account"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  service_group_name = local.service_group_name
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  virtual_network_id = module.token-api-vnet.virtual_network_id
  cosmosdb_ordinal = 1
  common_tags = local.common_tags
}

module "token-api-container-registry" {
  source = "../../modules/container_registry"
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  kubernetes_cluster_name = module.token-api-kubernetes-cluster.kubernetes_cluster_name
  registry_ordinal = 1 # Set, don't use default
  common_tags = local.common_tags
}
module "token-api-storage-account" {
  source = "../../modules/storage_account"
  azure_env_key = local.azure_env_key
  azure_region_key = local.azure_region_key
  storage_name = "files" ## Note, just use generic "files" here
  resource_group_name = module.token-api-resourcegroup.resource_group_name
  storage_account_ordinal = 1 # Set, don't use default
  storage_key = var.storage_types.standard
  storage_type = var.storage_tiers.standard
  common_tags = local.common_tags
}

*/