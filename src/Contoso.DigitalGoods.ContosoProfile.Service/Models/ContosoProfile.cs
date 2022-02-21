using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;

namespace Contoso.DigitalGoods.ContosoProfile.Service.Models
{
    public class ContosoProfile : IEntityModel<Guid>
    {
        public ContosoProfile()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string ContosoID { get; set; }
        public string NFTServiceUserID { get; set; }
        public string PublicAddress { get; set; }

    }
}
