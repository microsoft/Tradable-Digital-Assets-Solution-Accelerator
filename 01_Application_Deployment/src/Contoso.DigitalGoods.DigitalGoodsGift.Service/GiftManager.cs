using Contoso.DigitalGoods.ContosoProfile.Service;
using Contoso.DigitalGoods.DigitalGoodsGift.Service.Interfaces;
using Contoso.DigitalGoods.DigitalGoodsGift.Service.Models;
using Contoso.DigitalGoods.OffChain;
using Contoso.DigitalGoods.TokenAPI.Proxy;
using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.DigitalGoodsGift.Service
{
    public class GiftManager : MongoEntntyCollectionBase<CryptoGift, Guid>, IGiftManager
    {
        private string tokenServiceURL;
        private string ContosoABTUserId; //we don't need to have it.
        private ContosoProfileManager profileManager;
        //private string giftUrl = "https://CryptoGoods.com/gift/"; //should be in configuration file.

        public GiftManager(string DataConnectionString, string CollectionName, string ContosoID, string TokenServiceURL) : base(DataConnectionString, CollectionName)
        {
            tokenServiceURL = TokenServiceURL;
            ContosoABTUserId = ContosoID;
            profileManager = new ContosoProfileManager(DataConnectionString, "UserProfile", TokenServiceURL);
        }


        public async Task<CryptoGift> CreateCryptoGoodGift(string ReciverContosoUserID, long TokenId)
        {
            //Check not to creating duplicated Gift.
            var gift =
                ObjectCollection.Find(new GenericSpecification<CryptoGift>(x => x.ReciverId == ReciverContosoUserID && x.TokenId == TokenId));

            //Once we have already created, just return null
            if (gift != null) return null;

            var newCryptoGift = new CryptoGift()
            {
                GiftId = Guid.NewGuid().ToString(),
                TokenId = TokenId,
                ReciverId = ReciverContosoUserID,
                RequiresApproval = false,
                Status = "active",
                DateCreated = DateTime.UtcNow
            };

            var createCrptoGift = await ObjectCollection.SaveAsync(newCryptoGift);
            return createCrptoGift;
        }

        public CryptoGift GetCryptoGoodGift(string GiftId)
        {
            var giftInfo = ObjectCollection.Find(new GenericSpecification<CryptoGift>(x => x.GiftId == GiftId));

            return giftInfo;
        }

        public IEnumerable<CryptoGift> GetAllActiveCryptoGoodGifts()
        {
            var giftInfos = ObjectCollection.FindAll(new GenericSpecification<CryptoGift>(x => x.Status == "active"));
            return giftInfos;
        }

        public async Task<CryptoGift> TransferCryptoGoodGiftToken(string GiftId)
        {
            CryptoGift info = GetCryptoGoodGift(GiftId);

            if (info.Status.ToLower() == "active")
            {
                if (info.RequiresApproval == true)
                {
                    //Send notification
                    //Wait for response
                }

                var userProfile = profileManager.GetUserProfileByContosoID(info.ReciverId);

                Client _proxy = new Client(tokenServiceURL, new System.Net.Http.HttpClient());
                var result = await _proxy.GiftCryptoGoodAsync(userProfile.ABTUserID, info.TokenId);

                if (result)
                {
                    info.Status = "close";
                    info.DateClosed = DateTime.UtcNow;
                    await ObjectCollection.SaveAsync(info);
                    //Send Notification to Sender
                    //Send Nitification to Reciver
                }
            }
            return info;
        }

        public bool DeleteCryptoGift(CryptoGift GiftId)
        {
            ObjectCollection.Delete(GiftId);
            return true;
        }

    }

}
