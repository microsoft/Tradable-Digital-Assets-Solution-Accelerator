using Contoso.DigitalGoods.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.IO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.ContosoProfile.Service.Test
{
    [TestClass]
    public class ContosoProfileTest : TestBase
    {
        private static string mongoConnectionString;
        private static string tokenAPIURL;
        private static ContosoProfileManager _manager;
        private static string Contosoid;
        private static string NFTServiceUserID;


        [TestInitialize]
        public void InitTest()
        {
            //connstring should be removed
            mongoConnectionString = Config["Values:offchain_appconnectionstring"];
            tokenAPIURL = Config["Values:ServiceEndpoint"];

            //userid need to be changed at your env.
            if (Contosoid == null)
                Contosoid = Guid.NewGuid().ToString();
            if (NFTServiceUserID == null)
                NFTServiceUserID = Guid.NewGuid().ToString();
            if (_manager == null)
                _manager = new ContosoProfileManager(mongoConnectionString, "UserProfile", tokenAPIURL);
        }

      
        [TestMethod]
        public async Task Test0_ProvisioningContosoProfile()
        {
            var result =
                  await _manager.ProvisionContosoProfile(Contosoid);

            Console.WriteLine($"User Profile : {Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
            Assert.IsInstanceOfType(result, typeof(Models.ContosoProfile));
        }

     
        [TestMethod]
        public void Test1_GetContosoProfile()
        {
            var CryptoGoods = _manager.GetUserProfileByContosoID(Contosoid);

            Assert.IsTrue(CryptoGoods.ContosoID == Contosoid);

            //_manager.DeleteProfile(Contosoid);
        }
    }
}
