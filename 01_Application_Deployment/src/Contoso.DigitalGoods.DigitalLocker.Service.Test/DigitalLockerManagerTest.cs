using Contoso.DigitalGoods.DigitalLocker.Service;
using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.DigitalLocker.Server.Test
{
    [TestClass]
    public class DigitalLockerManagerTest : TestBase
    {
        private string mongoConnectionString;
        private static DigitalLockerManager _manager;
        private static string userid;
        private static Asset cryptoGood;

        [TestInitialize]
        public void InitTest()
        {
            //connstring should be removed
            mongoConnectionString = Config["Values:offchain_appconnectionstring"];
            //userid need to be changed at your env.
            if (userid == null) 
                userid = Guid.NewGuid().ToString();
            
            if (_manager == null)            
                _manager = new DigitalLockerManager(mongoConnectionString, "DigitalLockers");
        }

        [TestMethod]
        public async Task TEST_1_ProvisioningLocker()
        {
            var result =
                await _manager.ProvisionLocker(userid);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task TEST_2_AddCryptoGoods()
        {
            cryptoGood = new Asset()
            {
                ProductId = Guid.NewGuid().ToString(),
                Name = "air jordan",
                TokenNumber = 0
            };

            var result = await _manager.AddCryptoKics(userid, cryptoGood);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TEST_3_GetAllCryptoGoods_noAssets()
        {
            var CryptoGoods = _manager.GetAllUserCryptoGoods("");

            Assert.IsNull(CryptoGoods);
        }

        [TestMethod]
        public void TEST_4_GetAllCryptoGoods_hasAssets()
        {
            var CryptoGoods = _manager.GetAllUserCryptoGoods(userid);

            Assert.IsNotNull(CryptoGoods);
            CryptoGoods.ForEach(x => Console.WriteLine(x.Name));
        }


        [TestMethod]
        public void TEST_5_GetUserCryptoGood_exists()
        {
            var CryptoGoods = _manager.GetAllUserCryptoGoods(userid);
            var cryptoGood = _manager.GetUserCryptoGood(userid, CryptoGoods[0].TokenNumber);

            Assert.IsTrue(cryptoGood.ProductId == CryptoGoods[0].ProductId);
        }

        [TestMethod]
        public async Task TEST_6_RemoveCryptoKics()
        {
            var removeResult = await _manager.RemoveCryptoGoods(userid, cryptoGood.TokenNumber);

            Assert.IsTrue(removeResult);
        }

        [TestMethod]
        public void TEST_7_GetUserCryptoGood_not_exists()
        {
            var CryptoGoods = _manager.GetAllUserCryptoGoods(userid);
            var cryptoGood = _manager.GetUserCryptoGood(userid, -100);

            Assert.IsNull(cryptoGood);

            
        }
    }
}
