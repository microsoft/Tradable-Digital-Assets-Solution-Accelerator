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
    /// Response from the nameNFMBRG operation
    /// </summary>
    public partial class NameNFMBRGResponse
    {
        /// <summary>
        /// Initializes a new instance of the NameNFMBRGResponse class.
        /// </summary>
        public NameNFMBRGResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the NameNFMBRGResponse class.
        /// </summary>
        public NameNFMBRGResponse(string output = default(string))
        {
            Output = output;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "output")]
        public string Output { get; set; }

    }
}