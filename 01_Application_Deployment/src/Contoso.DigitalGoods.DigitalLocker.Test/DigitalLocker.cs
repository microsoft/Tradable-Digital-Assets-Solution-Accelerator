
using Contoso.DigitalGoods.DigitalLocker.App;
using Contoso.DigitalGoods.ProductCatalog.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contoso.DigitalGoods.DigitalLocker.Test
{
    [TestClass]
    public class DigitalLocker
    {
        private CryptoGoodLocker _manager;
        private IProductCatalogManager productCatalogManager;
        private string mongoConnectionString;
        private string tokenAPIURL;
        private string testAbtUserID;

        [TestInitialize]
        public void InitTest()
        {
            //mongoConnectionString = "mongodb://cryptogood-app:PbUAtgv7QITNdy3fILD6s7CWVJYIzHXFDRpfYKhlNif1btbgwSX5ujbSq2ck9xNClucfMuoMWDrQOeg3jNQBlQ==@cryptogood-app.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
            tokenAPIURL = "http://http://13.66.94.77/";
            _manager = new CryptoGoodLocker(productCatalogManager, tokenAPIURL);
        }

        //[TestMethod]
        //public void GetAllDigitalLockerItemsTest()
        //{
        //    testAbtUserID = "TestLocker";
        //    var result = _manager.GetAllUserDigitalLockerItems(testAbtUserID);
        //    Assert.IsInstanceOfType(result, typeof(DigitalLockerItem));
        //}


        //[TestMethod]
        //public void GetCryptoGoodFromDigitalLockerByTokenNumberTest()
        //{
        //}
    }
}
