using Microsoft.Azure.TokenService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.ServiceWrapper.Test
{
    [TestClass]
    public class TestWrapper
    {

        const string Contoso = "5166be8d-476f-43a8-a915-6a82fa62a711";
        const string UserA = "e605c240-ebb6-4c8a-a16a-455ad985c91a";
        const string UserB = "187156f7-930e-44b9-bdb0-60fa6121e079";
        const string tokenid = "daaa7de1-b067-4a8c-978d-660340675e9d";
        const string ContosoProductManager = "ba557ba3-980e-45c7-b354-7407bc1814e4";

        private static AzureTokenServiceAPI _api;
        private static TokenServiceWrapper _svcWrapper;
        private static AccountServiceWrapper _accountSvcWrapper;
        private string _tokenID;
        private string _ContosoProductManager;
        private string _Contoso;
        private string _userA;
        private static string _account;

        [TestInitialize]
        public void InitTest()
        {
            TokenAPIService tokenAPIConnection = new TokenAPIService();
            _api = tokenAPIConnection.Initialize();

            _api.TokenName = Guid.NewGuid().ToString();

            _svcWrapper = new TokenServiceWrapper(_api);
            _accountSvcWrapper = new AccountServiceWrapper(_api);
            _tokenID = "69247203-2a55-4eec-b86f-7e579e7c99e6";
            _Contoso = "04cbac3c-e950-466e-9935-416738563d97";
            _ContosoProductManager = "ContosoProductManager";
            _userA = "8b109889-0632-4658-9444-91f1c7b2e6f1";
            _account = Guid.NewGuid().ToString();
        }

        //[TestCleanup]
        //public async Task UnregisterUser()
        //{
        //    await _accountSvcWrapper.DeleteUser(_account);
        //}

        // [TestMethod]
        // public async Task TEST_1_RegisterUser()
        // {
        //     var registeredUser = await _accountSvcWrapper.RegisterAccount(_account);

        //     Console.WriteLine($"{registeredUser.Name} / {registeredUser.Id} / {registeredUser.PublicAddress} / {registeredUser.BlockchainNetworkName}");
        //     Assert.IsNotNull(registeredUser.Id);
        // }

        [TestMethod]
        public async Task TEST_2_Token_Should_be_Created()
        {
            string dynamicTokenName = $"ContosoCryptoGoodToken";
            var retResult = await _svcWrapper.CreateToken(dynamicTokenName, "CK", _ContosoProductManager);


            Console.WriteLine($"Created token ID is {retResult.Id}");
            _tokenID = retResult.Id.Replace("tokens.", "");

            WaitSeconds(10);

            Assert.IsNotNull(retResult);

            //var tokenName = await _svcWrapper.GetName(_tokenID, _Contoso);
            var tokenName = await _svcWrapper.GetName(_tokenID, _Contoso);
            Debug.Print($"Checking token name by token id : {tokenName}");

            Assert.IsTrue(dynamicTokenName.Equals(tokenName));

            var tokenSymbol = await _svcWrapper.GetSymbol(_tokenID, _Contoso);
            Assert.IsTrue(tokenSymbol.Equals("CK"));
        }

        // [TestMethod]
        // public async Task TEST_3_Token_Should_be_Minted_transfered()
        // {
        //     long tokenNumber = System.DateTime.Now.Ticks;
        //     _tokenID = "e140784c-bb56-4b13-9baf-737a3cae0b54";

        //     var mintedResult = await _svcWrapper.MintToken(_tokenID, _ContosoProductManager, _Contoso, "foo2", tokenNumber);

        //     WaitSeconds(8);

        //     var ContosoBalance = await _svcWrapper.GetBalance(_tokenID, _Contoso);
        //     var UserABlance = await _svcWrapper.GetBalance(_tokenID, _userA);

        //     var tokenMeta = await _svcWrapper.GetTokenMetaData(_tokenID, _Contoso, tokenNumber);
        //     Assert.IsTrue(tokenMeta.Output.Equals("foo2"));

        //     ContosoBalance = await _svcWrapper.GetBalance(_tokenID, _Contoso);
        //     UserABlance = await _svcWrapper.GetBalance(_tokenID, _userA);

        //     Console.WriteLine($"Contoso's balance is {ContosoBalance}");
        //     //Console.WriteLine($"USER A's balance is {UserABlance}");

        //     var transferedResult = await _svcWrapper.TranferToken(_tokenID, tokenNumber, _Contoso, _Contoso, _userA);

        //     WaitSeconds(9);

        //     ContosoBalance = await _svcWrapper.GetBalance(_tokenID, _Contoso);
        //     UserABlance = await _svcWrapper.GetBalance(_tokenID, _userA);

        //     var metaString = await _svcWrapper.GetTokenMetaData(_tokenID, _userA, tokenNumber);

        //     AccountResource account = new AccountResource();
        //     account.GroupName = "Contoso";

        //     var user = await account.GetAsync(_userA);


        //     var addressofOwner = await _svcWrapper.IsItMyToken(_tokenID,  _userA, tokenNumber);

        //     Console.WriteLine($"user's public address : {user.value[0].properties.publicAddress} / addressof Owener : {addressofOwner.Output}");

        //     Assert.IsTrue(user.value[0].properties.publicAddress.ToLower() == addressofOwner.Output.ToLower());

        //     Console.WriteLine($"Token value from User A is {metaString.Output}");

        //     Console.WriteLine($"Contoso's balance is {ContosoBalance}");
        //     Console.WriteLine($"USER A's balance is {UserABlance}");
        // }

        public static void WaitSeconds(int seconds)
        {
            var destinationTime = DateTime.Now.AddSeconds(seconds);
            Console.Write("Waiting to be transacted.....");
            while (DateTime.Now < destinationTime)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }
            Console.WriteLine("");
        }



    }
}
