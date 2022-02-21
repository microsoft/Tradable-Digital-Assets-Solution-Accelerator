using System.Text.Json.Serialization;

namespace Contoso.DigitalGoods.ProductCatalog.Service.Models
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum AssetType
    {
        Image,
        Video,
        Model
    }
}
