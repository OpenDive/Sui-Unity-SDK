//
//  CommandConverter.cs
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

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Types;
using Sui.Utilities;

namespace Sui.Transactions
{
    public class CommandConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Command);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject input = JObject.Load(reader);
            switch (input.Properties().Select(p => p.Name).ToList()[0])
            {
                case "MoveCall":
                    JToken arguments_move_call = input["MoveCall"];
                    return new Command(CommandKind.MoveCall, new MoveCall(arguments_move_call));

                case "TransferObjects":
                    JArray arguments_transfer_objects = (JArray)input["TransferObjects"];
                    return new Command
                    (
                        CommandKind.TransferObjects,
                        new TransferObjects
                        (
                            ((JArray)arguments_transfer_objects[0]).Select((val) => val.ToObject<TransactionArgument>()).ToArray(),
                            ((JObject)arguments_transfer_objects[1]).ToObject<TransactionArgument>()
                        )
                    );

                case "SplitCoins":
                    JArray arguments_split_coins = (JArray)input["SplitCoins"];
                    return new Command
                    (
                        CommandKind.SplitCoins,
                        new SplitCoins
                        (
                            ((JObject)arguments_split_coins[0]).ToObject<TransactionArgument>(),
                            ((JArray)arguments_split_coins[1]).Select((val) => val.ToObject<TransactionArgument>()).ToArray()
                        )
                    );

                case "MergeCoins":
                    JArray arguments_merge_coins = (JArray)input["MergeCoins"];
                    return new Command
                    (
                        CommandKind.MergeCoins,
                        new MergeCoins
                        (
                            ((JObject)arguments_merge_coins[0]).ToObject<TransactionArgument>(),
                            ((JArray)arguments_merge_coins[1]).Select((val) => val.ToObject<TransactionArgument>()).ToArray()
                        )
                    );

                case "Publish":
                    JArray arguments_publish = (JArray)input["Publish"];
                    return new Command
                    (
                        CommandKind.Publish,
                        new Publish
                        (
                            ((JArray)arguments_publish[0]).Select((val) => ((JArray)val).Select((inner) => val.Value<byte>()).ToArray()).ToArray(),
                            ((JArray)arguments_publish[1]).Select((val) => AccountAddress.FromHex(val.Value<string>())).ToArray()
                        )
                    );

                case "MakeMoveVec":
                    JArray arguments_make_move_vec = (JArray)input["MakeMoveVec"];
                    return new Command
                    (
                        CommandKind.MakeMoveVec,
                        new MakeMoveVec
                        (
                            ((JArray)arguments_make_move_vec[0]).Select((val) => val.ToObject<TransactionArgument>()).ToArray(),
                            SuiStructTag.FromStr(arguments_make_move_vec[1].Value<string>())
                        )
                    );

                case "Upgrade":
                    JArray arguments_upgrade = (JArray)input["Upgrade"];
                    return new Command
                    (
                        CommandKind.Upgrade,
                        new Upgrade
                        (
                            ((JArray)arguments_upgrade[0]).Select((val) => ((JArray)val).Select((inner) => val.ToObject<byte>()).ToArray()).ToArray(),
                            ((JArray)arguments_upgrade[1]).Select((val) => AccountAddress.FromHex(val.Value<string>())).ToArray(),
                            arguments_upgrade[2].Value<string>(),
                            ((JObject)arguments_upgrade[3]).ToObject<TransactionArgument>()
                        )
                    );

                default:
                    return new SuiError(0, "Unable to convert JSON to Command.", input);
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
                Command transaction = (Command)value;

                writer.WriteStartObject();

                writer.WritePropertyName(transaction.Kind.ToString());

                switch (transaction.Kind)
                {
                    case CommandKind.MoveCall:
                        MoveCall tx_move_call = (MoveCall)transaction.Function;

                        writer.WriteStartObject();

                        writer.WritePropertyName("package");
                        writer.WriteValue(tx_move_call.Target.Address.ToHex());

                        writer.WritePropertyName("module");
                        writer.WriteValue(tx_move_call.Target.Module);

                        writer.WritePropertyName("function");
                        writer.WriteValue(tx_move_call.Target.Name);

                        writer.WritePropertyName("arguments");
                        writer.WriteStartArray();

                        foreach (TransactionArgument transaction_argument in tx_move_call.Arguments)
                            writer.WriteRaw(JsonConvert.SerializeObject(transaction_argument));

                        writer.WriteEndArray();

                        writer.WriteEndObject();
                        break;
                    case CommandKind.TransferObjects:
                        TransferObjects tx_transfer_objects = (TransferObjects)transaction.Function;
                        writer.WriteStartArray();

                        writer.WriteStartArray();
                        foreach (TransactionArgument transaction_argument in tx_transfer_objects.Objects)
                            writer.WriteRaw(JsonConvert.SerializeObject(transaction_argument));
                        writer.WriteEndArray();

                        writer.WriteRaw(JsonConvert.SerializeObject(tx_transfer_objects.Address));

                        writer.WriteEndArray();
                        break;
                    case CommandKind.SplitCoins:
                        SplitCoins tx_split_coins = (SplitCoins)transaction.Function;

                        writer.WriteStartArray();

                        writer.WriteRaw(JsonConvert.SerializeObject(tx_split_coins.Coin));

                        writer.WriteStartArray();
                        foreach (TransactionArgument transaction_argument in tx_split_coins.Amounts)
                            writer.WriteRaw(JsonConvert.SerializeObject(transaction_argument));
                        writer.WriteEndArray();

                        writer.WriteEndArray();
                        break;
                    case CommandKind.MergeCoins:
                        MergeCoins tx_merge_coins = (MergeCoins)transaction.Function;

                        writer.WriteStartArray();

                        writer.WriteRaw(JsonConvert.SerializeObject(tx_merge_coins.Destination));

                        writer.WriteStartArray();
                        foreach (TransactionArgument transaction_argument in tx_merge_coins.Sources)
                            writer.WriteRaw(JsonConvert.SerializeObject(transaction_argument));
                        writer.WriteEndArray();

                        writer.WriteEndArray();
                        break;
                    case CommandKind.Publish:
                        Publish tx_publish = (Publish)transaction.Function;
                        writer.WriteStartArray();

                        writer.WriteValue(tx_publish.Modules);
                        writer.WriteRaw(JsonConvert.SerializeObject(tx_publish.Dependencies));

                        writer.WriteEndArray();
                        break;
                    case CommandKind.MakeMoveVec:
                        MakeMoveVec tx_make_move_vec = (MakeMoveVec)transaction.Function;

                        writer.WriteStartArray();

                        writer.WriteStartArray();
                        foreach (TransactionArgument transaction_argument in tx_make_move_vec.Objects)
                            writer.WriteRaw(JsonConvert.SerializeObject(transaction_argument));
                        writer.WriteEndArray();

                        writer.WriteValue(tx_make_move_vec.Type.ToString());

                        writer.WriteEndArray();
                        break;
                    case CommandKind.Upgrade:
                        Upgrade tx_upgrade = (Upgrade)transaction.Function;

                        writer.WriteValue(tx_upgrade.Modules);

                        writer.WriteRaw(JsonConvert.SerializeObject(tx_upgrade.Dependencies));

                        writer.WriteValue(tx_upgrade.PackagID);

                        writer.WriteRaw(JsonConvert.SerializeObject(tx_upgrade.Ticket));
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}