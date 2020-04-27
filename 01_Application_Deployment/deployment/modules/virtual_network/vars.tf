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

variable "service_group_id" {
  type    = number
  default = 1
}

variable "common_tags" { 
    type = map(string)
    default = { 
  } 
}

variable "resource_group_name" {
  type    = string
  default = ""
}

variable "vnet_id" {
  type= number
  default = 1
}

variable "vnet_block" {
  type= string
  default = ""
}

variable "vnet_mask" {
  type= number
  default = 18
}