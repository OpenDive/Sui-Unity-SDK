using Newtonsoft.Json;

namespace Sui.Clients.Faucet.Request
{
    
    public class FaucetRequesst
    {
        [JsonProperty("FixedAmountRequest")]
        public FixedAmountRequest FixedAmountRequest { get; set; }

        public FaucetRequesst(string recipient)
        {
            FixedAmountRequest = new FixedAmountRequest();
            FixedAmountRequest.Recipient = recipient;
        }
    }

    [JsonObject]
    public class FixedAmountRequest
    {
        [JsonProperty("recipient")]
        public string Recipient { get; set; }
    }
}