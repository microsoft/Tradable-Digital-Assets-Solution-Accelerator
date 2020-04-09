using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.TokenService.Interfaces;
using Contoso.DigitalGoods.TokenService.Models;
using Contoso.DigitalGoods.TokenService.ServiceWrapper.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService
{
    public class ServiceAgent : ITokenServiceAgent
    {
        private TokenManager tokenManager;
        private UserManager userManager;

        //Should created
        private string tokenID;
        //Should created
        private string contosoID; //Contoso Account
        //Should created
        private string contosoProductManager; //Token Owner

        public ServiceAgent(string TokenID, string ContosoProductManager, string ContosoID, string GroupName ,string CosmosConnectionString)
        {
            tokenManager = new TokenManager(CosmosConnectionString, GroupName);
            userManager = new UserManager(CosmosConnectionString);

            tokenID = TokenID;
            contosoID = ContosoID;
            contosoProductManager = ContosoProductManager;
        }

        /// <summary>
        /// Register User with ContosoUserIdentifier then
        /// Create User's Digital Locker
        /// </summary>
        /// <param name="ContosoUserIdentifier"></param>
        /// <returns>ABT User Identifier</returns>
        public async Task<string> ProvisionUser(string ContosoUserIdentifier)
        {
            return await userManager.ProvisionUser(ContosoUserIdentifier);
        }

        public async Task<bool> GiftCryptoGoods(string Recipient, long TokenNumber)
        {
            return await TransferCryptoGoods(contosoID, Recipient, TokenNumber);
        }

        public async Task<bool> TransferCryptoGoods(string Sender, string Recipient, long TokenNumber)
        {
            //Transfer Token by Blockchain
            await tokenManager.TransferCryptoGoods(tokenID, TokenNumber, Sender, Sender, Recipient);
            //Transfer Asset from DigitalLocker
            return await userManager.TransferAsset(Sender, Recipient, TokenNumber);
        }

        public async Task<CryptoKickToken> MakeCryptoGoods(CryptoKickToken cryptoGoodToken)
        {
            //Mint Token by Blockchain
            var tokenInfo = await tokenManager.MintToken(tokenID, contosoProductManager, contosoID, cryptoGoodToken);
            //Add Contoso's Digital Locker
            await userManager.AddCryptoGoods(contosoID, tokenInfo);

            return tokenInfo;
        }

        public async Task<CryptoKickToken> GetCryptoGoodfromToken(string ABTUserID, long TokenNumber)
        {
            var tokenInfo = await tokenManager.GetCryptoGoodInfoFromToken(tokenID, ABTUserID, TokenNumber);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CryptoKickToken>(tokenInfo);
        }

        public List<Asset> GetUserDigitalLocker(string ABTUserID)
        {
            return userManager.GetAllUserCryptoGoods(ABTUserID);
        }

        public Asset GetCryptoGoodFromDigitalLocker(string ABTUserID, long TokenNumber)
        {
            return userManager.GetCryptoGoodFromUserLocker(ABTUserID, TokenNumber);
        }

        public async Task<TokenMeta> CreateToken(string TokenName, string TokenSymbol, string CallerID)
        {
            return await tokenManager.CreateToken(TokenName, TokenSymbol, CallerID);
        }

        public async Task<Account> GetAccountInfo(string ABTUserID)
        {
            return await userManager.GetUserInfo(ABTUserID);
        }

        public async Task<Account[]> GetAllAccounts()
        {
            return await userManager.GetAllUsers();
        }

        public async Task DeleteAccount(string ABTUserID)
        {
            await userManager.DeleteUser(ABTUserID);
        }

        public async Task<TokenMeta> GetTokenInfo(string TokenId, string CallerID)
        {
            return await tokenManager.GetTokenInfo(TokenId, CallerID);
        }

        public async Task<bool> IsitMyToken(long? TokenNumber, string CallerID)
        {
            return await tokenManager.IsItMyToken(tokenID, CallerID, TokenNumber);
        }
    }
}
