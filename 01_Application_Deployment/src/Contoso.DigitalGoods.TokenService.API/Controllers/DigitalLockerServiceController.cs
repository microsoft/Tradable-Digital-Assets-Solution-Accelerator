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
        /// <param name="ABTUserID">ABT UserID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Users/{ABTUserID}")]
        public List<Asset> GetUserDigitalKicsFromDigitalLocker(string ABTUserID)
        {
            return serviceAgent.GetUserDigitalLocker(ABTUserID);
        }

        /// <summary>
        /// Get CryptoGood by TokenNumber
        /// </summary>
        /// <param name="ABTUserID">ABT User ID</param>
        /// <param name="TokenNumber">Token Number</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Users/{ABTUserID}/Tokens/{TokenNumber}")]
        public Asset GetDigitalGoodFromDigitalLocker(string ABTUserID, long TokenNumber)
        {
            return serviceAgent.GetDigitalGoodFromDigitalLocker(ABTUserID, TokenNumber);
        }


    }
}