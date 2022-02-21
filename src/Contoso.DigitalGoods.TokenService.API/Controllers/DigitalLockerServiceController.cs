using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.TokenService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Contoso.DigitalGoods.TokenService.API.Controllers
{
    //[Authorize]
    [Route("DigitalGood/[controller]")]
    [ApiController]
    public class DigitalLockerServiceController : ControllerBase
    {
        private ITokenServiceAgent serviceAgent;

        public DigitalLockerServiceController(IHttpContextAccessor httpContextAccessor, ITokenServiceAgent ServiceAgent)
        {
            serviceAgent = ServiceAgent;
            //get ContosoUserIdentifier from JwtToken
            //ContosoUserIdentifier = JwtUtil.GetContosoIDFromJWT(httpContextAccessor);           
        }

        /// <summary>
        /// Get All CryptoKics from Digital Locker
        /// </summary>
        /// <param name="NFTServiceUserID">NFTServiceUserID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Users/{NFTServiceUserID}")]
        public List<Asset> GetUserDigitalKicsFromDigitalLocker(string NFTServiceUserID)
        {
            return serviceAgent.GetUserDigitalLocker(NFTServiceUserID);
        }

        /// <summary>
        /// Get CryptoGood by TokenNumber
        /// </summary>
        /// <param name="NFTServiceUserID">NFTService User ID</param>
        /// <param name="TokenNumber">Token Number</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Users/{NFTServiceUserID}/Tokens/{TokenNumber}")]
        public Asset GetDigitalGoodFromDigitalLocker(string NFTServiceUserID, long TokenNumber)
        {
            return serviceAgent.GetDigitalGoodFromDigitalLocker(NFTServiceUserID, TokenNumber);
        }


    }
}