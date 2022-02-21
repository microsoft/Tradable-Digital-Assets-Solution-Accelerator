using Contoso.DigitalGoods.Test;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;


namespace Contoso.DigitalGoods.TokenService.Tests
{
    [TestClass()]
    public class TokenManagerTests : TestBase
    {
        private string mongoConnectionString;
        private TokenManager tokenManager;
        private string _ContosoProductManager;
        private static string _tokenID;
        private string _Contoso;
        private string _userA;
        private static long _tokenNumber;
        private static HttpClient _httpClient;
        [TestInitialize]
        public void Init()
        {
           _httpClient = new HttpClient();

            //connstring should be removed
            //mongoConnectionString = Config["Values:offchain_connectionstring"];

            tokenManager = new TokenManager(_httpClient, mongoConnectionString, Config["Values:ServiceEndpoint"]);
            
            _ContosoProductManager = Config["Values:ContosoProductManager"];
            _Contoso = Config["Values:ContosoID"];
            _userA = Config["Values:UserA_NFTServiceID"];
        }


        [TestMethod()]
        public async Task TEST_1_CreateTokenTest()
        {
            string dynamicTokenName = $"ContosoCryptoGoodToken";
            var retResult =
                await tokenManager.CreateToken(dynamicTokenName, "CK", _ContosoProductManager);


            Console.WriteLine($"Created token ID is {retResult.TokenID}");
            _tokenID = retResult.TokenID;
        }

        [TestMethod()]
        public async Task TEST_2_TokenMintTest()
        {
            //_tokenNumber = System.DateTime.Now.Ticks;

            var mintedResult = await tokenManager.MintToken(_tokenID, _ContosoProductManager, _Contoso,
                new Models.DigitalGoodToken()
                {
                    ImageURL = "http://",
                    Model3DURL = "http://",
                    ProductId = "pid000",
                    ProductName = "product name",
                });

            var response = await tokenManager.GetTokenMetaData(_tokenID, _Contoso, Convert.ToInt64(mintedResult.TokenNumber, 16));

            _tokenNumber = Convert.ToInt64(mintedResult.TokenNumber, 16);

            Assert.IsTrue(JsonConvert.SerializeObject(mintedResult) == response);
        }

        [TestMethod()]
        public async Task TEST_3_TransferCryptoGoodsTest()
        {
            //Assert.Fail();

            var transferResult = await tokenManager.TransferCryptoGoods(_tokenID, _tokenNumber, _Contoso, _Contoso, _userA);


            var response = await tokenManager.GetTokenMetaData(_tokenID, _userA, _tokenNumber);
            Console.WriteLine(response);

            Assert.IsInstanceOfType(response, Type.GetType("System.String"));
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