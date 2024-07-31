using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Utilities;

namespace Sui.Transactions.Types.Arguments
{
    public enum TransactionArgumentKind
    {
        GasCoin,
        Input,
        Result,
        NestedResult
    }

    public class TransactionArgumentConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SuiTransactionArgument);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject input = JObject.Load(reader);
            switch (input.Properties().Select(p => p.Name).ToList()[0])
            {
                case "Input":
                    return new SuiTransactionArgument
                    (
                        TransactionArgumentKind.Input,
                        new TransactionBlockInput
                        (
                            input["Input"].Value<ushort>()
                        )
                    );
                case "GasCoin":
                    return new SuiTransactionArgument
                    (
                        TransactionArgumentKind.GasCoin,
                        null
                    );
                case "Result":
                    return new SuiTransactionArgument
                    (
                        TransactionArgumentKind.Result,
                        new Result(input["Result"].Value<ushort>())
                    );
                case "NestedResult":
                    return new SuiTransactionArgument
                    (
                        TransactionArgumentKind.NestedResult,
                        new NestedResult
                        (
                            ((JArray)input["NestedResult"])[0].Value<ushort>(),
                            ((JArray)input["NestedResult"])[1].Value<ushort>()
                        )
                    );
                default:
                    return new SuiError(0, "Unable to convert JSON to SuiTransactionArgument", input);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                SuiTransactionArgument transaction_argument = (SuiTransactionArgument)value;

                writer.WriteStartObject();

                writer.WritePropertyName(transaction_argument.Kind.ToString());

                switch (transaction_argument.Kind)
                {
                    case TransactionArgumentKind.GasCoin:
                        break;
                    case TransactionArgumentKind.Input:
                        writer.WriteValue(((TransactionBlockInput)transaction_argument.TransactionArgument).Index);
                        break;
                    case TransactionArgumentKind.Result:
                        writer.WriteValue(((Result)transaction_argument.TransactionArgument).Index);
                        break;
                    case TransactionArgumentKind.NestedResult:
                        writer.WriteStartArray();
                        writer.WriteValue(((NestedResult)transaction_argument.TransactionArgument).Index);
                        writer.WriteValue(((NestedResult)transaction_argument.TransactionArgument).ResultIndex);
                        writer.WriteEndArray();
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }

    /// <summary>
    /// There are 4 types of transaction arguments.
    /// 1) GasCoin, 2) TransactionBlockInput, 3) Result, 4) NestedResult
    /// A TransactionArgument is an abstraction over these 4 types, and does
    /// not have a concrete implementation or serialization strategy.
    /// Each concrete type has it's own serialization strategy, particularly
    /// add a byte that represents it's type.
    /// </summary>
    public interface ITransactionArgument : ISerializable { }

    [JsonConverter(typeof(TransactionArgumentConverter))]
    public class SuiTransactionArgument: ISerializable
    {
        public TransactionArgumentKind Kind { get; private set; }

        private ITransactionArgument transaction_argument;

        public ITransactionArgument TransactionArgument
        {
            get => this.transaction_argument;
            set
            {
                if (value == null)
                    this.Kind = TransactionArgumentKind.GasCoin;
                else if (value.GetType() == typeof(TransactionBlockInput))
                    this.Kind = TransactionArgumentKind.Input;
                else if (value.GetType() == typeof(Result))
                    this.Kind = TransactionArgumentKind.Result;
                else if (value.GetType() == typeof(NestedResult))
                    this.Kind = TransactionArgumentKind.NestedResult;
                else
                    throw new Exception("Unable to setup SuiTransactionArgument");

                this.transaction_argument = value;
            }
        }

        public SuiTransactionArgument(TransactionArgumentKind kind, ITransactionArgument transactionArgument)
        {
            this.Kind = kind;
            this.TransactionArgument = transactionArgument;
        }

        public void Serialize(Serialization serializer)
        {
            switch (this.Kind)
            {
                case TransactionArgumentKind.GasCoin:
                    serializer.SerializeU8(0);
                    break;
                case TransactionArgumentKind.Input:
                    serializer.SerializeU8(1);
                    break;
                case TransactionArgumentKind.Result:
                    serializer.SerializeU8(2);
                    break;
                case TransactionArgumentKind.NestedResult:
                    serializer.SerializeU8(3);
                    break;
            }
            if (this.TransactionArgument != null) serializer.Serialize(TransactionArgument);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte value = deserializer.DeserializeU8().Value;
            switch (value)
            {
                case 0:
                    return new SuiTransactionArgument
                    (
                        TransactionArgumentKind.GasCoin,
                        null
                    );
                case 1:
                    return new SuiTransactionArgument(
                        TransactionArgumentKind.Input,
                        (TransactionBlockInput)TransactionBlockInput.Deserialize(deserializer)
                    );
                case 2:
                    return new SuiTransactionArgument(
                        TransactionArgumentKind.Result,
                        (Result)Result.Deserialize(deserializer)
                    );
                case 3:
                    return new SuiTransactionArgument(
                        TransactionArgumentKind.NestedResult,
                        (NestedResult)NestedResult.Deserialize(deserializer)
                    );
                default:
                    return new SuiError(0, "Unable to deserialize TransactionArgument", null);
            }
        }
    }
}