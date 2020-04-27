using Contoso.DigitalGoods.Application.API.Models;
using Contoso.DigitalGoods.Application.Models;
using Contoso.DigitalGoods.DigitalLocker.App;
using Contoso.DigitalGoods.DigitalLocker.App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.Application.API.Controllers
{
    //[Authorize]
    [Route("DigitalGoodsApp/[controller]")]
    [ApiController]
    public class DigitalLockerController : ControllerBase
    {
        private IDigitalLocker cryptoGoodLocker;

        public DigitalLockerController(IHttpContextAccessor httpContextAccessor, IDigitalLocker DigitalLocker)
        {
            cryptoGoodLocker = DigitalLocker;

            //get ContosoUserIdentifier from JwtToken
            //var ContosoUserIdentifier = JWTUtil.GetContosoIDFromJWT(httpContextAccessor);
        }

        [HttpGet]
        [Route("{ABTUserID}")]
        public async Task<IObjectCollectionMessage<DigitalLockerItem>> GetAllCryptoGoodsFromLocker(string ABTUserID)
        {
            var result = await cryptoGoodLocker.GetAllUserDigitalLockerItems(ABTUserID);
            //return result;

            if (result != null)
            {
                return new AssetInfos()
                {
                    Success = true,
                    Data = result
                };
            }
            else
            {
                return new AssetInfos()
                {
                    Success = false,
                    Data = null,
                    Message = $"The user ({ABTUserID}) has no Digital Locker"
                };
            }
        }

        [HttpGet]
        [Route("{ABTUserID}/Tokens/{TokenNumber}")]
        public async Task<IMessage<DigitalLockerItem>> GetCryptoGoodFromLockerByTokenNumber(string ABTUserID, long TokenNumber)
        {
            var result = await cryptoGoodLocker.GetCryptoGoodFromDigitalLockerByTokenNumber(ABTUserID, TokenNumber);
            //return result;
            if (result != null)
            {
                return new AssetInfo()
                {
                    Success = true,
                    Data = result
                };
            }
            else
            {
                return new AssetInfo()
                {
                    Success = false,
                    Data = null,
                    Message = $"The user ({ABTUserID}) doesn't have The Token number : {TokenNumber}"
                };
            }
        }
    }
}
