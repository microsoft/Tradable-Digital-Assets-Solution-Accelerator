using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;
using System.Collections.Generic;

namespace Contoso.DigitalGoods.ProductCatalog.Service.Models
{
    public class ProductInfo : IEntityModel<Guid>
    {
        public ProductInfo()
        {
            this.Id = Guid.NewGuid();
            this.Assets = new List<AssetInfo>();
        }

        public Guid Id { get; set; }
        public string ProductID { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public List<AssetInfo> Assets { get; set; }
    }
}
