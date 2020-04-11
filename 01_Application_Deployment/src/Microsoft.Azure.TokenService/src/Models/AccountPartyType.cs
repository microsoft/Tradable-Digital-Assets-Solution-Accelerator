// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.TokenService.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Represents a Party, Account, or LedgerNativeIdentifier
    /// </summary>
    public partial class AccountPartyType
    {
        /// <summary>
        /// Initializes a new instance of the AccountPartyType class.
        /// </summary>
        public AccountPartyType()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AccountPartyType class.
        /// </summary>
        /// <param name="descriptor">Possible values include: 'Account',
        /// 'Party', 'LedgerNativeIdentifier'</param>
        /// <param name="value">First, set the `Descriptor` field to Account,
        /// Party, or LedgerNativeIdentifier. Then put the corresponding value
        /// in this field.</param>
        public AccountPartyType(string descriptor = default(string), string value = default(string))
        {
            Descriptor = descriptor;
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'Account', 'Party',
        /// 'LedgerNativeIdentifier'
        /// </summary>
        [JsonProperty(PropertyName = "Descriptor")]
        public string Descriptor { get; set; }

        /// <summary>
        /// Gets or sets first, set the `Descriptor` field to Account, Party,
        /// or LedgerNativeIdentifier. Then put the corresponding value in this
        /// field.
        /// </summary>
        [JsonProperty(PropertyName = "Value")]
        public string Value { get; set; }

    }
}