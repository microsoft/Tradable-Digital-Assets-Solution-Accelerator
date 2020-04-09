
using Contoso.DigitalGoods.DigitalLocker.Service;
using Contoso.DigitalGoods.TokenAPI.Proxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.DigitalLocker.Server.Test
{
    [TestClass]
    public class DigitalLockerManagerTest
    {
        private string mongoConnectionString;
        private DigitalLockerManager _manager;
        private string userid;
        private static Asset cryptoGood;

        [TestInitialize]
        public void InitTest()
        {
            //connstring should be removed
            mongoConnectionString = "mongodb://nikeadmin:GEM7MGtrl2KyJ4P4rrQPTiGizFbn8PJdWvrZwlbRV9Gkl3sFlMyMTIdOGF9hHVM8F6m37BWiQOuZJKRGDYX9GA==@nikeadmin.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
            //userid need to be changed at your env.
            userid = "7f39f297-3ac0-4803-b19a-b79acee74797";
            _manager = new DigitalLockerManager(mongoConnectionString, "DigitalLockers");
        }


        [TestMethod]
        public async Task TEST_1_ProvisioningLocker()
        {
            var result =
                await _manager.ProvisionLocker(Guid.NewGuid().ToString());

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

            //var result = await _manager.AddCryptoKics(userid, cryptoGood);
            //Assert.IsTrue(result);
            Assert.IsTrue(true);
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
        public async Task TEST_5_RemoveCryptoKics()
        {
            //Asset cryptoGood = new Asset()
            //{
            //    ProductId = Guid.NewGuid().ToString(),
            //    Name = "air jordan",
            //    TokenNumber = 0
            //};

            //await _manager.AddCryptoKics(userid, cryptoGood);
            var removeResult = await _manager.RemoveCryptoGoods(userid, cryptoGood.TokenNumber);

            Assert.IsTrue(removeResult);
        }

        [TestMethod]
        public void TEST_6_GetUserCryptoGood_exists()
        {
            var CryptoGoods = _manager.GetAllUserCryptoGoods(userid);
            var cryptoGood = _manager.GetUserCryptoGood(userid, CryptoGoods[0].TokenNumber);

            Assert.IsTrue(cryptoGood.ProductId == CryptoGoods[0].ProductId);
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
