using Contoso.DigitalGoods.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contoso.DigitalGoods.TokenService.TokenNumberManager.Test
{
    [TestClass]
    public class TestTokenNumberManager : TestBase
    {
        private string mongoConnectionString;
        private TokenNumberManager _manager;
        private string dummyTokenID;

        [TestInitialize]
        public void InitTest()
        {
            //connstring should be removed
            //mongoConnectionString = Config["Values:offchain_connectionstring"];
            _manager = new TokenNumberManager(mongoConnectionString, "TokenNumberManager");
            dummyTokenID = "d6f93c33-3f90-4fc9-a07b-843378e3f475";
        }

        [TestMethod]
        public void TEST_1_Should_return_next_number()
        {
            var nextNum = _manager.GetNextNumber(dummyTokenID);
            var thenextNum = _manager.GetNextNumber(dummyTokenID);
            Assert.IsTrue(thenextNum == ++nextNum);
        }
    }
}
