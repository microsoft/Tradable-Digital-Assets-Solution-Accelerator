using Contoso.DigitalGoods.OffChain;
using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using Contoso.DigitalGoods.TokenService.TokenTransactionIndexer.Models;
using System;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService.TransactionIndexer
{
    public class TransactionIndexer : MongoEntntyCollectionBase<TokenTransaction, Guid>
    {
        public TransactionIndexer(string DataConnectionString, string CollectionName) : base(DataConnectionString, CollectionName)
        {
        }

        /// <summary>
        /// Adding TokenTransaction info to TokenTransactions Repo
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> AddTokenTransactionInformation(TokenTransaction transaction)
        {
            try
            {
                var updatedTransaction = await this.ObjectCollection.SaveAsync(transaction);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public TokenTransaction GetTokenMeta(string TokenId)
        {
            return this.ObjectCollection.Find(new GenericSpecification<TokenTransaction>(x => x.Type == TransactionType.TokenCreation
                                                                                    && x.TokenInstanceId == TokenId));

        }
    }
}

