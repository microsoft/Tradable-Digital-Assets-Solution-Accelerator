using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Azure.TokenService.Models;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.SetUp.Interface
{
    interface ICryptoGoodsSetup
    {
        Task<GenericResource> SetupContosoProductManagerProfile();
        Task<GenericResource> SetupContosoProfile();
        Task SetupDigitalLocker();
        Task SetupProductCatalog();

        Task<GenericResource> SetupGroup();
        Task<GenericResource> SetupBlockChainNetwork();

        Task<AsyncResponse> InitializeToken(string productManager);

    }
}
