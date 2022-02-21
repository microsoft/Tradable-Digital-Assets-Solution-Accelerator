using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.TokenService.Interfaces;
using Contoso.DigitalGoods.TokenService.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Solutions.NFT;
using Microsoft.Extensions.Configuration;
using Microsoft.Solutions.CosmosDB.Security.ManagedIdentity;

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

        public ServiceAgent(IConfiguration Configuration, TokenManager TokenManager, UserManager UserManager)
        {
            tokenID = Configuration["Values:TokenID"];
            contosoProductManager = Configuration["Values:ContosoProductManager"];
            contosoID = Configuration["Values:ContosoID"];

            tokenManager = TokenManager;
            userManager = UserManager;
        }

        /// <summary>
        /// Register User with ContosoUserIdentifier then
        /// Create User's Digital Locker
        /// </summary>
        /// <param name="ContosoUserIdentifier"></param>
        /// <returns>NFTService User Identifier</returns>
        public async Task<string> ProvisionUser(string ContosoUserIdentifier)
        {
            return await userManager.ProvisionUser(ContosoUserIdentifier);
        }

        public async Task<bool> GiftDigitalGoods(string Recipient, long TokenNumber)
        {
            return await TransferDigitalGoods(contosoID, Recipient, TokenNumber);
        }

        public async Task<bool> TransferDigitalGoods(string Sender, string Recipient, long TokenNumber)
        {
            //Transfer Token by Blockchain
            await tokenManager.TransferCryptoGoods(tokenID, TokenNumber, Sender, Sender, Recipient);
            //Transfer Asset from DigitalLocker
            return await userManager.TransferAsset(Sender, Recipient, TokenNumber);
        }

        public async Task<DigitalGoodToken> MakeDigitalGoods(DigitalGoodToken cryptoGoodToken)
        {
            //Mint Token by Blockchain
            var tokenInfo = await tokenManager.MintToken(tokenID, contosoProductManager, contosoID, cryptoGoodToken);
            //Add Contoso's Digital Locker
            await userManager.AddCryptoGoods(contosoID, tokenInfo);

            return tokenInfo;
        }

        public async Task<DigitalGoodToken> GetDigitalGoodfromToken(string NFTServiceUserID, long TokenNumber)
        {
            var tokenInfo = await tokenManager.GetCryptoGoodInfoFromToken(tokenID, NFTServiceUserID, TokenNumber);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DigitalGoodToken>(tokenInfo);
        }

        public List<Asset> GetUserDigitalLocker(string NFTServiceUserID)
        {
            return userManager.GetAllUserCryptoGoods(NFTServiceUserID);
        }

        public Asset GetDigitalGoodFromDigitalLocker(string NFTServiceUserID, long TokenNumber)
        {
            return userManager.GetCryptoGoodFromUserLocker(NFTServiceUserID, TokenNumber);
        }

        public async Task<TokenMeta> CreateToken(string TokenName, string TokenSymbol, string CallerID)
        {
            return await tokenManager.CreateToken(TokenName, TokenSymbol, CallerID);
        }

        public async Task<User> GetAccountInfo(string NFTServiceUserID)
        {
            return await userManager.GetUserInfo(NFTServiceUserID);
        }

        public async Task<IEnumerable<User>> GetAllAccounts()
        {
            return await userManager.GetAllUsers();
        }

        public async Task DeleteAccount(string NFTServiceUserID)
        {
            await userManager.DeleteUser(NFTServiceUserID);
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
