using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Solutions.NFT;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
   
    public class AccountServiceWrapper 
    {
        private string PartyId;
        private string BlockchainNetworkId;
        private ServiceClient userServiceAPI;

        public AccountServiceWrapper(HttpClient HttpClient,IConfiguration Configuration)
        {
            this.PartyId = Configuration["Values:PartyID"];
            this.BlockchainNetworkId = Configuration["Values:BlockchainNetworkID"];

            this.userServiceAPI = new ServiceClient(Configuration["Values:NFTServiceEndpoint"], HttpClient);
        }

        /// <summary>
        /// PartyID and BlockchainNetworkId value should move in configuration
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <returns></returns>
        public async Task<User> RegisterAccount(string ContosoUserIdentifier)
        {
            var userInfo = new UserInfo()
            {
                Name = ContosoUserIdentifier,
                BlockchainNetworkID = Guid.Parse(BlockchainNetworkId),
                PartyID = Guid.Parse(PartyId),
                Description = $"Registered user with Contoso User ID : {ContosoUserIdentifier}"
            };

            return await userServiceAPI.RegisterUserAsync(userInfo);
        }

        public async Task<User> GetAccountAsync(string NFTServiceUserID)
        {
            return await userServiceAPI.GetUserByIdAsync(Guid.Parse(NFTServiceUserID));
        }

        public async Task<IEnumerable<User>> GetAllAccountAsync()
        {
            return await userServiceAPI.GetAllUsersAsync();
        }

        public async Task DeleteUser(string NFTServiceUserID)
        { 
             await userServiceAPI.UnRegisterUserAsync(Guid.Parse(NFTServiceUserID));
        }
    }
}
