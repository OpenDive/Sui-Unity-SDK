using System;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class TransactionBlockResponseOptions
    {
        [JsonProperty("showInput")]
        public bool ShowInput { get; set; }

        [JsonProperty("showEffects")]
        public bool ShowEffects { get; set; }

        [JsonProperty("showEvents")]
        public bool ShowEvents { get; set; }

        [JsonProperty("showObjectChanges")]
        public bool ShowObjectChanges { get; set; }

        [JsonProperty("showBalanceChanges")]
        public bool ShowBalanceChanges { get; set; }

        public TransactionBlockResponseOptions
        (
            bool showInput = false,
            bool showEffects = false,
            bool showEvents = false,
            bool showObjectChanges = false,
            bool showBalanceChanges = false
        )
        {
            this.ShowInput = showInput;
            this.ShowEffects = showEffects;
            this.ShowEvents = showEvents;
            this.ShowObjectChanges = showObjectChanges;
            this.ShowBalanceChanges = showBalanceChanges;
        }
    }
}
