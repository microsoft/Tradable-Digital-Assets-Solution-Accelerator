using Microsoft.Azure.TokenService;
using Microsoft.Azure.TokenService.Proxy;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
   
    public class AccountServiceWrapper 
    {
        private string PartyId;
        private string BlockchainNetworkId;
        private UsersClient userServiceAPI;
        private ServiceManagementClient svcManagementAPI;


        public AccountServiceWrapper(string ServiceEndpoint, string PartyId, string BlockchainNetworkID)
        {
            this.PartyId = PartyId;
            this.BlockchainNetworkId = BlockchainNetworkID;

            this.userServiceAPI = new UsersClient(ServiceEndpoint);
            this.svcManagementAPI = new ServiceManagementClient(ServiceEndpoint);

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

            return await userServiceAPI.UserPostAsync(userInfo);
        }

        public async Task<User> GetAccountAsync(string ABTUserID)
        {
            return await userServiceAPI.UserPostAsync(Guid.Parse(ABTUserID));
        }

        public async Task<IEnumerable<User>> GetAllAccountAsync()
        {
            return await svcManagementAPI.UsersAsync();
        }

        public async Task DeleteUser(string ABTUserID)
        { 
             await userServiceAPI.UserDeleteAsync(Guid.Parse(ABTUserID));
        }
    }
}
