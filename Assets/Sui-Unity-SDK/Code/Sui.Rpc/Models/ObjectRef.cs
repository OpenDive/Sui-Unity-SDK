using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    ///
    /// Below are some json examples of an object ref
    ///
    /// <code>
    /// {
    ///     "objectId":"0x1a3e898029d024eec1d44c6af5e2facded84d03b5373514f16e3d66e00081051",
    ///     "version":2,
    ///     "digest":"7nDZ5J4VyvYGUbX2f6mQdhkr3RFrb3vZqui1ogoyApD9"
    /// }
    /// </code>
    /// </summary>
    [JsonObject]
    public class ObjectRef
    {
        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }
    }
}