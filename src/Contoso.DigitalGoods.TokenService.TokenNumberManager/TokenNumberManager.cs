using Contoso.DigitalGoods.OffChain;
using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;

namespace Contoso.DigitalGoods.TokenService.TokenNumberManager
{
    public class TokenNumberManager : MongoEntntyCollectionBase<Models.TokenNumberManager, Guid>
    {
        public TokenNumberManager(string DataConnectionString, string CollectionName) : base(DataConnectionString, CollectionName)
        {
        }


        /// <summary>
        /// Token Numbering for Every CryptoGoods
        /// Token Number have to be System.Int64 type (by Design)
        /// that means we may generate new number up to 9223372036854775807
        /// 
        /// </summary>
        /// <param name="TokenID"></param>
        /// <returns></returns>
        public long GetNextNumber(string TokenID)
        {
            var tokenManager = this.ObjectCollection.Find(new GenericSpecification<Models.TokenNumberManager>(x => x.TokenID == TokenID));

            if (tokenManager == null)
            {
                this.ObjectCollection.Save(new Models.TokenNumberManager()
                {
                    TokenID = TokenID,
                    SequenceNo = 0
                });
                return 0;
            }

            ++tokenManager.SequenceNo;
            this.ObjectCollection.Save(tokenManager);
            return tokenManager.SequenceNo;
        }
    }
}
