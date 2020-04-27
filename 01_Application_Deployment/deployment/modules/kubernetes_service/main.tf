locals { 
    kubernetes_cluster_name = templatefile("${path.module}/templates/nike-container-service-name.tmpl", {
      env = var.azure_env_key,
      geo = var.azure_region_key,
      container_type = var.container_type,
      
      id = "${format("%05s", var.container_service_ordinal)}"
     }) 
    common_tags = var.common_tags
}

resource "azurerm_kubernetes_cluster" "kubernetes_cluster" {
  name                = local.kubernetes_cluster_name
  location            = var.location
  resource_group_name = var.resource_group_name
  dns_prefix          = var.dns_prefix
  tags                = local.common_tags

  default_node_pool {
    name       = "default"
    node_count = var.container_node_count
    vm_size    = "Standard_D2_v2"
  }

  service_principal {
    client_id     = var.service_principle_id
    client_secret = var.service_principle_secret
  }
}

output "client_certificate" {
  value = azurerm_kubernetes_cluster.kubernetes_cluster.kube_config.0.client_certificate
}

output "kube_config" {
  value = azurerm_kubernetes_cluster.kubernetes_cluster.kube_config_raw
}
