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

variable "common_tags" { 
    type = map(string)
    default = { 
  } 
}

## Virtual Machine Specific

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

variable "admin_username" {
  type    = string
  default = ""
}

variable "public_key_file" {
  type    = string
  default = ""
}

variable "subnet_id" {
  type    = string
  default = ""
}

variable "public_ip_address_id" {
  type    = number
  default = 1
}

variable "virtual_machine_ordinal" {
  type    = string
  default = ""
}




