using Microsoft.Azure.TokenService.Proxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
namespace Microsoft.Azure.TokenService.Management.Tests
{
    [TestClass()]
    public class AccountResourceTests
    {
        private static Guid _account;
        private static UsersClient _usersClient;
        private static string serviceEndpoint = "http://52.137.97.182";
        private static string _blockchainNetworkID = "75244f45-1bde-4adf-982f-29dd3f69056c";
        private static string _partyID = "88c3a148-5158-402e-a59b-34406eb00142";

        [TestInitialize()]
        public void init()
        {
            if (_usersClient == null) _usersClient = new UsersClient(serviceEndpoint); 
        }


        [TestMethod()]
        public async Task Task_4_UnRegisterAsyncTest()
        {

            await _usersClient.UserDeleteAsync(_account);
        }

        [TestMethod()]
        public async Task Test_1_RegisterOrUpdateAsyncTest()
        {
            var result = await _usersClient.UserPostAsync(new UserInfo()
            {
                Name = "Test User",
                BlockchainNetworkID = Guid.Parse(_blockchainNetworkID),
                PartyID = Guid.Parse(_partyID)
            });

            _account = result.Id;

            Console.WriteLine($"{result.Name} /{JsonConvert.SerializeObject(result)}");

            //Assert.IsTrue(result.Name == userAccount);
        }

        [TestMethod()]
        public async Task Test_3_GetAllAsyncTest()
        {
            //AccountResource ar = new AccountResource();
            //ar.GroupName = "Contoso";

            ServiceManagementClient svcMgmtClient = new ServiceManagementClient(serviceEndpoint);
            var results = await svcMgmtClient.UsersAsync();

            int count = 0;

            foreach (var item in results)
            {
               
                    dynamic profile = new
                    {
                        ContosoID = item.Name,
                        ABTUserID = item.Id,
                        PublicAddress = item.PublicAddress,
                        BlockchainNetworkId = item.BlockchainNetwork.Id
                    };
                    Console.WriteLine(JsonConvert.SerializeObject(profile));
                
            }

            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod()]
        public async Task Test_2_GetAsyncTest()
        {

            var result = await _usersClient.UserPostAsync(_account);
            Assert.IsTrue(result.Name == "Test User");

        }

        [TestMethod]
        public async Task Test_7_CreateBlockchainNetwork()
        {
            BlockchainNetwork blockchainNetwork = new BlockchainNetwork();
            var bcClient = new BlockchainNetworksClient(serviceEndpoint);
            var result = await bcClient.BlockchainNetworkPostAsync(new BlockchainNetworkInfo()
            {
                Name = "foo Network",
                NodeURL = "http://foo",
                Description = "Test"
            });

            //var result = await blockchainNetwork.RegisterOrUpdateAsync(new Model.BlockchainNetworkRequestPropertyBag()
            //{
            //    BlockchainNetworkId = "foo2",
            //    blockchainNode = "http://foo.com"
            //});

            Console.WriteLine(result.ToString());
        }

      

    }
}