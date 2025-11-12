using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class TransactionBlockResponseQuery
    {
        [JsonProperty("filter", NullValueHandling = NullValueHandling.Include)]
        public ITransactionFilter Filter { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Include)]
        public TransactionBlockResponseOptions Options { get; set; }

        public TransactionBlockResponseQuery
        (
            ITransactionFilter filter = null,
            TransactionBlockResponseOptions options = null
        )
        {
            this.Filter = filter;
            this.Options = options;
        }
    }
}