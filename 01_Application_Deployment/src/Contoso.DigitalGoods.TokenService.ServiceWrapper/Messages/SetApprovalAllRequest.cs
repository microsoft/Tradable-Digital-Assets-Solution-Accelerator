namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
    public class SetApprovalAllRequest
    {
        public string TokenID { get; set; }
        public string CallerID { get; set; }
        public string AllowerID { get; set; }
    }
}
