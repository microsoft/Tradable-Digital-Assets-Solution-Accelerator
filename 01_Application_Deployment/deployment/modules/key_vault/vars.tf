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

## Common Tag Variables


variable "common_tags" { 
    type = map(string)
    default = { 
  } 
}

## For Key Vault 
variable "resource_group_name" {
  type    = string
  default = ""
}

variable "key_vault_ordinal" {
  type= number
  default = 1
}

variable "service_principle_id" {
  type = string
  default = ""
}

variable "tenant_id" {
  type = string
  default = ""
}


