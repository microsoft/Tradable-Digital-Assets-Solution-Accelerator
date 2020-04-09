using Contoso.DigitalGoods.Application.Models;
using Contoso.DigitalGoods.ProductCatalog.Service.Models;
using System.Collections.Generic;

namespace Contoso.DigitalGoods.Application.API.Models
{
    public class ProductCatalogInfo : IMessage<ProductInfo>
    {
        public bool Success { get; set; }
        public ProductInfo Data { get; set; }
        public string Message { get; set; }
    }

    public class ProductCatalogInfos : IObjectCollectionMessage<ProductInfo>
    {
        public bool Success { get; set; }
        public ICollection<ProductInfo> Data { get; set; }
        public string Message { get; set; }
    }


}
