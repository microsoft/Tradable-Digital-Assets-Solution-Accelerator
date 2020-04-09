// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.TokenService.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class IsApprovedForAllNFMBRGRequestFunctionParams
    {
        /// <summary>
        /// Initializes a new instance of the
        /// IsApprovedForAllNFMBRGRequestFunctionParams class.
        /// </summary>
        public IsApprovedForAllNFMBRGRequestFunctionParams()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// IsApprovedForAllNFMBRGRequestFunctionParams class.
        /// </summary>
        public IsApprovedForAllNFMBRGRequestFunctionParams(AccountPartyType owner = default(AccountPartyType), AccountPartyType operatorProperty = default(AccountPartyType))
        {
            Owner = owner;
            OperatorProperty = operatorProperty;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "owner")]
        public AccountPartyType Owner { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "operator")]
        public AccountPartyType OperatorProperty { get; set; }

    }
}
