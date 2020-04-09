using Contoso.DigitalGoods.ProductCatalog.Service.Models;
using Contoso.DigitalGoods.TokenAPI.Proxy;

namespace Contoso.DigitalGoods.DigitalLocker.App.Models
{
    public class DigitalLockerItem : Asset
    {
        public ProductInfo ProductDetail { get; set; }
    }
}
