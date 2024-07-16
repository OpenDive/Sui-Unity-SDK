using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Client;
using Sui.Transactions.Types.Arguments;
using Sui.Types;

namespace Sui.Transactions.Types
{
    public enum TransactionKind
    {
        MoveCall,
        TransferObjects,
        SplitCoins,
        MergeCoins,
        Publish,
        Upgrade,
        MakeMoveVec,
    }

    public class TransactionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SuiTransaction);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject input = JObject.Load(reader);
            switch (input.Properties().Select(p => p.Name).ToList()[0])
            {
                case "MoveCall":
                    JToken arguments_move_call = input["MoveCall"];
                    return new SuiTransaction(TransactionKind.MoveCall, new MoveCall(arguments_move_call));
                case "TransferObjects":
                    JArray arguments_transfer_objects = (JArray)input["TransferObjects"];
                    return new SuiTransaction
                    (
                        TransactionKind.TransferObjects,
                        new TransferObjects
                        (
                            ((JArray)arguments_transfer_objects[0]).Select((val) => val.ToObject<SuiTransactionArgument>()).ToArray(),
                            ((JObject)arguments_transfer_objects[1]).ToObject<SuiTransactionArgument>()
                        )
                    );
                case "SplitCoins":
                    JArray arguments_split_coins = (JArray)input["SplitCoins"];
                    return new SuiTransaction
                    (
                        TransactionKind.SplitCoins,
                        new SplitCoins
                        (
                            ((JObject)arguments_split_coins[0]).ToObject<SuiTransactionArgument>(),
                            ((JArray)arguments_split_coins[1]).Select((val) => val.ToObject<SuiTransactionArgument>()).ToArray()
                        )
                    );
                case "MergeCoins":
                    JArray arguments_merge_coins = (JArray)input["MergeCoins"];
                    return new SuiTransaction
                    (
                        TransactionKind.MergeCoins,
                        new MergeCoins
                        (
                            ((JObject)arguments_merge_coins[0]).ToObject<SuiTransactionArgument>(),
                            ((JArray)arguments_merge_coins[1]).Select((val) => val.ToObject<SuiTransactionArgument>()).ToArray()
                        )
                    );
                case "Publish":
                    JArray arguments_publish = (JArray)input["Publish"];
                    return new SuiTransaction
                    (
                        TransactionKind.Publish,
                        new Publish
                        (
                            ((JArray)arguments_publish[0]).Select((val) => ((JArray)val).Select((inner) => val.ToObject<byte>()).ToArray()).ToArray(),
                            ((JArray)arguments_publish[1]).Select((val) => AccountAddress.FromHex(val.Value<string>())).ToArray()
                        )
                    );
                case "MakeMoveVec":
                    JArray arguments_make_move_vec = (JArray)input["MakeMoveVec"];
                    return new SuiTransaction
                    (
                        TransactionKind.MakeMoveVec,
                        new MakeMoveVec
                        (
                            ((JArray)arguments_make_move_vec[0]).Select((val) => val.ToObject<SuiTransactionArgument>()).ToArray(),
                            SuiStructTag.FromStr(arguments_make_move_vec[1].Value<string>())
                        )
                    );
                case "Upgrade":
                    JArray arguments_upgrade = (JArray)input["Upgrade"];
                    return new SuiTransaction
                    (
                        TransactionKind.Upgrade,
                        new Upgrade
                        (
                            ((JArray)arguments_upgrade[0]).Select((val) => ((JArray)val).Select((inner) => val.ToObject<byte>()).ToArray()).ToArray(),
                            ((JArray)arguments_upgrade[1]).Select((val) => AccountAddress.FromHex(val.Value<string>())).ToArray(),
                            arguments_upgrade[2].Value<string>(),
                            ((JObject)arguments_upgrade[3]).ToObject<SuiTransactionArgument>()
                        )
                    );
                default:
                    throw new NotImplementedException();
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A TransactionObject can be:
    /// MakeMove, MergeCoin, MoveCall, Publish, SplitCOins, TransferObject, Upgrade
    /// </summary>
    public interface ITransaction : ISerializable { }

    [JsonConverter(typeof(TransactionConverter))]
    public class SuiTransaction: ISerializable
    {
        public ITransaction Transaction;
        public TransactionKind Kind { get; }

        public SuiTransaction(TransactionKind kind, ITransaction transaction)
        {
            this.Kind = kind;
            this.Transaction = transaction;
        }

        public void Serialize(Serialization serializer)
        {
            switch(this.Kind)
            {
                case TransactionKind.MoveCall:
                    serializer.SerializeU8(0);
                    break;
                case TransactionKind.TransferObjects:
                    serializer.SerializeU8(1);
                    break;
                case TransactionKind.SplitCoins:
                    serializer.SerializeU8(2);
                    break;
                case TransactionKind.MergeCoins:
                    serializer.SerializeU8(3);
                    break;
                case TransactionKind.Publish:
                    serializer.SerializeU8(4);
                    break;
                case TransactionKind.MakeMoveVec:
                    serializer.SerializeU8(5);
                    break;
                case TransactionKind.Upgrade:
                    serializer.SerializeU8(6);
                    break;
            }
            serializer.Serialize(Transaction);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            var value = deserializer.DeserializeU8();
            switch (value)
            {
                case 0:
                    return new SuiTransaction(
                        TransactionKind.MoveCall,
                        (MoveCall)MoveCall.Deserialize(deserializer)
                    );
                case 1:
                    return new SuiTransaction(
                        TransactionKind.TransferObjects,
                        (TransferObjects)TransferObjects.Deserialize(deserializer)
                    );
                case 2:
                    return new SuiTransaction(
                        TransactionKind.SplitCoins,
                        SplitCoins.Deserialize(deserializer)
                    );
                case 3:
                    return new SuiTransaction(
                        TransactionKind.MergeCoins,
                        MergeCoins.Deserialize(deserializer)
                    );
                case 4:
                    return new SuiTransaction(
                        TransactionKind.Publish,
                        Publish.Deserialize(deserializer)
                    );
                case 5:
                    return new SuiTransaction(
                        TransactionKind.MakeMoveVec,
                        (MakeMoveVec)MakeMoveVec.Deserialize(deserializer)
                    );
                case 6:
                    return new SuiTransaction(
                        TransactionKind.Upgrade,
                        (Upgrade)Upgrade.Deserialize(deserializer)
                    );
                default:
                    return new SuiError(0, "Unable to deserialize SuiTransaction.", null);
            }
        }
    }
}