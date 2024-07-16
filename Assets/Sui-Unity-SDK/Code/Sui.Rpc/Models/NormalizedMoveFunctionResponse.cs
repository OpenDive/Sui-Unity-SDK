using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Transactions;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class NormalizedMoveFunctionResponse
    {
        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("isEntry")]
        public bool IsEntry { get; set; }

        [JsonProperty("typeParameters")]
        public List<TypeParameter> TypeParameters { get; set; }

        [JsonProperty("parameters")]
        public List<SuiMoveNormalizedType> Parameters { get; set; }

        [JsonProperty("return")]
        public List<SuiMoveNormalizedType> Return { get; set; }

        public bool HasTxContext()
        {
            if (this.Parameters.Count == 0)
                return false;

            SuiMoveNormalizedType possibly_tx_context = this.Parameters.Last();
            SuiMoveNormalziedTypeStruct struct_tag = Serializer.ExtractStructType(possibly_tx_context);

            if (struct_tag == null)
                return false;

            return
                struct_tag.Struct.StructTag.address.ToHex() == NormalizedTypeConverter.NormalizeSuiAddress("0x2") &&
                struct_tag.Struct.StructTag.module == "tx_context" &&
                struct_tag.Struct.StructTag.name == "TxContext";
        }
    }

    [JsonObject]
    public class TypeParameter
    {
        [JsonProperty("abilities")]
        public string[] Abilities { get; set; }
    }

    [JsonObject]
    public class NormalizedTest
    {
        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("isEntry")]
        public bool IsEntry { get; set; }

        //[JsonProperty("typeParameters")]
        //public List<TypeParameter> TypeParameters { get; set; }

        //[JsonProperty("parameters"), JsonConverter(typeof(NormalizedTypesConverter))]
        //public List<ISuiMoveNormalizedType> Parameters { get; set; }

        //[JsonProperty("return"), JsonConverter(typeof(NormalizedTypesConverter))]
        //public List<ISuiMoveNormalizedType> Return { get; set; }
    }
}