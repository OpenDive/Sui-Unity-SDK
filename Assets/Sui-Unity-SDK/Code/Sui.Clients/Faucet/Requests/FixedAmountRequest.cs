using Newtonsoft.Json;

namespace Sui.Clients.Faucet.Request
{
    public class FixedAmountRequest
    {
        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        public FixedAmountRequest(string recipient) { Recipient = recipient;  }
    }
}