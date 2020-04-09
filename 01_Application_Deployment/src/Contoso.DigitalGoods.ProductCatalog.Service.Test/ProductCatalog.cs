using Contoso.DigitalGoods.ProductCatalog.Service.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.ProductCatalog.Service.Test
{
    [TestClass]
    public class ProductCatalog
    {
        private string productID;
        private string productSubID;
        private string productDes;
        private string mongoConnectionString;
        private ProductCatalogManager _manager;
        private string tokenAPIURL;

        [TestInitialize]
        public void InitTest()
        {
            //connstring should be removed
            mongoConnectionString = "mongodb://cryptogood-app:PbUAtgv7QITNdy3fILD6s7CWVJYIzHXFDRpfYKhlNif1btbgwSX5ujbSq2ck9xNClucfMuoMWDrQOeg3jNQBlQ==@cryptogood-app.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
            tokenAPIURL = "http://http://13.66.94.77/";
            productID = "TestProduct";
            productSubID = "TestSub";
            productDes = "TestDes";

            _manager = new ProductCatalogManager(mongoConnectionString, "ProductCatalog");

        }

        [TestMethod]
        public async Task RegisterProduct()
        {
            var product = new ProductInfo()
            {
                ProductID = productID,
                Subtitle = productSubID,
                Description = productDes
            };

            var result = await _manager.RegisterProductCatalog(product);
            var unRegister = _manager.UnRegisterProductCatalog(productID);

            Assert.IsInstanceOfType(result, typeof(ProductInfo));
            Assert.IsTrue(unRegister);
        }

        [TestMethod]
        public void DeleteFakeProduct()
        {
            productID = "NotARealProduct";
            var result = _manager.UnRegisterProductCatalog(productID);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetTestProductAsync()
        {
            productID = "TestProductDontDelete";
            var result = _manager.GetProductInfo(productID);
            ; Assert.IsInstanceOfType(result, typeof(ProductInfo));
        }
    }
}
