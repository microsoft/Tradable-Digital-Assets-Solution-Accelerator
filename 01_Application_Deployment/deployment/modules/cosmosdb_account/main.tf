locals { 
    cosmos_db_name = templatefile("${path.module}/templates/nike-cosmos-db-name.tmpl", {
      env = var.azure_env_key,
      geo = var.azure_region_key,
      svc_group = var.service_group_name,
      id = "${format("%02s", var.cosmosdb_ordinal)}"
     }) 
     common_tags = var.common_tags
}

resource "azurerm_cosmosdb_account" "db" {
  name                = local.cosmos_db_name
  location            = var.location
  resource_group_name = var.resource_group_name
  offer_type          = "Standard"
  kind                = "MongoDB"
  tags                = local.common_tags

  enable_automatic_failover = true

  is_virtual_network_filter_enabled = true

  #virtual_network_rule {
  #  id = var.virtual_network_id
  #}

  capabilities {
    name = "MongoDBv3.4"
  }

  consistency_policy {
    consistency_level       = "BoundedStaleness"
    max_interval_in_seconds = 300
    max_staleness_prefix    = 100000
  }

  geo_location {
    location          = var.failover_location
    failover_priority = 1
  }

  geo_location {
    prefix            = join("-",[local.cosmos_db_name,"main"])
    location          = var.location
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_mongo_database" "mongo-database" {
  name                =  join("-",[local.cosmos_db_name,"mongo-database"])
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.db.name
}

resource "azurerm_cosmosdb_mongo_collection" "mongo-database-collection" {
  name                = join("-",[local.cosmos_db_name,"mongo-collection"])
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.db.name
  database_name       = azurerm_cosmosdb_mongo_database.mongo-database.name

  default_ttl_seconds = "777"
  shard_key           = "uniqueKey"
}