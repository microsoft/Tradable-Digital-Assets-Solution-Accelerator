using Microsoft.Azure.TokenService;

namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
    public class TokenServiceWrapperBase
    {
        protected AzureTokenServiceAPI tokenServiceAPI;

        public TokenServiceWrapperBase(AzureTokenServiceAPI api)
        {
            tokenServiceAPI = api;
        }
    }
}
