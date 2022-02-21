using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.TokenService.Models;
using Microsoft.Solutions.NFT;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.Interfaces
{
    public interface ITokenServiceAgent
    {
        Task<TokenMeta> CreateToken(string TokenName, string TokenSymbol, string CallerID);
        Task DeleteAccount(string NFTServiceUserID);
        Task<User> GetAccountInfo(string NFTServiceUserID);
        Task<IEnumerable<User>> GetAllAccounts();
        Asset GetDigitalGoodFromDigitalLocker(string NFTServiceUserID, long TokenNumber);
        Task<DigitalGoodToken> GetDigitalGoodfromToken(string NFTServiceUserID, long TokenNumber);
        Task<TokenMeta> GetTokenInfo(string TokenId, string CallerID);
        List<Asset> GetUserDigitalLocker(string NFTServiceUserID);
        Task<bool> GiftDigitalGoods(string Recipient, long TokenNumber);
        Task<bool> IsitMyToken(long? TokenNumber, string CallerID);
        Task<DigitalGoodToken> MakeDigitalGoods(DigitalGoodToken digitalGoodToken);
        Task<string> ProvisionUser(string ContosoUserIdentifier);
        Task<bool> TransferDigitalGoods(string Sender, string Recipient, long TokenNumber);
    }
}