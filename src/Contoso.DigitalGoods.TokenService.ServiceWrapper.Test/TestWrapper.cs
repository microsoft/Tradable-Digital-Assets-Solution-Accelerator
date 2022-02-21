using Contoso.DigitalGoods.Test;
using Microsoft.Solutions.NFT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.ServiceWrapper.Test
{
    [TestClass]
    public class TestWrapper : TestBase
    {

        //const string Contoso = 
        //const string UserA = 
        //const string UserB = Config["Values:UserB_NFTServiceID"];
        //const string tokenid = "0x154aaa8b24c68d85b83b70c301f3d6049997f613";
        //const string ContosoProductManager = "4e8c4a26-e6ed-4141-b3ed-b55511282e78";

        private static TokenServiceWrapper _svcWrapper;
        private static AccountServiceWrapper _accountSvcWrapper;
        private string _tokenID;
        private string _ContosoProductManager;
        private string _Contoso;
        private string _userA;
        private static string _account;
        private static string _endpointUrl;
        private static HttpClient _httpClient;

        [TestInitialize]
        public void InitTest()
        {
            //_httpClient = new HttpClient();
            //_endpointUrl = Config["Values:ServiceEndpoint"];
            //_svcWrapper = new TokenServiceWrapper(_httpClient, _endpointUrl);
            //_accountSvcWrapper = new AccountServiceWrapper(_httpClient, _endpointUrl, Config["Values:PartyID"], Config["Values:BlockchainNetworkID"]);
            //_tokenID = Config["Values:TokenID"];
            //_userA = Config["Values:UserA_NFTServiceID"]; 
            //_Contoso = Config["Values:ContosoID"];
            //_ContosoProductManager = Config["Values:ContosoProductManager"];
            //if (_account == null)
            //    _account = Guid.NewGuid().ToString();
        }

        [TestCleanup]
        public async Task UnregisterUser()
        {
            await _accountSvcWrapper.DeleteUser(_account);
        }

        [TestMethod]
        public async Task TEST_1_RegisterUser()
        {
            var registeredUser = await _accountSvcWrapper.RegisterAccount(_account);

            Console.WriteLine($"{registeredUser.Name} / {registeredUser.Id} / {registeredUser.PublicAddress} / {registeredUser.BlockchainNetwork}");
            Assert.IsNotNull(registeredUser.Id);
        }

        [TestMethod]
        public async Task TEST_2_Token_Should_be_Created()
        {
            string dynamicTokenName = $"ContosoCryptoGoodToken";
            var retResult = await _svcWrapper.CreateToken(dynamicTokenName, "CK", _ContosoProductManager);


            Console.WriteLine($"Created token ID is {retResult.ContractAddress}");
            _tokenID = retResult.ContractAddress; 

            Assert.IsNotNull(retResult);

            var tokenName = await _svcWrapper.GetName(_tokenID, _Contoso);
            Debug.Print($"Checking token name by token id : {tokenName}");

            Assert.IsTrue(dynamicTokenName.Equals(tokenName));

            var tokenSymbol = await _svcWrapper.GetSymbol(_tokenID, _Contoso);
            Assert.IsTrue(tokenSymbol.Equals("CK"));
        }

        [TestMethod]
        public async Task TEST_3_Token_Should_be_Minted_transfered()
        {
            long tokenNumber = System.DateTime.Now.Ticks;
           
            var mintedResult = await _svcWrapper.MintToken(_tokenID, _ContosoProductManager, _Contoso, "foo2", tokenNumber);


            var ContosoBalance = await _svcWrapper.GetBalance(_tokenID, _Contoso);
            var UserABlance = await _svcWrapper.GetBalance(_tokenID, _userA);

            var tokenMeta = await _svcWrapper.GetTokenMetaData(_tokenID, _Contoso, tokenNumber);
            Assert.IsTrue(tokenMeta.Equals("foo2"));

            ContosoBalance = await _svcWrapper.GetBalance(_tokenID, _Contoso);
            UserABlance = await _svcWrapper.GetBalance(_tokenID, _userA);

            Console.WriteLine($"Contoso's balance is {ContosoBalance}");
            Console.WriteLine($"USER A's balance is {UserABlance}");

            var transferedResult = await _svcWrapper.TranferToken(_tokenID, tokenNumber, _Contoso, _Contoso, _userA);


            ContosoBalance = await _svcWrapper.GetBalance(_tokenID, _Contoso);
            UserABlance = await _svcWrapper.GetBalance(_tokenID, _userA);

            var metaString = await _svcWrapper.GetTokenMetaData(_tokenID, _userA, tokenNumber);


            var user = await _accountSvcWrapper.GetAccountAsync(_userA);


            var addressofOwner = await _svcWrapper.WhoisOwner(_tokenID, _userA, tokenNumber);

            Console.WriteLine($"user's public address : {user.PublicAddress} / addressof Owener : {addressofOwner}");

            Assert.IsTrue(user.PublicAddress.ToLower().Equals(addressofOwner.ToLower()));

            Console.WriteLine($"Token value from User A is {metaString}");

            Console.WriteLine($"Contoso's balance is {ContosoBalance}");
            Console.WriteLine($"USER A's balance is {UserABlance}");
        }

     



    }
}
