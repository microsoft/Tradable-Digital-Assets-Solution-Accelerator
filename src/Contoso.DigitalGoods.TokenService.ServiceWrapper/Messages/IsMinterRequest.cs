namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
    public class IsMinterRequest
    {
        public string TokenID { get; set; }
        public string CallerID { get; set; }
        public string TokenOwnerID { get; set; }
    }
}
