using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Contoso.DigitalGoods.ContosoProfile.Service;

namespace Contoso.DigitalGoods.ContosoProfile.Service.Test
{
    [TestClass]
    public class ContosoProfileTest
    {
        private string mongoConnectionString;
        private string tokenAPIURL;
        private ContosoProfileManager _manager;
        private string Contosoid;
        private string abtUserID;

        [TestInitialize]
        public void InitTest()
        {
            //connstring should be removed
            mongoConnectionString = "mongodb://cryptokick-app:PbUAtgv7QITNdy3fILD6s7CWVJYIzHXFDRpfYKhlNif1btbgwSX5ujbSq2ck9xNClucfMuoMWDrQOeg3jNQBlQ==@cryptokick-app.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
            tokenAPIURL = "http://13.66.95.45";

            //userid need to be changed at your env.
            Contosoid = Guid.NewGuid().ToString();
            abtUserID = Guid.NewGuid().ToString();
            _manager = new ContosoProfileManager(mongoConnectionString, "UserProfile", tokenAPIURL);
        }

        // [TestMethod]
        // public async Task ProvisioningContosoProfile()
        // {
        //     var result =
        //           await _manager.ProvisionContosoProfile(Contosoid);

        //     Assert.IsInstanceOfType(result, typeof(Models.ContosoProfile));
        //     _manager.DeleteProfile(Contosoid);
        // }

        [TestMethod]
        public async Task ContosoProfileExsits()
        {
            Contosoid = "DONTDELETE";
            var result =
                  await _manager.ProvisionContosoProfile(Contosoid);
            Assert.IsNull(result);
        }


        [TestMethod]
        public void ContosoProfileDosentExsits()
        {
            Contosoid = "DONTDELETE2";
            var result =
                   _manager.CheckForContosoDuplicates(Contosoid);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetContosoProfile()
        {
            Contosoid = "DONTDELETE";
            var CryptoGoods = _manager.GetUserProfileByContosoID(Contosoid);

            Assert.IsTrue(CryptoGoods.ContosoID == Contosoid);
        }
    }
}
