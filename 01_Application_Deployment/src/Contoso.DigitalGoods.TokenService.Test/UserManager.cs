
using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.Test
{
    [TestClass]
    public class UserManager
    {

        private string mongoConnectionString;

        [TestInitialize]
        public void InitializeTest()
        {
            //connstring should be removed
            mongoConnectionString = "mongodb://Contosoadmin:GEM7MGtrl2KyJ4P4rrQPTiGizFbn8PJdWvrZwlbRV9Gkl3sFlMyMTIdOGF9hHVM8F6m37BWiQOuZJKRGDYX9GA==@Contosoadmin.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
        }

        //[TestMethod]
        //public async Task TEST_1_ProvisionUser()
        //{
        //    TokenService.UserManager userManager = new TokenService.UserManager(mongoConnectionString);
        //    TokenAPIService tokenAPIConnection = new TokenAPIService();

        //    var result = await userManager.ProvisionUser(Guid.NewGuid().ToString());

        //    Console.WriteLine($"provisioned user ABT id is {result}");

        //    Assert.IsNotNull(result);
        //}

        [TestMethod]
        public async Task TEST_2_PutCryptoGoodToLocker()
        {
            TokenService.UserManager userManager = new TokenService.UserManager(mongoConnectionString);

            var result = await userManager.PutCryptoGoodToLocker("bac0558f-f706-4465-acda-8eb701913032",
                new Asset()
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Name = "Air Jordan"
                });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TEST_3_GetAllUserCryptoGoods()
        {
            TokenService.UserManager userManager = new TokenService.UserManager(mongoConnectionString);

            var result = userManager.GetAllUserCryptoGoods("bac0558f-f706-4465-acda-8eb701913032");
            result.ForEach(x => Console.WriteLine($"CryptoKics - id : {x.ProductId}"));

            Assert.IsTrue(result.Count >= 0);
        }

        [TestMethod]
        public async Task TEST_4_RemveCryptoGoods()
        {
            TokenService.UserManager userManager = new TokenService.UserManager(mongoConnectionString);
            string cryptoGoodID = Guid.NewGuid().ToString();

            //Adding...
            var result = await userManager.PutCryptoGoodToLocker("bac0558f-f706-4465-acda-8eb701913032",
            new Asset()
            {
                ProductId = cryptoGoodID,
                Name = "Air Jordan",
                TokenNumber = 0
            });

            Console.WriteLine($"CryptoGood added with id : {cryptoGoodID}");

            result = await userManager.RemoveCryptoGood("bac0558f-f706-4465-acda-8eb701913032", 0);

            Console.WriteLine($"CryptoGood id : {cryptoGoodID} has been removed");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TEST_5_NotPassingContructor()
        {
            try
            {
                TokenService.UserManager userManager = new TokenService.UserManager();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(MongoDB.Driver.MongoConfigurationException));
            }

        }
    }
}
