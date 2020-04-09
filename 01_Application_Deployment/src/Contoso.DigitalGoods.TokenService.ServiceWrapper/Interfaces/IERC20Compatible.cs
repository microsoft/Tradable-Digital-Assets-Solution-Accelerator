using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
    public interface IERC20Compatible
    {
        Task<string> GetName(string TokenID, string CallerID, string CallerGroupName);
        Task<string> GetSymbol(string TokenID, string CallerID, string CallerGroupName);
        Task<long?> GetTotalSupply(string TokenID, string CallerID, string CallerGroupName);
        Task<long?> GetBalance(string TokenID, string CallerID, string CallerGroupName);

    }
}
