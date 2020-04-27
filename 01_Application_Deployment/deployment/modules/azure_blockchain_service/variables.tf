


variable "location" {
  description = "The region where the virtual network is created."
}



variable "azure_env_key" {
  type = string
  default = ""
}


variable "resource"{ 
}


variable "service_group_name" {
  type    = string
  default = ""
}
variable "azure_region_key" {
  type    = string
  default = ""
}


variable "abs_sku"{
 type    = string
  default = ""
}

variable "abs_tier"{
 type    = string
  default = ""
}
variable consortiumManagementAccountPassword{
   type    = string
  default = ""
}

variable abs_password{
   type    = string
  default = ""
}


variable "common_tags" {
  type = map(string)
  default = {}
}