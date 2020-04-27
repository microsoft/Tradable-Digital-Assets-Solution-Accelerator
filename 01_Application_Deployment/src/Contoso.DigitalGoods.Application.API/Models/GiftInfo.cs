using Contoso.DigitalGoods.Application.Models;
using Contoso.DigitalGoods.CryptoGoodsGift.Service.Models;
using System.Collections.Generic;

namespace Contoso.DigitalGoods.Application.API.Models
{
    public class GiftInfo : IMessage<DigitalGift>
    {
        public bool Success { get; set; }

        public DigitalGift Data { get; set; }
        public string Message { get; set; }
    }

    public class GiftInfos : IObjectCollectionMessage<DigitalGift>
    {
        public bool Success { get; set; }
        public ICollection<DigitalGift> Data { get; set; }
        public string Message { get; set; }
    }
}
