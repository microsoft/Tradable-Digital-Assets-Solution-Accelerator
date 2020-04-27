using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.TokenService.Models;
using Microsoft.Azure.TokenService.Proxy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.Interfaces
{
    public interface ITokenServiceAgent
    {
        Task<TokenMeta> CreateToken(string TokenName, string TokenSymbol, string CallerID);
        Task DeleteAccount(string ABTUserID);
        Task<User> GetAccountInfo(string ABTUserID);
        Task<IEnumerable<User>> GetAllAccounts();
        Asset GetDigitalGoodFromDigitalLocker(string ABTUserID, long TokenNumber);
        Task<DigitalKickToken> GetDigitalGoodfromToken(string ABTUserID, long TokenNumber);
        Task<TokenMeta> GetTokenInfo(string TokenId, string CallerID);
        List<Asset> GetUserDigitalLocker(string ABTUserID);
        Task<bool> GiftDigitalGoods(string Recipient, long TokenNumber);
        Task<bool> IsitMyToken(long? TokenNumber, string CallerID);
        Task<DigitalKickToken> MakeDigitalGoods(DigitalKickToken digitalGoodToken);
        Task<string> ProvisionUser(string ContosoUserIdentifier);
        Task<bool> TransferDigitalGoods(string Sender, string Recipient, long TokenNumber);
    }
}