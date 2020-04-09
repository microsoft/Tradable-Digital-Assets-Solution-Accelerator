using Microsoft.Azure.TokenService;

namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
    public interface ITokenService
    {
        AzureTokenServiceAPI Initialize();
    }
}
