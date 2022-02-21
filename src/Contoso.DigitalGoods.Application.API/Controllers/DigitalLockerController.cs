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
        [Route("{NFTServiceUserID}")]
        public async Task<IObjectCollectionMessage<DigitalLockerItem>> GetAllCryptoGoodsFromLocker(string NFTServiceUserID)
        {
            var result = await cryptoGoodLocker.GetAllUserDigitalLockerItems(NFTServiceUserID);
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
                    Message = $"The user ({NFTServiceUserID}) has no Digital Locker"
                };
            }
        }

        [HttpGet]
        [Route("{NFTServiceUserID}/Tokens/{TokenNumber}")]
        public async Task<IMessage<DigitalLockerItem>> GetCryptoGoodFromLockerByTokenNumber(string NFTServiceUserID, long TokenNumber)
        {
            var result = await cryptoGoodLocker.GetCryptoGoodFromDigitalLockerByTokenNumber(NFTServiceUserID, TokenNumber);
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
                    Message = $"The user ({NFTServiceUserID}) doesn't have The Token number : {TokenNumber}"
                };
            }
        }
    }
}
