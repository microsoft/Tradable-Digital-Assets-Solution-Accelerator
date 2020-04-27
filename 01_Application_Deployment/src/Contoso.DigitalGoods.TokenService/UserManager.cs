using Contoso.DigitalGoods.DigitalLocker.Service;
using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.TokenService.Models;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Microsoft.Azure.TokenService.Proxy;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService
{
    public class UserManager
    {
        private string dbConnectionString;
        private DigitalLockerManager digitalLockerManager;
        private AccountServiceWrapper accountServiceAPI;

        public UserManager()
        {
            throw new MongoConfigurationException("Should Pass Connectionstring with constructor parameter");
        }

        public UserManager(string DBConnectionString, string ServiceEndpoint, string PartyId, string BlockchainNetworkId)
        {
            //InitializeConnections(DBConnectionString);
            dbConnectionString = DBConnectionString;
            digitalLockerManager = new DigitalLockerManager(dbConnectionString, "DigitalLockers");
            accountServiceAPI = new AccountServiceWrapper(ServiceEndpoint, PartyId, BlockchainNetworkId);
        }

        /// <summary>
        /// Register ContosoUser to ABT user registry
        /// Provisioning User to Digital locker with ABT user account
        /// </summary>
        /// <param name="ContosoUserIdentifier"></param>
        /// <returns>ABT User Account ID</returns>
        /// 
        public async Task<string> ProvisionUser(string ContosoUserIdentifier)
        {
            //register Contoso User to ABT user registry
            var abtUserAccount = await accountServiceAPI.RegisterAccount(ContosoUserIdentifier);

            //provision User's digital Locker with ABT User Account ID
            //var digitalLockerManager = new DigitalLockerManager(dbConnectionString);
            await digitalLockerManager.ProvisionLocker(abtUserAccount.Id.ToString());
            return abtUserAccount.Id.ToString();
        }

        public async Task<User> GetUserInfo(string ABTUserID)
        {
            return await accountServiceAPI.GetAccountAsync(ABTUserID);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await accountServiceAPI.GetAllAccountAsync();
        }

        public async Task DeleteUser(string ABTUserID)
        {
            await accountServiceAPI.DeleteUser(ABTUserID);
        }

        public async Task<bool> PutCryptoGoodToLocker(string ABTUserID, Asset CryptoGood)
        {
            //Add CryptoGoods to User Locker
            return await digitalLockerManager.AddCryptoKics(ABTUserID, CryptoGood);
        }

        public List<Asset> GetAllUserCryptoGoods(string ABTUserID)
        {
            return digitalLockerManager.GetAllUserCryptoGoods(ABTUserID);
        }

        public async Task<bool> RemoveCryptoGood(string ABTUserID, long TokenNumber)
        {
            return await digitalLockerManager.RemoveCryptoGoods(ABTUserID, TokenNumber);
        }

        public async Task<bool> AddCryptoGoods(string ABTUserID, DigitalKickToken cryptoGoodToken)
        {
            return await digitalLockerManager.AddCryptoKics(ABTUserID,
                new DigitalLocker.Service.Models.Asset()
                {
                    ProductId = cryptoGoodToken.ProductId,
                    Name = cryptoGoodToken.ProductName,
                    TokenNumber = Convert.ToInt64(cryptoGoodToken.TokenNumber, 16)
                });
        }

        public async Task<bool> TransferAsset(string SenderID, string RecipientID, long TokenNumber)
        {
            return await digitalLockerManager.TransferAsset(SenderID, RecipientID, TokenNumber);
        }

        public Asset GetCryptoGoodFromUserLocker(string ABTUserID, long TokenID)
        {
            return digitalLockerManager.GetUserCryptoGood(ABTUserID, TokenID);
        }
    }
}
