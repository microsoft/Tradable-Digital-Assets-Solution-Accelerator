
locals{ 
    virtual_machine = templatefile("${path.module}/templates/nike-virtual-machine-name.tmpl", { 
        env = var.azure_env_key,
        geo = var.azure_region_key,
        os = var.os_type,
        srv_type = var.server_type,
        subnet_type = var.subnet_type, 
        id = "${format("%04s", var.virtual_machine_ordinal)}"
    }) 
    interface = templatefile("${path.module}/templates/nike-network-interface-name.tmpl", {
        env = var.azure_env_key,
        geo = var.azure_region_key,
        hostname = local.virtual_machine,
        nic_id = "${format("%02s", 1)}"
    })
    common_tags = var.common_tags
}

resource "azurerm_virtual_machine" "virtual_machine" {
  name                  = local.virtual_machine
  location              = var.location
  resource_group_name   = var.resource_group_name
  network_interface_ids = [azurerm_network_interface.interface.id]
  vm_size               = "Standard_A1_v2"
  tags                  = var.common_tags

  # this is a demo instance, so we can delete all data on termination
  delete_os_disk_on_termination = true
  delete_data_disks_on_termination = true

  storage_image_reference {
    publisher = "Canonical"
    offer     = "UbuntuServer"
    sku       = "18.04-LTS"
    version   = "latest"
  }
  storage_os_disk {
    name              = join("-", [local.virtual_machine,"dataDisk1"])             # n6winf00002-dataDisk1
    caching           = "ReadWrite"
    create_option     = "FromImage"
    managed_disk_type = "Standard_LRS"
  }
  os_profile {
    computer_name  = local.virtual_machine
    admin_username = var.admin_username
  }
  os_profile_linux_config {
    disable_password_authentication = true
    ssh_keys {
      key_data = file(var.public_key_file)
      path     = "/home/demo/.ssh/authorized_keys"
    }
  }
}

resource "azurerm_network_interface" "interface" {
  name                      = local.interface
  location                  = var.location
  resource_group_name       = var.resource_group_name

  ip_configuration {
    name                          = "instance1"
    subnet_id                     = var.subnet_id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = var.public_ip_address_id
  }
}
