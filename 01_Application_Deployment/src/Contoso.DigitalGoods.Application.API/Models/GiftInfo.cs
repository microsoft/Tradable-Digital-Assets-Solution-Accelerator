using Contoso.DigitalGoods.Application.Models;

using System.Collections.Generic;
using Contoso.DigitalGoods.DigitalGoodsGift.Service.Models;

namespace Contoso.DigitalGoods.Application.API.Models
{
    public class GiftInfo : IMessage<CryptoGift>
    {
        public bool Success { get; set; }

        public CryptoGift Data { get; set; }
        public string Message { get; set; }
    }

    public class GiftInfos : IObjectCollectionMessage<CryptoGift>
    {
        public bool Success { get; set; }
        public ICollection<CryptoGift> Data { get; set; }
        public string Message { get; set; }
    }
}
