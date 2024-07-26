using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// The class representing an Owned Object Reference
    /// </summary>
    [JsonObject]
    public class SuiOwnedObjectRef
    {
        /// <summary>
        /// An `Owner` object representing the owner of the objec
        /// </summary>
        [JsonProperty("owner")]
        public Owner Owner { get; set; }

        /// <summary>
        /// A `SuiObjectRef` type representing a reference to the Sui Object.
        /// </summary>
        [JsonProperty("reference")]
        public Sui.Types.SuiObjectRef Reference { get; set; }
    }
}