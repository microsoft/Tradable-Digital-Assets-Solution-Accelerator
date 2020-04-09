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
    /// Response from the Constructor operation
    /// </summary>
    public partial class AsyncResponse
    {
        /// <summary>
        /// Initializes a new instance of the AsyncResponse class.
        /// </summary>
        public AsyncResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AsyncResponse class.
        /// </summary>
        public AsyncResponse(string status = default(string), string id = default(string), string reason = default(string))
        {
            Status = status;
            Id = id;
            Reason = reason;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

    }
}
