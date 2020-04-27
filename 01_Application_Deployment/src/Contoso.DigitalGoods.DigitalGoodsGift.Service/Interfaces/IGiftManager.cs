using Contoso.DigitalGoods.CryptoGoodsGift.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.CryptoGoodsGift.Service.Interfaces
{
    public interface IGiftManager
    {
        Task<DigitalGift> CreateDigitalGoodGift(string ReciverABTUserID, long TokenId);

        DigitalGift GetDigitalGoodGift(string GiftId);

        Task<DigitalGift> TransferDigitalGoodGiftToken(string GiftId);

        IEnumerable<DigitalGift> GetAllActiveDigitalGoodGifts();
    }
}
