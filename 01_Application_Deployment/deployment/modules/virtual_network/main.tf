locals{ 
    virtual_network_name = templatefile("${path.module}/templates/nike-virtual-network-name.tmpl", {
      env = var.azure_env_key,
      geo = var.azure_region_key,
      svc_group = var.service_group_name,
      svc_group_id = "${format("%02s", var.service_group_id)}",
      az_region = var.location
      vnet_id = "${format("%02s", var.vnet_id)}"
     })
     common_tags = var.common_tags
}

resource "azurerm_virtual_network" "virtual_network" {
  name                = local.virtual_network_name
  location            = var.location
  resource_group_name = var.resource_group_name
  tags                = var.common_tags
  address_space       = [join("/",[ var.vnet_block,var.vnet_mask])]
}


