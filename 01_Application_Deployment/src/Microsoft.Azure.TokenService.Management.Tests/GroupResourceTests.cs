using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.TokenService.Management.Tests
{
    [TestClass()]
    public class GroupResourceTests
    {
        private string _groupName;

        [TestInitialize]
        public void init()
        {
            _groupName = Guid.NewGuid().ToString();
        }

        [TestMethod()]
        public async Task TEST_1_RegisterOrUpdateAsyncTest()
        {
            GroupResource gr = new GroupResource();
            var result = await gr.RegisterOrUpdateAsync(new Model.GroupRequestPropertyBag()
            {
                GroupName = _groupName,
                description = "this is Contoso group"
                //GroupName = "Contoso",
                //description = "Contoso group"
            });

            Assert.IsTrue(result.Name == _groupName);
        }

        [TestMethod()]
        public async Task TEST_2_GetAsyncTest()
        {
            GroupResource gr = new GroupResource();
            var result = await gr.GetAsync("msft");
            Assert.IsTrue(result.value.Length > 0);
            Assert.IsTrue(result.value[0].name == "msft");
        }

        [TestMethod()]
        public async Task TEST_3_GetAllAsyncTest()
        {
            GroupResource gr = new GroupResource();
            var result = await gr.GetAllAsync();

            foreach (var item in result.value)
            {
                Console.WriteLine(JsonConvert.SerializeObject(item));
            }

            Assert.IsTrue(result.value.Length > 0);
        }

        //[TestMethod()]
        //public async Task TEST_4_UnRegisterAsyncTest()
        //{
        //    GroupResource gr = new GroupResource();
        //    await gr.UnRegisterAsync("testgroup");
        //}
    }
}