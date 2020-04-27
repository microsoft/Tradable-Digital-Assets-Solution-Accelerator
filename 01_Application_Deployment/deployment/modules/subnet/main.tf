locals{ 

  subnet_name = templatefile("${path.module}/templates/nike-subnet-name.tmpl", {
      env = var.azure_env_key,
      geo = var.azure_region_key,
      ip_block = var.subnet_block,
      mask_bits = var.subnet_mask
  })


}

resource "azurerm_subnet" "subnet" {
  name                 = local.subnet_name
  resource_group_name  = var.resource_group_name
  virtual_network_name = var.virtual_network_name
  address_prefix       = join("/",[var.subnet_block,var.subnet_mask]) #"${var.subnet_1_block}/${var.subnet_1_mask}"   
}
