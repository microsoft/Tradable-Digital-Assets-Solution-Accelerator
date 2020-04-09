using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.Tests
{
    [TestClass()]
    public class TokenManagerTests
    {
        private string mongoConnectionString;
        private TokenManager tokenManager;
        private string _ContosoProductManager;
        private static string _tokenID;
        private string _Contoso;
        private string _userA;
        private static long _tokenNumber;

        [TestInitialize]
        public void Init()
        {
            //connstring should be removed
            mongoConnectionString = "mongodb://Contosoadmin:GEM7MGtrl2KyJ4P4rrQPTiGizFbn8PJdWvrZwlbRV9Gkl3sFlMyMTIdOGF9hHVM8F6m37BWiQOuZJKRGDYX9GA==@Contosoadmin.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

            tokenManager = new TokenManager(mongoConnectionString, "nike");
            //_tokenID = "e8c3f0ca-fd71-4368-ae20-fb2a8ced6720";
            _ContosoProductManager = "nikeproductmanager";
            _Contoso = "04cbac3c-e950-466e-9935-416738563d97";
            _userA = "a58a7bf0-2753-4e18-8f3e-281cddb9cec7";
        }


        [TestMethod()]
        public async Task TEST_1_CreateTokenTest()
        {
            string dynamicTokenName = $"ContosoCryptoGoodToken";
            var retResult =
                await tokenManager.CreateToken(dynamicTokenName, "CK", _ContosoProductManager);

            Libs.WaitProcess.HoldsOnSeconds(10);

            Console.WriteLine($"Created token ID is {retResult.TokenID}");
            _tokenID = retResult.TokenID;
        }

        [TestMethod()]
        public async Task TEST_2_TokenMintTest()
        {
            //_tokenNumber = System.DateTime.Now.Ticks;

            var mintedResult = await tokenManager.MintToken(_tokenID, _ContosoProductManager, _Contoso,
                new Models.CryptoKickToken()
                {
                    ImageURL = "http://",
                    Model3DURL = "http://",
                    ProductId = "pid000",
                    ProductName = "product name",
                });

            var response = await tokenManager.GetTokenMetaData(_tokenID, _Contoso, Convert.ToInt64(mintedResult.TokenNumber, 16));

            _tokenNumber = Convert.ToInt64(mintedResult.TokenNumber, 16);

            Assert.IsTrue(JsonConvert.SerializeObject(mintedResult) == response.Output);
        }

        [TestMethod()]
        public async Task TEST_3_TransferCryptoGoodsTest()
        {
            //Assert.Fail();

            var transferResult = await tokenManager.TransferCryptoGoods(_tokenID, _tokenNumber, _Contoso, _Contoso, _userA);

            Libs.WaitProcess.HoldsOnSeconds(8);

            var response = await tokenManager.GetTokenMetaData(_tokenID, _userA, _tokenNumber);
            Console.WriteLine(response.Output);

            Assert.IsInstanceOfType(response.Output, Type.GetType("System.String"));
        }


        [TestMethod()]
        public async Task TEST_4_GetCryptoGoodInfoFromTokenTest()
        {
            var response = await tokenManager.GetCryptoGoodInfoFromToken(_tokenID, _userA, _tokenNumber);
            Console.WriteLine(response);

            Assert.IsInstanceOfType(response, Type.GetType("System.String"));
        }
    }
}