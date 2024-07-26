using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    public enum DataType
    {
        MoveObject,
        MovePackage
    }

    public interface IParsedData { }

    [JsonConverter(typeof(DataJsonConverter))]
    public class Data : ReturnBase
    {
        public DataType Type { get; internal set; }

        public IParsedData ParsedData { get; internal set; }

        public Data
        (
            DataType type,
            IParsedData parsed_data
        )
        {
            this.Type = type;
            this.ParsedData = parsed_data;
        }

        public Data(SuiError error)
        {
            this.Error = error;
        }
    }

    public class ParsedMoveObject : IParsedData
    {
        public JToken Fields { get; internal set; }

        public bool HasPublicTransfer { get; internal set; }

        public SuiStructTag Type { get; internal set; }

        public ParsedMoveObject
        (
            JToken fields,
            bool has_public_transfer,
            SuiStructTag type
        )
        {
            this.Fields = fields;
            this.HasPublicTransfer = has_public_transfer;
            this.Type = type;
        }
    }

    public class ParsedMovePackage : IParsedData
    {
        public Dictionary<string, object> Disassembled { get; internal set; }

        public ParsedMovePackage
        (
            Dictionary<string, object> disassembled
        )
        {
            this.Disassembled = disassembled;
        }
    }
}