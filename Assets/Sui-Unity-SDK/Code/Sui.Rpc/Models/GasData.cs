using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    ///
    ///
    /// <code>
    ///     "gasData":{
    ///         "payment":[
    ///             {
    ///                 "objectId":"0x1a3e898029d024eec1d44c6af5e2facded84d03b5373514f16e3d66e00081051",
    ///                 "version":2,
    ///                 "digest":"7nDZ5J4VyvYGUbX2f6mQdhkr3RFrb3vZqui1ogoyApD9"
    ///             }
    ///         ],
    ///         "owner":"0x82179c57d5895babfb655cd62e8e886a53334b5e7be9be658eb759cc35e3fc66",
    ///         "price":"10",
    ///         "budget":"100000"
    ///     }
    /// </code>
    /// </summary>
    [JsonObject]
    public class GasData
    {
        [JsonProperty("payment")]
        public List<ObjectRef> Payment { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("price")]
        public BigInteger Price { get; set; }

        [JsonProperty("budget")]
        public BigInteger Budget { get; set; }
    }
}