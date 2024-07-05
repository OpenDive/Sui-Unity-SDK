using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    public class ObjectDataOptions
    {
        [JsonProperty("showBcs")]
        public bool ShowBcs { get; set; } = false;

        [JsonProperty("showContent")]
        public bool ShowContent { get; set; } = false;

        [JsonProperty("showDisplay")]
        public bool ShowDisplay { get; set; } = false;

        [JsonProperty("showOwner")]
        public bool ShowOwner { get; set; } = false;

        [JsonProperty("showPreviousTransaction")]
        public bool ShowPreviousTransaction { get; set; } = false;

        [JsonProperty("showStorageRebate")]
        public bool ShowStorageRebate { get; set; } = false;

        [JsonProperty("showType")]
        public bool ShowType { get; set; } = false;

        public ObjectDataOptions
        (
            bool show_bcs = false,
            bool show_content = false,
            bool show_display = false,
            bool show_owner = false,
            bool show_previous_transaction = false,
            bool show_storage_rebate = false,
            bool show_type = false
        )
        {
            this.ShowBcs = show_bcs;
            this.ShowContent = show_content;
            this.ShowDisplay = show_display;
            this.ShowOwner = show_owner;
            this.ShowPreviousTransaction = show_previous_transaction;
            this.ShowStorageRebate = show_storage_rebate;
            this.ShowType = show_type;
        }

        // Factory
        public static ObjectDataOptions ShowAll()
        {
            return new ObjectDataOptions
            {
                ShowBcs = true,
                ShowContent = true,
                ShowDisplay = true,
                ShowOwner = true,
                ShowPreviousTransaction = true,
                ShowStorageRebate = true,
                ShowType = true
            };
        }

        public static ObjectDataOptions ShowNone()
        {
            return new ObjectDataOptions
            {
                ShowBcs = false,
                ShowContent = false,
                ShowDisplay = false,
                ShowOwner = false,
                ShowPreviousTransaction = false,
                ShowStorageRebate = false,
                ShowType = false
            };
        }
    }
}