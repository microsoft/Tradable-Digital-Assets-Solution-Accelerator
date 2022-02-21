using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;

namespace Contoso.DigitalGoods.TokenService.TokenNumberManager.Models
{
    public class TokenNumberManager : IEntityModel<Guid>
    {
        public TokenNumberManager()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string TokenID { get; set; }
        public long SequenceNo { get; set; }
    }
}
