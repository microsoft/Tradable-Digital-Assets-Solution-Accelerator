locals { 
    container_registry_name = templatefile("${path.module}/templates/nike-container-registry-name.tmpl", {
      kubernetes_cluster_name = var.kubernetes_cluster_name
      id = "${format("%02s", var.registry_ordinal)}"
     })
    common_tags = var.common_tags
}

resource "azurerm_container_registry" "container_registry" {
  name                     = local.container_registry_name
  resource_group_name      = var.resource_group_name
  location                 = var.location
  sku                      = "Standard"
  admin_enabled            = false
  tags                     = local.common_tags
}
