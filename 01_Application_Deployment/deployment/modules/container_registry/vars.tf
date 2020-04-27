# Declare module variables, to be overwritten by call from module

## Common Variables ##

variable "location" {
  type = string
  default = "eastus"
}

variable "common_tags" { 
    type = map(string)
    default = { 
  } 
}

## For Container Registry
variable "resource_group_name" {
  type    = string
  default = ""
}

variable "kubernetes_cluster_name" {
  type = string
  default = ""
}

variable "registry_ordinal" {
  type= number
  default = 1
}


