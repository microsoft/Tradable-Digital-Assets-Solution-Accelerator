##############################################################################
# Outputs File
#
# Expose the outputs you want your users to see after a successful 
# `terraform apply` or `terraform output` command. You can add your own text 
# and include any data from the state file. Outputs are sorted alphabetically;
# use an underscore _ to move things to the bottom. 

output "virtual_network_name" {
  value = azurerm_virtual_network.virtual_network.name
}
output "virtual_network_id" {
  value = azurerm_virtual_network.virtual_network.id
}
