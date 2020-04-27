locals{ 
    ## Create Group Name
    storage_account_name = templatefile("${path.module}/templates/nike-storage-account-name.tmpl", {
      env = var.azure_env_key,
      geo = var.azure_region_key,
      svc_group = var.service_group_name,
      storage_key = var.storage_key
      id = "${format("%02s", var.storage_account_ordinal)}"
    })

    common_tags = var.common_tags
    
}


resource "azurerm_storage_account" "storage_account" {
  name                     = local.storage_account_name
  location                 = var.location
  resource_group_name      = var.resource_group_name
  account_tier             = var.storage_type
  account_replication_type = "LRS"
  tags                     = local.common_tags
}

