namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
    public class TransferTokenRequest
    {
        public string TokenID { get; set; }
        public string TokenSequence { get; set; }
        public string CallerID { get; set; }
        public string SenderID { get; set; }
        public string RecipientID { get; set; }
    }
}
