using Contoso.DigitalGoods.ContosoProfile.Service.Models;
using Contoso.DigitalGoods.OffChain;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Contoso.DigitalGoods;
using Microsoft.Solutions.CosmosDB.Security.ManagedIdentity;

namespace Contoso.DigitalGoods.ContosoProfile.Service
{
    public class ContosoProfileManager : MongoEntntyCollectionBase<Models.ContosoProfile, Guid>, IContosoProfileManager
    {

        private string tokenServiceURL;
        private HttpClient httpClient;

        public ContosoProfileManager(string DataConnectionString, string CollectionName, string TokenServiceURL) : base(DataConnectionString, CollectionName)
        {
            tokenServiceURL = TokenServiceURL;
        }

        public ContosoProfileManager(IConfiguration Configuration, HttpClient HttpClient, CosmosConnectionStrings connectionStrings) : base(connectionStrings.PrimaryReadWriteKey, Configuration["Values:profileCollectionName"])
        {
            tokenServiceURL = Configuration["Values:tokenAPIURL"];
            //partyID = Configuration["Values:PartyID"];
            //blockchainNetworkID = Configuration["Values:BlockchainNetworkID"];

            httpClient = HttpClient;
        }

        /// <summary>
        /// Pass Contoso Identifier then provisioning process will be happend.
        /// 1. Register Contoso User to NFTService Token Service
        /// 2. Provisioning User's Digital Locker
        /// 3. Update User Profile Service with Contoso Identifier and NFTServiceUserID together.
        /// </summary>
        /// <param name="ContosoID"></param>
        /// <returns></returns>
        public async Task<Models.ContosoProfile> ProvisionContosoProfile(string ContosoID)
        {

            try
            {
                var profile = GetUserProfileByContosoID(ContosoID);
                if (profile != null) return profile;

                TokenAPI.Proxy.Client accountService = new (tokenServiceURL, httpClient);
                
                //SHOULD Invoke TokenAPI Provision then get NFTService User ID first.
                var NFTServiceAccount = await accountService.ProvisioningUserAsync(ContosoID);
                var user = await accountService.GetUserInfoAsync(NFTServiceAccount);

                var newContosoProfile = new Models.ContosoProfile() { ContosoID = ContosoID, 
                                                                      NFTServiceUserID = NFTServiceAccount,
                                                                      Id = Guid.NewGuid(), 
                                                                      PublicAddress = user.PublicAddress };

                var provisionedContosoProfile = await this.ObjectCollection.SaveAsync(newContosoProfile);

                return provisionedContosoProfile;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get NFTServiceUserID by Contoso Identifier
        /// </summary>
        /// <param name="ContosoID"></param>
        /// <returns></returns>
        public Models.ContosoProfile GetUserProfileByContosoID(string ContosoID)
        {
            return this.ObjectCollection.Find(new GenericSpecification<Models.ContosoProfile>(x => x.ContosoID == ContosoID));
        }

        public bool CheckForContosoDuplicates(string ContosoID)
        {
            try
            {
                if (ObjectCollection.Find(new GenericSpecification<Models.ContosoProfile>(x => x.ContosoID == ContosoID)) != null)
                    throw new Exception("Contoso Profile already exists. Can't provision user's Contoso Profile");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteProfile(string ContosoId)
        {
            var profile = this.ObjectCollection.Find(new GenericSpecification<Models.ContosoProfile>(x => x.ContosoID == ContosoId));
            ObjectCollection.Delete(profile);
            return true;
        }

        public async Task<Models.ContosoProfile> AddPrfileAsync(Models.ContosoProfile profile)
        {
            return await this.ObjectCollection.SaveAsync(profile);
        }
    }
}
