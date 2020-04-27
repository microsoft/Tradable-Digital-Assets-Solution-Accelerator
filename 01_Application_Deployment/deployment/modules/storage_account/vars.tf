# Declare module variables, to be overwritten by call from module

## Common Variables ##
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

## Common Tag Variables
variable "common_tags" { 
    type = map(string)
    default = { 
  } 
}

## Storage Account

variable "resource_group_name" {
  type    = string
  default = ""
}

variable "service_group_name" {
  type    = string
  default = ""
}

variable "storage_account_ordinal" {
  type    = number
  default = 1
}

variable "storage_key" {
  type    = string
  default = ""
}

variable "storage_type" {
  type    = string
  default = ""
}

variable "storage_name" {
  type    = string
  default = ""
}


