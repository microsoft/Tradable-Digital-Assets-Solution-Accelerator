using Contoso.DigitalGoods.OffChain;
using Contoso.DigitalGoods.ProductCatalog.Service.Models;
using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Solutions.CosmosDB.Security.ManagedIdentity;

namespace Contoso.DigitalGoods.ProductCatalog.Service
{
    public class ProductCatalogManager : MongoEntntyCollectionBase<Models.ProductInfo, Guid>, IProductCatalogManager
    {
        public ProductCatalogManager(string DatabaseConnectionString, string CollectionName) : base(DatabaseConnectionString, CollectionName)
        {
        }

        public ProductCatalogManager(
            IConfiguration Configuration,
            CosmosConnectionStrings connectionStrings) : base(connectionStrings.PrimaryReadWriteKey, Configuration["Values:productCatalogCollectionName"])
        {
        }


        public async Task<ProductInfo> RegisterProductCatalog(ProductInfo ProductInfo)
        {
            return await this.ObjectCollection.SaveAsync(ProductInfo);
        }

        public bool UnRegisterProductCatalog(string productID)
        {
            var product = GetProductInfo(productID);
            try
            {
                ObjectCollection.Delete(product);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public ProductInfo GetProductInfo(string ProductID)
        {
            return this.ObjectCollection.Find(new GenericSpecification<ProductInfo>(x => x.ProductID == ProductID));
        }

        public IEnumerable<ProductInfo> GetAllProducts()
        {
            return this.ObjectCollection.GetAll();
        }

    }
}
