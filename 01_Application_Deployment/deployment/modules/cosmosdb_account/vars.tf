# Declare module variables, to be overwritten by call from module

## Common Variables ##

variable "location" {
  type = string
  default = "eastus"
}

variable "azure_env_key" {
  type = string
  default = ""
}

variable "azure_region_key" {
  type    = string
  default = ""
}

variable "service_group_name" {
  type    = string
  default = ""
}
 
variable "common_tags" {
  type = map(string)
  default = {}
}

## For Cosmodb
variable "resource_group_name" {
  type    = string
  default = ""
}

variable "failover_location" {
  type = string
  default = "westus"
}

variable "virtual_network_id" {
  type= number
  default = 1
}

variable "cosmosdb_ordinal" {
  type= number
  default = 1
}


