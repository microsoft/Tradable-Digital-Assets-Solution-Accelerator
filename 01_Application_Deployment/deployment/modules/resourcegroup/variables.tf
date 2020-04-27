# Declare module variables, to be overwritten by call from module

## Common Variables ##

variable "location"{
  type = string
  default = ""
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

variable "service_name" {
  type    = string
  default = ""
}

variable "resource_group_id" {
  type    = number
  default = 1
}

variable "common_tags" { 
    type = map(string)
    default = { 
  } 
}

variable "resource_group_tags" { 
    type = map(string)
    default = { 
  } 
}

