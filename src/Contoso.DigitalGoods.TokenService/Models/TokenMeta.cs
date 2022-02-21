using System;

namespace Contoso.DigitalGoods.TokenService.Models
{
    public class TokenMeta
    {
        public string TokenTemplateID { get; set; }
        public string TokenID { get; set; }
        public string TokenName { get; set; }
        public string TokenSymbol { get; set; }
        public string TokenCreator { get; set; }
        public DateTime TokenCreatedDate { get; set; }
    }
}
