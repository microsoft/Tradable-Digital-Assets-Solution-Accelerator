using Microsoft.Azure.TokenService.Management.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.TokenService.Management.Tests
{
    [TestClass()]
    public class BlockchainNetworkTests
    {
        private string _blockchainName;
        [TestInitialize]
        public void init()
        {
            //Contosocryptogood
            _blockchainName = Guid.NewGuid().ToString();
        }


        [TestMethod()]
        public async Task TEST_1_RegisterOrUpdateTest()
        {
            BlockchainNetwork blockchainNetwork = new BlockchainNetwork();
            BlockchainNetworkRequestPropertyBag propBag = new BlockchainNetworkRequestPropertyBag()
            {
                //BlockchainNetworkId = "cryptogoodchain",
                //blockchainNode = "https://Contosomember01.blockchain.azure.com:3200/f24gVpuHVS8npNjWbIgUfQ1P",
                //description = "cryptoGood transaction node"
                BlockchainNetworkId = _blockchainName,
                blockchainNode = "https://",
                description = "cryptoGood test transaction node"
            };

            var result = await blockchainNetwork.RegisterOrUpdateAsync(propBag);
            Console.WriteLine(result.Name);

            Assert.IsTrue(((Newtonsoft.Json.Linq.JObject)result.Properties).Value<string>("description") == propBag.description);
        }

        [TestMethod()]
        public async Task TEST_2_GetAsyncTest()
        {
            BlockchainNetwork blockchainNetwork = new BlockchainNetwork();
            var result = await blockchainNetwork.GetAsync("xinxlbcn1");

            Assert.IsTrue(result.value.Length > 0);
            Assert.IsTrue(result.value[0].name == "xinxlbcn1");

        }

        [TestMethod()]
        public async Task TEST_3_GetAllAsyncTest()
        {
            BlockchainNetwork blockchainNetwork = new BlockchainNetwork();
            var result = await blockchainNetwork.GetAllAsync();

            foreach (var item in result.value)
            {
                Console.WriteLine(item.name);
            }

            Assert.IsNotNull(result.value.Length > 0);
        }


        [TestMethod()]
        public async Task TEST_4_UnRegisterTest()
        {
            BlockchainNetwork blockchainNetwork = new BlockchainNetwork();
            await blockchainNetwork.UnRegisterAsync("DBChain");
        }
    }
}