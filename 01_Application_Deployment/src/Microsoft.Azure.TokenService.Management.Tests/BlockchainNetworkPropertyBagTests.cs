using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Microsoft.Azure.TokenService.Management.Model.Tests
{
    [TestClass()]
    public class BlockchainNetworkPropertyBagTests
    {
        [TestMethod()]
        public void TEST_1_ToProperyDictionaryTest()
        {
            var propertyBag = new BlockchainNetworkRequestPropertyBag()
            {
                BlockchainNetworkId = "my name is Foo",
                blockchainNode = "http://foo",
                description = "bla bla bla"
            };

            var propdic = propertyBag.ToDictionary();

            Assert.IsInstanceOfType(propdic, typeof(Dictionary<string, object>));
        }
    }
}