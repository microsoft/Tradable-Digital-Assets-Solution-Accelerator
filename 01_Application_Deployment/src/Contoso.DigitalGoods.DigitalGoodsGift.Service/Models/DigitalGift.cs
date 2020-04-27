using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using System;

namespace Contoso.DigitalGoods.CryptoGoodsGift.Service.Models
{
    public class DigitalGift : IEntityModel<Guid>
    {
        public DigitalGift()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string GiftId { get; set; }
        //public string GiftUrl { get; set; }
        public long TokenId { get; set; }

        //Gift Sender should be Contoso. Doesn't need to assign
        //public string SenderId { get; set; }

        //Contoso's Identifier
        public string ReciverId { get; set; }
        public string AcceptedIds { get; set; }
        public bool RequiresApproval { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateClosed { get; set; }

    }
}
