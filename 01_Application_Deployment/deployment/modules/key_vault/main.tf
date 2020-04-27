locals { 
    key_vault_name = templatefile("${path.module}/templates/nike-key-vault-name.tmpl", {
      env = var.azure_env_key,
      geo = var.azure_region_key,
      svc_group = var.service_group_name,
      id = "${format("%02s", var.key_vault_ordinal)}"
    }) 
    common_tags = var.common_tags
}

resource "azurerm_key_vault" "key_vault" {
  name                        = local.key_vault_name
  location                    = var.location
  resource_group_name         = var.resource_group_name
  enabled_for_disk_encryption = true
  tenant_id                   = var.tenant_id
  soft_delete_enabled         = true
  purge_protection_enabled    = false
  tags                        = local.common_tags
  sku_name                    = "standard"

  access_policy {
    tenant_id = var.tenant_id
    object_id = var.service_principle_id
    key_permissions = ["create","get",]
    secret_permissions = ["set", "get", "delete",]
  }
}