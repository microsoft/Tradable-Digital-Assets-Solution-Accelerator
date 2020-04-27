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


## Public IP Address Specific

variable "resource_group_name" {
  type    = string
  default = ""
}

variable "os_type" {
  type    = string
  default = ""
}

variable "server_type" {
  type    = string
  default = ""
}

variable "subnet_type" {
  type    = string
  default = ""
}

variable "virtual_machine_ordinal" {
  type    = string
  default = ""
}

