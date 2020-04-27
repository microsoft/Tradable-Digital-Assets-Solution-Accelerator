locals{ 
    ## Create Group Name
    resource_group_name = templatefile("${path.module}/templates/nike-resourcegroup-name.tmpl", {
      env = var.azure_env_key,
      geo = var.azure_region_key,
      location = var.location
      svc_group = var.service_group_name,
      svc_group_id = format("%02s", var.service_group_id),
      svc_name = var.service_name,
      rg_id = format("%02s", var.resource_group_id)
    })

    common_tags = var.common_tags
    resource_group_tags = var.resource_group_tags
}

resource "azurerm_resource_group" "resource_group" {
  name     = local.resource_group_name
  location = var.location
  tags = merge(local.common_tags,local.resource_group_tags)
}
        