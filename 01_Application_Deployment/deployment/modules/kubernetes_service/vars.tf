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

## For Azure Container Service
variable "resource_group_name" {
  type    = string
  default = ""
}

variable "dns_prefix" {
  type = string
  default = ""
}

variable "container_node_count" {
  type= number
  default = 1
}

variable "container_service_ordinal" {
  type= number
  default = 1
}

variable "service_principle_id" {
  type = string
  default = ""
}

variable "service_principle_secret" {
  type = string
  default = ""
}

variable "container_type" {
  type = string
  default = ""
}


