locals { 
    block_chain_name = templatefile("${path.module}/templates/nike-abs-name.tmpl", {
      env = var.azure_env_key,
      geo = var.azure_region_key,
      svc_group = var.service_group_name,
      id = "abs"
     }) 
     common_tags = var.common_tags
}



resource "azurerm_template_deployment" "azure_blockchain_service" {
  name                = local.block_chain_name
  resource_group_name = var.resource
  template_body       = file("${path.module}/templates/template.json")
 # tags                = local.common_tags
  parameters ={
   
    name       = local.block_chain_name
    location   = var.location 
  
     
    sku        = var.abs_sku
    abs_tier = var.abs_tier
consortiumManagementAccountPassword = var.consortiumManagementAccountPassword
abs_password = var.abs_password

  }

  deployment_mode = "Incremental"
}