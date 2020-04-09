using Contoso.DigitalGoods.TokenService.ServiceWrapper.Messages;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
    interface IContosoTokenServiceAccount
    {
        Task<Account> RegisterAccount(string ContosoUserIdentifier);
    }
}
