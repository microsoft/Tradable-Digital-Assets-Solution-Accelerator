using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;
using System.Collections.Generic;

namespace Contoso.DigitalGoods.DigitalLocker.Service.Models
{
    public class DigitalLocker : IEntityModel<Guid>
    {
        public DigitalLocker()
        {
            this.Id = Guid.NewGuid();
            this.DigitalGoods = new List<Asset>();
        }

        public Guid Id { get; set; }
        public string UserID { get; set; }
        public List<Asset> DigitalGoods { get; set; }
    }
}
