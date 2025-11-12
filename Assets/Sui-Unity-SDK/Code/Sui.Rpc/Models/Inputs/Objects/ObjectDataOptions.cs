//
//  ObjectDataOptions.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A class representing the various display options for `ObjectData`.
    /// </summary>
    public class ObjectDataOptions
    {
        /// <summary>
        /// Indicates whether to show BCS representation.
        /// </summary>
        [JsonProperty("showBcs")]
        public bool ShowBcs { get; set; }

        /// <summary>
        /// Indicates whether to display the content of the object.
        /// </summary>
        [JsonProperty("showContent")]
        public bool ShowContent { get; set; }

        /// <summary>
        /// Indicates whether to show the display representation of the object.
        /// </summary>
        [JsonProperty("showDisplay")]
        public bool ShowDisplay { get; set; }

        /// <summary>
        /// Indicates whether to show the owner of the object.
        /// </summary>
        [JsonProperty("showOwner")]
        public bool ShowOwner { get; set; }

        /// <summary>
        /// Indicates whether to show the previous transaction of the object.
        /// </summary>
        [JsonProperty("showPreviousTransaction")]
        public bool ShowPreviousTransaction { get; set; }

        /// <summary>
        /// Indicates whether to show the storage rebate of the object.
        /// </summary>
        [JsonProperty("showStorageRebate")]
        public bool ShowStorageRebate { get; set; }

        /// <summary>
        /// Indicates whether to show the type of the object.
        /// </summary>
        [JsonProperty("showType")]
        public bool ShowType { get; set; }

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
    }
}