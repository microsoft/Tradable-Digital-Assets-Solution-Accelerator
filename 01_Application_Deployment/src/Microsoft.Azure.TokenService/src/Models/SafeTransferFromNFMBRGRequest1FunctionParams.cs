// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.TokenService.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class SafeTransferFromNFMBRGRequest1FunctionParams
    {
        /// <summary>
        /// Initializes a new instance of the
        /// SafeTransferFromNFMBRGRequest1FunctionParams class.
        /// </summary>
        public SafeTransferFromNFMBRGRequest1FunctionParams()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// SafeTransferFromNFMBRGRequest1FunctionParams class.
        /// </summary>
        public SafeTransferFromNFMBRGRequest1FunctionParams(AccountPartyType fromProperty = default(AccountPartyType), AccountPartyType to = default(AccountPartyType), long? tokenId = default(long?), string data = default(string))
        {
            FromProperty = fromProperty;
            To = to;
            TokenId = tokenId;
            Data = data;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public AccountPartyType FromProperty { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public AccountPartyType To { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "tokenId")]
        public long? TokenId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

    }
}