using Contoso.DigitalGoods.ProductCatalog.Service.Models;
using Contoso.DigitalGoods.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.ProductCatalog.Service.Test
{
    [TestClass]
    public class ProductCatalog : TestBase
    {
        private static string productID;
        private static string productSubID;
        private static string productDes;
        private static string mongoConnectionString;
        private ProductCatalogManager _manager;
        

        [TestInitialize]
        public void InitTest()
        {
            //connstring should be removed
            mongoConnectionString = Config["Values:offchain_appconnectionstring"];
            productID = "";
            productSubID = "";
            productDes = "";

            _manager = new ProductCatalogManager(mongoConnectionString, "ProductCatalog");

        }

        [TestMethod]
        public async Task Test0_RegisterProduct()
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
        public void Test2_DeleteFakeProduct()
        {
            productID = "NotARealProduct";
            var result = _manager.UnRegisterProductCatalog(productID);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Test1_GetTestProductAsync()
        {
            productID = "TestProductDontDelete";
            var result = _manager.GetProductInfo(productID);
            Assert.IsInstanceOfType(result, typeof(ProductInfo));
        }
    }
}
