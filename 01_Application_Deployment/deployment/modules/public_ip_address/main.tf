
locals{ 
    virtual_machine = templatefile("${path.module}/templates/nike-virtual-machine-name.tmpl", { 
        env = var.azure_env_key,
        geo = var.azure_region_key,
        os = var.os_type,
        srv_type = var.server_type,
        subnet_type = var.subnet_type, 
        id = "${format("%04s", var.virtual_machine_ordinal)}"
    }) 
    public_ip_address_name = templatefile("${path.module}/templates/nike-public-ip-name.tmpl", {
        env = var.azure_env_key,
        geo = var.azure_region_key,
        hostname = local.virtual_machine
    })

}

resource "azurerm_public_ip" "public_ip_address" {
    name                         = local.public_ip_address_name
    location                     = var.location
    resource_group_name          = var.resource_group_name
    allocation_method            = "Dynamic" # Can be static if we want to ensure it's retained and reassigned, may be able to assign a public IP from list created.
    sku                          = "Basic"  # Could be Standard if Availablity Zone features are useful
}