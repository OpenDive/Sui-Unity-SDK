using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    ///
    /// <code>
    /// {
    ///     "owner":{
    ///         "AddressOwner":"0x0c567ffdf8162cb6d51af74be0199443b92e823d4ba6ced24de5c6c463797d46"
    ///     },
    ///     "reference":{
    ///         "objectId":"0x7cb7bf705ad0edf9a93f993ba077a5dfd23052c1e059bd51e62a4bce4b3f8378",
    ///         "version":2,
    ///         "digest":"6ZNCsz3aFqLwp883sD64wia77F9xxL27ymfe4Cj6GKij"
    ///     }
    /// }
    /// </code>
    /// </summary>
    [JsonObject]
    public class SuiOwnedObjectRef
    {
        [JsonProperty("owner")]
        public Owner Owner { get; set; }
        [JsonProperty("reference")]
        public SuiObjectRef Reference { get; set; }
    }
}