using Contoso.DigitalGoods.TokenService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.API.Controllers
{

    /// <summary>
    /// Contoso DigitalGoods Token API Service
    /// </summary>
    //[Authorize]
    [Route("DigitalGood/[controller]")]
    [ApiController]
    public class TokenServiceController : ControllerBase
    {
        private ITokenServiceAgent serviceAgent;
        //private string ContosoUserIdentifier;

        public TokenServiceController(IHttpContextAccessor httpContextAccessor, ITokenServiceAgent ServiceAgent, IConfiguration configuration)
        {
            serviceAgent = ServiceAgent;

            //get ContosoUserIdentifier from JwtToken
            //ContosoUserIdentifier = JwtUtil.GetContosoIDFromJWT(httpContextAccessor);           
        }


        /// <summary>
        /// Gift CryptoGood to User by Contoso
        /// </summary>
        /// <param name="Recipient">CryptoGood User</param>
        /// <param name="TokenNumber">Token Number</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GiftDigitalGood")]
        public async Task<bool> GiftDigitalGoods(string Recipient, long TokenNumber)
        {
            return await serviceAgent.GiftDigitalGoods(Recipient, TokenNumber);
        }

        /// <summary>
        /// Transfer CryptoGood to User by Contoso
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="Recipient">Recipient</param>
        /// <param name="TokenNumber">Token Number</param>
        /// <returns></returns>
        [HttpPost]
        [Route("TransferDigitalGood")]
        public async Task<bool> TransferDigitalGood(string Sender, string Recipient, long TokenNumber)
        {
            return await serviceAgent.TransferDigitalGoods(Sender, Recipient, TokenNumber);
        }

        /// <summary>
        /// Check the token ownership
        /// </summary>
        /// <param name="TokenNumber">Minted Token Number</param>
        /// <param name="CallerID">Caller NFTServiceID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("IsitMyToken")]
        public async Task<bool> IsitMyToken(long? TokenNumber, string CallerID)
        {
            return await serviceAgent.IsitMyToken(TokenNumber, CallerID);
        }
    }
}