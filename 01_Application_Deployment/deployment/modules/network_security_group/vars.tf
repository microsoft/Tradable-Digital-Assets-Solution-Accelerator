## Common Values
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

## Common tags vars 
variable "common_tags" { 
    type = map(string)
    default = { 
  } 
}

## for security group 
variable "resource_group_name" {
  type    = string
  default = ""
}

variable "subnet_id" { 
    type = string
    default = ""
}

variable "ssh-source-address" {
  type    = string
  default = "10.0.0.0/8"
}

variable security_group_type {
    type = string
    default = ""
}