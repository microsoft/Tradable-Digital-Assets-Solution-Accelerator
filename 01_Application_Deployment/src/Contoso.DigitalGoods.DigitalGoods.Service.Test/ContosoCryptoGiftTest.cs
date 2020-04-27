using Contoso.DigitalGoods.CryptoGoodsGift.Service;
using Contoso.DigitalGoods.CryptoGoodsGift.Service.Models;
using Contoso.DigitalGoods.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.ContosoProfile.Service.Test
{
    [TestClass]
    public class ContosoCryptoGiftTest : TestBase
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
            mongoConnectionString = Config["Values:offchain_appconnectionstring"];
            tokenAPIURL = Config["Values:ServiceEndpoint"];

            //userid need to be changed at your env.
            senderContosoid = Config["Values:UserA_ContosoID"];
            senderAbtUserID = Config["Values:UserA_ABTID"];
            reciverContosoid = Config["Values:UserB_ContosoID"];
            reciverAbtUserID = Config["Values:UserB_ABTID"];
            giftId = 1;
            _manager = new GiftManager(mongoConnectionString, "CryptoGifts", senderContosoid, tokenAPIURL, Config["Values:PartyID"], Config["Values:BlockchainNetworkID"]);
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
