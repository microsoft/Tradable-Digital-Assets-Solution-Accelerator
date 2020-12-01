

using Contoso.DigitalGoods.DigitalGoodsGift.Service;
using Contoso.DigitalGoods.DigitalGoodsGift.Service.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.ContosoProfile.Service.Test
{
    [TestClass]
    public class ContosoDigitalGiftTest
    {
        private string mongoConnectionString;
        private string tokenAPIURL;
        private GiftManager _manager;
        private string senderContosoid;
        private string senderAbtUserID;
        private string reciverContosoid;
        private string reciverAbtUserID;
        private long giftId;

        [TestInitialize]
        public void InitTest()
        {
            //connstring should be removed
            mongoConnectionString = "";
            tokenAPIURL = "http://13.66.95.45";

            //userid need to be changed at your env.
            senderContosoid = "39972f98-583e-47fa-adaa-1dea60c82b27";
            senderAbtUserID = "67df940d-539d-4ff4-a4cf-60e992e129d0";
            reciverContosoid = "7fded3b8-68d3-44c4-a1d0-18d09a605496";
            reciverAbtUserID = "5e5b2faf-f812-4437-86a5-a1731fe4979d";
            giftId = 1;
            _manager = new GiftManager(mongoConnectionString, "CryptoGifts", senderContosoid, tokenAPIURL);
        }

        [TestMethod]
        public async Task CreateContosoCryptoGift()
        {
            var result = await _manager.CreateCryptoGoodGift(reciverAbtUserID, giftId);
            Assert.IsInstanceOfType(result, typeof(CryptoGift));
            _manager.DeleteCryptoGift(result);
        }

        [TestMethod]
        public async Task GetCryptoGift()
        {
            var gift = await _manager.CreateCryptoGoodGift(reciverAbtUserID, giftId);
            var result = _manager.GetCryptoGoodGift(gift.GiftId);
            Assert.IsTrue(result.TokenId == giftId);
            _manager.DeleteCryptoGift(result);
        }

        [TestMethod]
        public void GetCryptAllGift()
        {
            var gift = _manager.GetAllActiveCryptoGoodGifts();
            Assert.IsInstanceOfType(gift, typeof(IEnumerable<CryptoGift>));
        }

    }
}
