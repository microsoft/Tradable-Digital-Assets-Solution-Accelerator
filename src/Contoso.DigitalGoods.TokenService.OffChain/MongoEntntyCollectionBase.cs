using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using Contoso.DigitalGoods.TokenService.OffChain.Mongo;
using MongoDB.Driver;

namespace Contoso.DigitalGoods.OffChain
{
    public class MongoEntntyCollectionBase<TEntity, Guid> where TEntity : class, IEntityModel<Guid>
    {
        protected IRepository<TEntity, Guid> ObjectCollection;

        public MongoEntntyCollectionBase(string DataConnectionString, string CollectionName)
        {
            this.ObjectCollection =
                new BusinessTransactionRepository<TEntity, Guid>(new MongoClient(DataConnectionString),
                CollectionName);
        }
    }
}
