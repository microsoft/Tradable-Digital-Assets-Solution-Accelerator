using Contoso.DigitalGoods.DigitalGoodsGift.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.DigitalGoodsGift.Service.Interfaces
{
    public interface IGiftManager
    {
        Task<CryptoGift> CreateCryptoGoodGift(string ReciverABTUserID, long TokenId);

        CryptoGift GetCryptoGoodGift(string GiftId);

        Task<CryptoGift> TransferCryptoGoodGiftToken(string GiftId);

        IEnumerable<CryptoGift> GetAllActiveCryptoGoodGifts();
    }
}
