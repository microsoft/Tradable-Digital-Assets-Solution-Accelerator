locals{ 

    security_group_name = templatefile("${path.module}/templates/nike-security-group-name.tmpl", {
        env = var.azure_env_key,
        geo = var.azure_region_key,
        svc_group = var.service_group_name,
        svc_group_id = var.service_group_id,
        az_region = var.location,
        sg_type = var.security_group_type,
        sg_id = "${format("%02s", 1)}"
    })
    common_tags = var.common_tags
}

# Note this a barebones security group, you will want to modify for your useage

resource "azurerm_network_security_group" "security_group" {
    name                = local.security_group_name
    location            = var.location
    resource_group_name = var.resource_group_name
    tags                = local.common_tags
     security_rule {
        name                       = "SSH"
        priority                   = 1001
        direction                  = "Inbound"
        access                     = "Allow"
        protocol                   = "Tcp"
        source_port_range          = "*"
        destination_port_range     = "22"
        source_address_prefix      = var.ssh-source-address
        destination_address_prefix = "*"
    }
    security_rule {
        name                                  = "default-deny"
        priority                              = 1002
        direction                             = "Inbound"
        access                                = "Deny"
        protocol                              = "Tcp"
        source_port_range                     = "*"
        destination_port_range                = "*"
        source_address_prefix                 = "*"
        destination_address_prefix            = "*"
    }
}

resource "azurerm_subnet_network_security_group_association" "internal-ssh" {
  subnet_id                 = var.subnet_id
  network_security_group_id = azurerm_network_security_group.security_group.id
}
