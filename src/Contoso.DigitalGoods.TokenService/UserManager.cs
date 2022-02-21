using Contoso.DigitalGoods.DigitalLocker.Service;
using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.TokenService.Models;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Solutions.CosmosDB.Security.ManagedIdentity;
using Microsoft.Solutions.NFT;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService
{
    public class UserManager
    {
        private DigitalLockerManager digitalLockerManager;
        private AccountServiceWrapper accountServiceAPI;

        public UserManager()
        {
            throw new MongoConfigurationException("Should Pass Connectionstring with constructor parameter");
        }

        public UserManager(DigitalLockerManager DigitalLockerManager, AccountServiceWrapper AccountServiceWrapper)
        {
            digitalLockerManager = DigitalLockerManager;
            accountServiceAPI = AccountServiceWrapper;
        }

        /// <summary>
        /// Register ContosoUser to NFTService user registry
        /// Provisioning User to Digital locker with NFTService user account
        /// </summary>
        /// <param name="ContosoUserIdentifier"></param>
        /// <returns>NFTService User Account ID</returns>
        /// 
        public async Task<string> ProvisionUser(string ContosoUserIdentifier)
        {
            //register Contoso User to NFTService user registry
            var NFTServiceUserAccount = await accountServiceAPI.RegisterAccount(ContosoUserIdentifier);

            //provision User's digital Locker with NFTService User Account ID
            //var digitalLockerManager = new DigitalLockerManager(dbConnectionString);
            await digitalLockerManager.ProvisionLocker(NFTServiceUserAccount.Id.ToString());
            return NFTServiceUserAccount.Id.ToString();
        }

        public async Task<User> GetUserInfo(string NFTServiceUserID)
        {
            return await accountServiceAPI.GetAccountAsync(NFTServiceUserID);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await accountServiceAPI.GetAllAccountAsync();
        }

        public async Task DeleteUser(string NFTServiceUserID)
        {
            await accountServiceAPI.DeleteUser(NFTServiceUserID);
        }

        public async Task<bool> PutCryptoGoodToLocker(string NFTServiceUserID, Asset CryptoGood)
        {
            //Add CryptoGoods to User Locker
            return await digitalLockerManager.AddCryptoKics(NFTServiceUserID, CryptoGood);
        }

        public List<Asset> GetAllUserCryptoGoods(string NFTServiceUserID)
        {
            return digitalLockerManager.GetAllUserCryptoGoods(NFTServiceUserID);
        }

        public async Task<bool> RemoveCryptoGood(string NFTServiceUserID, long TokenNumber)
        {
            return await digitalLockerManager.RemoveCryptoGoods(NFTServiceUserID, TokenNumber);
        }

        public async Task<bool> AddCryptoGoods(string NFTServiceUserID, DigitalGoodToken cryptoGoodToken)
        {
            return await digitalLockerManager.AddCryptoKics(NFTServiceUserID,
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

        public Asset GetCryptoGoodFromUserLocker(string NFTServiceUserID, long TokenID)
        {
            return digitalLockerManager.GetUserCryptoGood(NFTServiceUserID, TokenID);
        }
    }
}
