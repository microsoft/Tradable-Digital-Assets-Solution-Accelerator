using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
namespace Microsoft.Azure.TokenService.Management.Tests
{
    [TestClass()]
    public class AccountResourceTests
    {
        private static string _account;

        [TestInitialize()]
        public void init()
        {
            AccountResourceTests._account = Guid.NewGuid().ToString();
        }


        [TestMethod()]
        public async Task Task_4_UnRegisterAsyncTest()
        {
            AccountResource ar = new AccountResource();
            ar.GroupName = "Contoso";

            //await ar.UnRegisterAsync("Contosoproductmanager");
            await ar.UnRegisterAsync("65a6ca84-6448-4565-b880-e14686783c05");
        }

        [TestMethod()]
        public async Task Test_1_RegisterOrUpdateAsyncTest()
        {
            AccountResource ar = new AccountResource();
            ar.GroupName = "msft";
            string userAccount = Guid.NewGuid().ToString();


            var result = await ar.RegisterOrUpdateAsync(new Model.AccountRequestPropertyBag()
            {
                AccountName = userAccount, //"14ed4dc5-2ef4-46b6-8e4e-e63c17f4deb2",
                description = $"Contoso test user",
                blockchainNetworkId = "foo"
            });

            Console.WriteLine($"{result.Name} /{((Newtonsoft.Json.Linq.JObject)result.Properties).ToString()}");

            Assert.IsTrue(result.Name == userAccount);
        }

        [TestMethod()]
        public async Task Test_3_GetAllAsyncTest()
        {
            AccountResource ar = new AccountResource();
            ar.GroupName = "Contoso";

            var results = await ar.GetAllAsync();
            int count = 0;

            foreach (var item in results.value)
            {
                if (item.properties.provisioningState == "Succeeded")
                {
                    dynamic profile = new
                    {
                        ContosoID = item.properties.description.Replace("Contoso user -", "").Trim(),
                        ABTUserID = item.name,
                        PublicAddress = item.properties.publicAddress
                        //BlockchainNetworkId = item.properties.blockchainNetworkName
                    };
                    Console.WriteLine(JsonConvert.SerializeObject(profile));
                    //Console.WriteLine("{ ""ContosoID"" :" " + $"{item.name} {item.properties.description.Replace("Nike user -","").Trim()}" + "}");
                }
            }

            Assert.IsTrue(results.value.Length > 0);
        }

        [TestMethod()]
        public async Task Test_2_GetAsyncTest()
        {
            AccountResource ar = new AccountResource();
            //ar.GroupName = "msft";

            //var results = await ar.GetAsync("xinxl1");

            //Assert.IsTrue(results.value[0].name == "xinxl1");

            ar.GroupName = "msft";

            var results = await ar.GetAsync("b027ef79-9b95-48d6-8254-e01cdc3d7850");

            Assert.IsTrue(results.value[0].name == "b027ef79-9b95-48d6-8254-e01cdc3d7850");

        }

        [TestMethod]
        public async Task Test_7_CreateBlockchainNetwork()
        {
            BlockchainNetwork blockchainNetwork = new BlockchainNetwork();
            var result = await blockchainNetwork.RegisterOrUpdateAsync(new Model.BlockchainNetworkRequestPropertyBag()
            {
                BlockchainNetworkId = "foo2",
                blockchainNode = "http://foo.com"
            });

            Console.WriteLine(result.ToString());
        }

        //[TestMethod()]
        //public async Task Test_5_ClearallUsers()
        //{
        //    AccountResource ar = new AccountResource();
        //    ar.GroupName = "Contoso";

        //    var results = await ar.GetAllAsync();
        //    int count = 0;

        //    foreach (var item in results.value)
        //    {
        //        //Console.WriteLine($"{++count} => {item.name}");
        //        Console.WriteLine($"{item.name} will be removed");
        //        try
        //        {
        //            await ar.UnRegisterAsync(item.name);
        //        }
        //        catch { }
        //    }
        //}


        //[TestMethod()]
        //public async Task Test_6_CreateUsersUpTo50()
        //{
        //    AccountResource ar = new AccountResource();
        //    ar.GroupName = "Contoso";

        //    for (int i = 0; i < 50; i++)
        //    {
        //        var result = await ar.RegisterOrUpdateAsync(new Model.AccountRequestPropertyBag()
        //        {
        //            AccountName = Guid.NewGuid().ToString(),
        //            description = $"msft user -{_account}",
        //            blockchainNetworkName = "cryptogoodchain"
        //        });

        //        Console.WriteLine($"{(i + 1)} => {result.Name} /{((Newtonsoft.Json.Linq.JObject)result.Properties).ToString()}");
        //    }

        //}





    }
}