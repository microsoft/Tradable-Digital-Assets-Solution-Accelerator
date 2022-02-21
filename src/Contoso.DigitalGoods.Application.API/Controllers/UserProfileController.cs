using Contoso.DigitalGoods.Application.API.Models;
using Contoso.DigitalGoods.Application.Models;
using Contoso.DigitalGoods.ContosoProfile.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Model = Contoso.DigitalGoods.ContosoProfile.Service.Models;


namespace Contoso.DigitalGoods.Application.API.Controllers
{
    /// <summary>
    /// User Profile Service API
    /// </summary>
    //[Authorize]
    [Route("DigitalGoodsApp/User")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private IContosoProfileManager profileManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="ProfileManager"></param>
        public UserProfileController(IHttpContextAccessor httpContextAccessor, IContosoProfileManager ProfileManager)
        {
            profileManager = ProfileManager;

            //get ContosoUserIdentifier from JwtToken
            //var ContosoUserIdentifier = JWTUtil.GetContosoIDFromJWT(httpContextAccessor);

        }

        /// <summary>
        /// Provisioning User's Digital Locker and Registering NFTService User ID
        /// </summary>
        /// <param name="ContosoIdentifier">Contoso User Identifier which can be get through Contoso Authentication</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{ContosoIdentifier}")]
        public async Task<IMessage<Model.ContosoProfile>> ProvisionUser(string ContosoIdentifier)
        {
            var result = await profileManager.ProvisionContosoProfile(ContosoIdentifier);

            if (result != null)
            {
                var profileInfo = new ProfileInfo()
                {
                    Success = true,
                    Data = result
                };

                return profileInfo;

            }
            else
            {
                return new ProfileInfo()
                {
                    Success = false,
                    Data = null,
                    Message = "Error provisioning new User"
                };
            }
        }


        /// <summary>
        /// Return NFTServiceUserID from ContosoIdentifier
        /// </summary>
        /// <param name="ContosoIdentifier"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{ContosoIdentifier}")]
        public IMessage<Model.ContosoProfile> GetNFTServiceUserID(string ContosoIdentifier)
        {
            var result = profileManager.GetUserProfileByContosoID(ContosoIdentifier);

            if (result != null)
            {
                var profileInfo = new ProfileInfo()
                {
                    Success = true,
                    Data = result
                };

                return profileInfo;
            }
            else
            {
                return new ProfileInfo()
                {
                    Success = false,
                    Data = null,
                    Message = $"The Contoso User - {ContosoIdentifier} is not yet registered"
                };
            }
        }
    }
}
