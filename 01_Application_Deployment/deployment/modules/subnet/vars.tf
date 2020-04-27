## module variables

variable "resource_group_name" {
  type    = string
  default = ""
}

variable "virtual_network_name" {
  type    = string
  default = ""
}

variable "subnet_block" {
  type    = string
  default = ""
}

variable "subnet_mask" {
  type    = number
  default = 1
}

## Common Variables ##

variable "location"{
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

