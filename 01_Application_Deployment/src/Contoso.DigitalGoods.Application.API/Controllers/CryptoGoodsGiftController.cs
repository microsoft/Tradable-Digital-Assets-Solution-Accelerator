using Contoso.DigitalGoods.Application.API.Models;
using Contoso.DigitalGoods.Application.API.Utils;
using Contoso.DigitalGoods.Application.Models;
using Contoso.DigitalGoods.ContosoProfile.Service;
using Contoso.DigitalGoods.DigitalGoodsGift.Service.Interfaces;
using Contoso.DigitalGoods.DigitalGoodsGift.Service.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.Application.API.Controllers
{
    //[Authorize]
    [Route("CryptoGoodsApp")]
    [ApiController]
    public class CryptoGoodsGiftController : ControllerBase
    {
        private IGiftManager cryptoGiftManager;
        private IDataProtector dataProtector;
        private IConfiguration configuration;
        private IContosoProfileManager profileManager;

        public CryptoGoodsGiftController(IDataProtectionProvider dataProtectionProvider, IHttpContextAccessor httpContextAccessor, IGiftManager GiftManager, IContosoProfileManager ProfileManager, IConfiguration Configuration)
        {
            //get ContosoUserIdentifier from JwtToken
            var ContosoUserIdentifier = JWTUtil.GetContosoIDFromJWT(httpContextAccessor);

            cryptoGiftManager = GiftManager;
            dataProtector = dataProtectionProvider.CreateProtector("ContosoCryptoGoods.GiftInfo");
            configuration = Configuration;
            profileManager = ProfileManager;
        }


        [HttpPost]
        [Route("Management/Gifts/GenerateURL")]
        public string GeneratateGiftURL(string GiftID)
        {
            //check ContosoIdentifier and GiftID.
            var ContosoIdentifier = cryptoGiftManager.GetCryptoGoodGift(GiftID).ReciverId;

            //adding ContosoIdentifier and GiftID with encrypted mannder
            var propoertyBag = new { ContosoID = ContosoIdentifier, GiftID = GiftID };

            string encyrptedGiftProperties = dataProtector.Protect(JsonConvert.SerializeObject(propoertyBag));

            return configuration["Values:GiftURL"] + encyrptedGiftProperties;
        }

        /// <summary>
        /// Get Encrypted URL and resolve information then Transfer token to User
        /// </summary>
        /// <param name="encryptedGiftID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Gifts/{encryptedGiftID}")]
        public async Task<IMessage<CryptoGift>> ResolveURLAndTransferGiftToken(string encryptedGiftID)
        {
            dynamic propertyBag;

            try
            {
                //Parse URL
                propertyBag = JsonConvert.DeserializeObject(dataProtector.Unprotect(encryptedGiftID));

                //Check Properties in the propertyBag
                var giftID = propertyBag.GiftID.Value;
                var ContosoID = propertyBag.ContosoID.Value;

                //Check Contoso User is already provisioned user
                var userProfile = profileManager.GetUserProfileByContosoID(ContosoID);
                if (userProfile == null)
                {
                    return new GiftInfo()
                    {
                        Success = false,
                        Data = null,
                        Message = "Without provisioning, user cannot get CryptoGood"
                    };
                }

                //Check Recipient from Gift storage
                CryptoGift cryptoGift = cryptoGiftManager.GetCryptoGoodGift(giftID);
                if (cryptoGift.Status == "close")
                {
                    return new GiftInfo()
                    {
                        Success = false,
                        Data = null,
                        Message = "this URL seems to be used already. URL can be used at once."
                    };
                }



                //check User's Contoso's Identifier with Gift Information in storage
                if (ContosoID != cryptoGift.ReciverId)
                {
                    return new GiftInfo()
                    {
                        Success = false,
                        Data = null,
                        Message = "Something wrong to finding your gift. probably you must not be the Reciever"
                    };
                }
                else
                {
                    //checked and it's time to transfer the gift
                    CryptoGift result = await cryptoGiftManager.TransferCryptoGoodGiftToken(giftID);

                    if (result.Status == "close")
                    {
                        return new GiftInfo()
                        {
                            Success = true,
                            Data = result
                        };
                    }
                    else
                    {
                        return new GiftInfo()
                        {
                            Success = false,
                            Data = null,
                            Message = "Something wrong to transfering token"
                        };
                    }
                }
            }
            catch (Exception)
            {
                return new GiftInfo()
                {
                    Success = false,
                    Data = null,
                    Message = "this is not correct URL"
                };
            }
        }


        [HttpPost]
        [Route("Management/Gifts")]
        public async Task<IMessage<CryptoGift>> CreateCryptoGift(string ContosoUserID, long TokenID)
        {
            //var idCheck = profileManager.GetABTUserID(ReciverContosoUserID); 
            var result = await cryptoGiftManager.CreateCryptoGoodGift(ContosoUserID, TokenID);

            if (result != null)
            {
                var giftInfo = new GiftInfo()
                {
                    Success = true,
                    Data = result
                };

                return giftInfo;
            }
            else
            {
                return new GiftInfo()
                {
                    Success = false,
                    Data = null,
                    Message = $"You've already created Gift for {ContosoUserID} with Token {TokenID}"
                };
            }
        }

        //[HttpPost]
        //[Route("Gifts/Transfer")]
        //public async Task<IMessage<CryptoGift>> TransferCryptoGoodGiftToken(string GiftId)
        //{
        //    var result = await cryptoGiftManager.TransferCryptoGoodGiftToken(GiftId);

        //    if (result != null)
        //    {
        //        var giftInfo = new GiftInfo()
        //        {
        //            Success = true,
        //            Data = result
        //        };

        //        return giftInfo;

        //    }
        //    else
        //    {
        //        return new GiftInfo()
        //        {
        //            Success = false,
        //            Data = null,
        //            Message = "Error during Transferring CryptoGood"
        //        };
        //    }
        //}



        [HttpGet]
        [Route("Management/Gifts/{GiftId}")]
        public IMessage<CryptoGift> GetCryptoGift(string GiftId)
        {
            var result = cryptoGiftManager.GetCryptoGoodGift(GiftId);

            if (result != null)
            {
                var giftInfo = new GiftInfo()
                {
                    Success = true,
                    Data = result
                };

                return giftInfo;

            }
            else
            {
                return new GiftInfo()
                {
                    Success = false,
                    Data = null,
                    Message = "Error getting gift record"
                };
            }
        }

        [HttpGet]
        [Route("Management/Gifts")]
        public IObjectCollectionMessage<CryptoGift> GetAllCryptoGifts()
        {
            var result = cryptoGiftManager.GetAllActiveCryptoGoodGifts();


            if (result != null)
            {
                var giftInfo = new GiftInfos()
                {
                    Success = true,
                    Data = result.ToArray()
                };

                return giftInfo;

            }
            else
            {
                return new GiftInfos()
                {
                    Success = false,
                    Data = null,
                    Message = "Error getting gift record"
                };
            }

        }
    }
}

