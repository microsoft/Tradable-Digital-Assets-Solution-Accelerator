using Contoso.DigitalGoods.DigitalLocker.Service.Models;
using Contoso.DigitalGoods.OffChain;
using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using Microsoft.Solutions.CosmosDB.Security.ManagedIdentity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.DigitalLocker.Service
{
    public class DigitalLockerManager : MongoEntntyCollectionBase<Models.DigitalLocker, Guid>
    {
        public DigitalLockerManager(string DataConnectionString, string CollectionName) : base(DataConnectionString, CollectionName)
        {
        }

        public DigitalLockerManager(CosmosConnectionStrings cosmosConnectionStrings) : base(cosmosConnectionStrings.PrimaryReadWriteKey, "TradableDigitalGoods")
        {
        }


        public async Task<bool> ProvisionLocker(string UserID)
        {
            try
            {
                if (this.ObjectCollection.Find(new GenericSpecification<Models.DigitalLocker>(x => x.UserID == UserID)) != null)
                    throw new Exception("Locker already exists. Can't provision user's Locker");

                var newLocker = new Models.DigitalLocker() { UserID = UserID };
                var provisionedLocker = await this.ObjectCollection.SaveAsync(newLocker);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<bool> TransferAsset(string SenderID, string Recipient, long TokenNumber)
        {
            //Get Sender's CryptoGood
            var asset = this.GetUserCryptoGood(SenderID, TokenNumber);

            //Remove CryptoGood from Sender's Locker
            await this.RemoveCryptoGoods(SenderID, TokenNumber);

            //Put CryptoGood to Recipient's Locker
            await this.AddCryptoKics(Recipient, asset);

            return true;
        }

        public List<Asset> GetAllUserCryptoGoods(string UserID)
        {
            var userLocker = this.ObjectCollection.Find(new GenericSpecification<Models.DigitalLocker>(x => x.UserID == UserID));

            //return (userLocker != null) ? userLocker.DigitalGoods : null;
            return userLocker?.DigitalGoods ?? new List<Asset>();
        }

        public Asset GetUserCryptoGood(string UserID, long TokenNumber)
        {
            var userCryptoGood = this.ObjectCollection.Find(
                new GenericSpecification<Models.DigitalLocker>(x => x.UserID == UserID))
                .DigitalGoods.Find(x => x.TokenNumber == TokenNumber);

            return (userCryptoGood != null) ? userCryptoGood : null;

        }

        public async Task<bool> AddCryptoKics(string UserID, Asset CryptoGoods)
        {
            var userLocker = this.ObjectCollection.Find(new GenericSpecification<Models.DigitalLocker>(x => x.UserID == UserID));
            if (userLocker == null)
            {
                //throw new Exception("Not found user Locker for this user ID");
                userLocker = new Models.DigitalLocker() { UserID = UserID };
            }

            userLocker.DigitalGoods.Add(CryptoGoods);

            try
            {
                await this.ObjectCollection.SaveAsync(userLocker);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveCryptoGoods(string UserID, long TokenNumber)
        {
            var userLocker = this.ObjectCollection.Find(new GenericSpecification<Models.DigitalLocker>(x => x.UserID == UserID));
            if (userLocker == null) throw new Exception("Not found user Locker for this user ID");

            userLocker.DigitalGoods.RemoveAll(x => x.TokenNumber == TokenNumber);

            try
            {
                await this.ObjectCollection.SaveAsync(userLocker);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
