using Contoso.DigitalGoods.TokenService.OffChain.ModelBase;
using Newtonsoft.Json;
using System;

namespace Contoso.DigitalGoods.TokenService.TokenTransactionIndexer.Models
{
    public class TokenTransaction : IEntityModel<Guid>
    {
        public TokenTransaction()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public DateTime TransactionTime { get; set; }
        public TransactionType Type { get; set; }
        public string TokenInstanceId { get; set; }
        public string Caller { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string SerializedTokenData { get; set; }
        public TransactionReceipt TransactionInfo { get; set; }

    }

    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum TransactionType
    {
        TokenCreation,
        MintingToken,
        TransferToken,
        ApproveForAll,
        Approve,
        BurnToken
    }
}
