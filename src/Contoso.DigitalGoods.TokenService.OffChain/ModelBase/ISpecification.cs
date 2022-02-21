using System;
using System.Linq.Expressions;

namespace Contoso.DigitalGoods.TokenService.OffChain.ModelBase
{
    public interface ISpecification<TEntity>
    {
        /// <summary>
        /// Gets or sets the func delegate query to execute against the repository for searching records.
        /// </summary>
        Expression<Func<TEntity, bool>> Predicate { get; }
    }
}
