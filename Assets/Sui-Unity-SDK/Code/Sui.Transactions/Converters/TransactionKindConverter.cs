//
//  TransactionKindConverter.cs
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
using Newtonsoft.Json.Linq;
using Sui.Utilities;
using System;

namespace Sui.Transactions
{
    public class TransactionKindConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(TransactionKind);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject transaction_kind = JObject.Load(reader);

                switch (transaction_kind["kind"].Value<string>())
                {
                    case "ProgrammableTransaction":
                        return new TransactionKind
                        (
                            TransactionKindType.ProgrammableTransaction,
                            transaction_kind.ToObject<ProgrammableTransaction>(serializer)
                        );
                    case "ChangeEpoch":
                        return new TransactionKind
                        (
                            TransactionKindType.ChangeEpoch,
                            transaction_kind.ToObject<SuiChangeEpoch>(serializer)
                        );
                    case "Genesis":
                        return new TransactionKind
                        (
                            TransactionKindType.Genesis,
                            transaction_kind.ToObject<Genesis>(serializer)
                        );
                    case "ConsensusCommitPrologue":
                        return new TransactionKind
                        (
                            TransactionKindType.ConsensusCommitPrologue,
                            transaction_kind.ToObject<SuiConsensusCommitPrologue>(serializer)
                        );
                    default:
                        return new TransactionKind(new SuiError(0, $"Unable to convert {transaction_kind["kind"].Value<string>()} to TransactionKindType", null));
                }
            }

            return new TransactionKind(new SuiError(0, "Unable to convert JSON to TransactionKind", null));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                TransactionKind transaction_kind = (TransactionKind)value;

                writer.WriteStartObject();

                writer.WritePropertyName("kind");
                writer.WriteValue(transaction_kind.Type.ToString());

                switch (transaction_kind.Type)
                {
                    case TransactionKindType.ProgrammableTransaction:
                        ProgrammableTransaction programmable_tx = (ProgrammableTransaction)transaction_kind.Transaction;
                        writer.WriteRaw(JsonConvert.SerializeObject(programmable_tx));
                        break;
                    case TransactionKindType.ChangeEpoch:
                        SuiChangeEpoch change_epoch = (SuiChangeEpoch)transaction_kind.Transaction;
                        writer.WriteRaw(JsonConvert.SerializeObject(change_epoch));
                        break;
                    case TransactionKindType.Genesis:
                        Genesis genesis = (Genesis)transaction_kind.Transaction;
                        writer.WriteRaw(JsonConvert.SerializeObject(genesis));
                        break;
                    case TransactionKindType.ConsensusCommitPrologue:
                        SuiConsensusCommitPrologue consensus_commit_prologue = (SuiConsensusCommitPrologue)transaction_kind.Transaction;
                        writer.WriteRaw(JsonConvert.SerializeObject(consensus_commit_prologue));
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}