using Contoso.DigitalGoods.TokenService.Interfaces;
using Contoso.DigitalGoods.TokenService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Solutions.NFT;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.API.Controllers
{
    //[Authorize]
    [Route("DigitalGood/[controller]")]
    [ApiController]
    public class TokenManagementController : ControllerBase
    {
        private ITokenServiceAgent serviceAgent;

        public TokenManagementController(IHttpContextAccessor httpContextAccessor, ITokenServiceAgent ServiceAgent)
        {
            serviceAgent = ServiceAgent;
            //get ContosoUserIdentifier from JwtToken
            //ContosoUserIdentifier = JwtUtil.GetContosoIDFromJWT(httpContextAccessor);      
        }

        /// <summary>
        /// Create Contoso Token. Be aware it's the method for deploying new Token SmartContract.
        /// </summary>
        /// <param name="TokenName">Token Name ex. Contoso CryptoGoods</param>
        /// <param name="TokenSymbol">Symbol ex. NKCK. currency symbol</param>
        /// <param name="CallerID">Shoud be NFTService User ID - Should Token SmartContract owner</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Tokens")]
        public async Task<TokenMeta> CreateNonfungibleToken(string TokenName, string TokenSymbol, string CallerID)
        {
            return await serviceAgent.CreateToken(TokenName, TokenSymbol, CallerID);
        }

        /// <summary>
        /// Get Token's detail information
        /// </summary>
        /// <param name="TokenId">TokenID</param>
        /// <param name="CallerID">Caller NFTServiceUserID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Tokens/{TokenId}")]
        public async Task<TokenMeta> GetTokenInfo(string TokenId, string CallerID)
        {
            return await serviceAgent.GetTokenInfo(TokenId, CallerID);
        }

        /// <summary>
        /// Get User Account Information
        /// </summary>
        /// <param name="NFTServiceUserID">NFTServiceUserID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Users/{NFTServiceUserID}")]
        public async Task<User> GetUserInfo(string NFTServiceUserID)
        {
            return await serviceAgent.GetAccountInfo(NFTServiceUserID);
        }

        /// <summary>
        /// Provision User by Contoso's Identifier
        /// Internally Provisioning User's Digital Locker and NFTService user registration
        /// </summary>
        /// <returns>NFTService Service UserID for UserProfile Service</returns>
        [HttpPost]
        [Route("Users")]
        public async Task<string> ProvisioningUser(string ContosoUserIdentifier)
        {
            return await serviceAgent.ProvisionUser(ContosoUserIdentifier);
        }

        /// <summary>
        /// Mint Token to Contoso (Producing CryptoGood)
        /// </summary>
        /// <param name="digitalGood">CryptoGood Information</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Inventories/Users/ProductManager/DigitalGoods/")]
        public async Task<DigitalGoodToken> MakeDigitalGood(DigitalGoodToken digitalGood)
        {
            return await serviceAgent.MakeDigitalGoods(digitalGood);
        }

        //[HttpPost]
        //[Route("Management/Inventories/CryptoGoods/{CryptoGoodID}")]
        //public async Task<DigitalKickToken> Foo(DigitalKickToken cryptoGood)
        //{
        //    //return await serviceAgent.MakeCryptoGoods(cryptoGood);
        //}

        /// <summary>
        /// Get CryptoGood Token Information by TokenNumber
        /// </summary>
        /// <param name="NFTServiceUserID">NFTService User ID</param>
        /// <param name="TokenNumber">Token Number</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Inventories/Users/{NFTServiceUserID}/DigitalGoods/{TokenNumber}")]
        public async Task<DigitalGoodToken> GetDigitalGoodFromToken(string NFTServiceUserID, long TokenNumber)
        {
            return await serviceAgent.GetDigitalGoodfromToken(NFTServiceUserID, TokenNumber);
        }
    }
}