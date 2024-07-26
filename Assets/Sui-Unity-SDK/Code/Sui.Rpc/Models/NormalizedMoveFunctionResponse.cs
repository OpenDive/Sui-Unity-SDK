using System.Linq;
using Newtonsoft.Json;
using Sui.Transactions;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a normalized function within a Sui Move entity.
    /// </summary>
    [JsonObject]
    public class NormalizedMoveFunctionResponse
    {
        /// <summary>
        /// Defines the visibility of the function (e.g. public, private).
        /// </summary>
        [JsonProperty("visibility")]
        public string Visibility { get; internal set; }

        /// <summary>
        /// Indicates whether the function is an entry point to its containing entity.
        /// </summary>
        [JsonProperty("isEntry")]
        public bool IsEntry { get; internal set; }

        /// <summary>
        /// Holds the ability sets for the function's type parameters, detailing the capabilities of those types.
        /// </summary>
        [JsonProperty("typeParameters")]
        public TypeParameter[] TypeParameters { get; internal set; }

        /// <summary>
        /// Represents the types of the parameters that the function can receive.
        /// </summary>
        [JsonProperty("parameters")]
        public SuiMoveNormalizedType[] Parameters { get; internal set; }

        /// <summary>
        /// Represents the types of the values that the function can return.
        /// </summary>
        [JsonProperty("return")]
        public SuiMoveNormalizedType[] Return { get; internal set; }

        /// <summary>
        /// Returns whether or not that this object contains a TxContext parameter or not.
        /// </summary>
        /// <returns>A `bool` value indiciating whether or not the function parameter has a TxContext.</returns>
        public bool HasTxContext()
        {
            if (this.Parameters.Length == 0)
                return false;

            SuiMoveNormalizedType possibly_tx_context = this.Parameters.Last();
            SuiMoveNormalziedTypeStruct struct_tag = Serializer.ExtractStructType(possibly_tx_context);

            if (struct_tag == null)
                return false;

            return
                struct_tag.Struct.Address.ToHex() == Utils.NormalizeSuiAddress("0x2") &&
                struct_tag.Struct.Module == "tx_context" &&
                struct_tag.Struct.Name == "TxContext";
        }
    }

    /// <summary>
    /// Represents the set of abilities of a Sui Move in the ecosystem.
    /// </summary>
    [JsonObject]
    public class TypeParameter
    {
        /// <summary>
        /// Holds the abilities of the Sui Move.
        /// </summary>
        [JsonProperty("abilities")]
        public string[] Abilities { get; internal set; }
    }
}