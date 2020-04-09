

using Contoso.DigitalGoods.OffChain;
using Contoso.DigitalGoods.TokenAPI.Proxy;
using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.ContosoProfile.Service
{
    public class ContosoProfileManager : MongoEntntyCollectionBase<Models.ContosoProfile, Guid>, IContosoProfileManager
    {

        private string tokenServiceURL;

        public ContosoProfileManager(string DataConnectionString, string CollectionName, string TokenServiceURL) : base(DataConnectionString, CollectionName)
        {
            tokenServiceURL = TokenServiceURL;
        }

        /// <summary>
        /// Pass Contoso Identifier then provisioning process will be happend.
        /// 1. Register Contoso User to ABT Token Service
        /// 2. Provisioning User's Digital Locker
        /// 3. Update User Profile Service with Contoso Identifier and ABT UserID together.
        /// </summary>
        /// <param name="ContosoID"></param>
        /// <returns></returns>
        public async Task<Models.ContosoProfile> ProvisionContosoProfile(string ContosoID)
        {

            try
            {
                if (CheckForContosoDuplicates(ContosoID) == false)
                    throw new Exception("Contoso Profile already exists. Can't provision user's Contoso Profile");


                Client _proxy = new Client(tokenServiceURL, new System.Net.Http.HttpClient());

                //SHOULD Invoke TokenAPI Provision then get ABT User ID first.
                string ABTUserID = await _proxy.Users2Async(ContosoID);

                var newContosoProfile = new Models.ContosoProfile() { ContosoID = ContosoID, ABTUserID = ABTUserID };
                var provisionedContosoProfile = await this.ObjectCollection.SaveAsync(newContosoProfile);

                return provisionedContosoProfile;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get ABT UserID by Contoso Identifier
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
