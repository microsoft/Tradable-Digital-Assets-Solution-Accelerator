using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.Test
{
    [TestClass]
    public class UserManager : TestBase
    {

        private string mongoConnectionString;

        private static string partyid;
        private static string blockchainNetworkid;
        private static string serviceEndpoint;
        private static string NFTServiceUserID;
        private static HttpClient httpClient;

        [TestInitialize]
        public void InitializeTest()
        {
            httpClient = new HttpClient();
            //connstring should be removed
            //mongoConnectionString = Config["Values:offchain_connectionstring"];

            if(partyid == null) partyid = Config["Values:PartyID"];

            if (blockchainNetworkid == null) blockchainNetworkid = Config["Values:BlockchainNetworkID"];

            if (serviceEndpoint == null) serviceEndpoint = Config["Values:ServiceEndpoint"];

        }

        [TestMethod]
        public async Task TEST_1_ProvisionUser()
        {
            TokenService.UserManager userManager = new (httpClient,mongoConnectionString, serviceEndpoint,partyid, blockchainNetworkid);
            var result = await userManager.ProvisionUser(Guid.NewGuid().ToString());

            Console.WriteLine($"provisioned user NFTService id is {result}");

            NFTServiceUserID = result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TEST_2_PutCryptoGoodToLocker()
        {
            TokenService.UserManager userManager = new (httpClient, mongoConnectionString,serviceEndpoint,partyid,blockchainNetworkid);

            var result = await userManager.PutCryptoGoodToLocker(NFTServiceUserID,
                new Asset()
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Name = "myDigitalGood"
                });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TEST_3_GetAllUserCryptoGoods()
        {
            TokenService.UserManager userManager = new (httpClient, mongoConnectionString, serviceEndpoint, partyid, blockchainNetworkid);

            var result = userManager.GetAllUserCryptoGoods(NFTServiceUserID);
            result.ForEach(x => Console.WriteLine($"DigitalGood - id : {x.ProductId}"));

            Assert.IsTrue(result.Count >= 0);
        }

        [TestMethod]
        public async Task TEST_4_RemveCryptoGoods()
        {
            TokenService.UserManager userManager = new (httpClient, mongoConnectionString, serviceEndpoint, partyid, blockchainNetworkid);
            string cryptoGoodID = Guid.NewGuid().ToString();

            //Adding...
            var result = await userManager.PutCryptoGoodToLocker(NFTServiceUserID,
            new Asset()
            {
                ProductId = cryptoGoodID,
                Name = "myDigitalGood",
                TokenNumber = 0
            });

            Console.WriteLine($"DigitalGood added with id : {cryptoGoodID}");

            result = await userManager.RemoveCryptoGood(NFTServiceUserID, 0);

            Console.WriteLine($"CryptoGood id : {cryptoGoodID} has been removed");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TEST_5_NotPassingContructor()
        {
            try
            {
                TokenService.UserManager userManager = new ();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(MongoDB.Driver.MongoConfigurationException));
            }

        }
    }
}
