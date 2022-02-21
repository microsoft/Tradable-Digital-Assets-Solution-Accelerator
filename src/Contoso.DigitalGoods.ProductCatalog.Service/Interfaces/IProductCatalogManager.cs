using Contoso.DigitalGoods.ProductCatalog.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.ProductCatalog.Service
{
    public interface IProductCatalogManager
    {
        ProductInfo GetProductInfo(string ProductID);
        Task<ProductInfo> RegisterProductCatalog(ProductInfo ProductInfo);
        bool UnRegisterProductCatalog(string productID);
        IEnumerable<ProductInfo> GetAllProducts();

    }
}