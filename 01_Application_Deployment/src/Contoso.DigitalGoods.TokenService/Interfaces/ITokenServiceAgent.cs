using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.TokenService.Models;
using Contoso.DigitalGoods.TokenService.ServiceWrapper.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.Interfaces
{
    public interface ITokenServiceAgent
    {
        Task<string> ProvisionUser(string ContosoUserIdentifier);
        Task<bool> GiftCryptoGoods(string Recipient, long TokenNumber);
        Task<bool> TransferCryptoGoods(string Sender, string Recipient, long TokenNumber);
        Task<CryptoKickToken> MakeCryptoGoods(CryptoKickToken cryptoGoodToken);
        List<Asset> GetUserDigitalLocker(string ABTUserID);
        Asset GetCryptoGoodFromDigitalLocker(string ABTUserID, long TokenNumber);
        Task<CryptoKickToken> GetCryptoGoodfromToken(string ABTUserID, long TokenNumber);
        Task<TokenMeta> CreateToken(string TokenName, string TokenSymbol, string CallerID);
        Task<TokenMeta> GetTokenInfo(string TokenId, string CallerID);

        Task<Account[]> GetAllAccounts();
        Task DeleteAccount(string ABUUserID);

        Task<bool> IsitMyToken(long? TokenNumber, string CallerID);
        Task<Account> GetAccountInfo(string aBTUserID);
    }
}
