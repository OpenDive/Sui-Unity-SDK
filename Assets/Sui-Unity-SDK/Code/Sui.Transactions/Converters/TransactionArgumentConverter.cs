//
//  TransactionArgumentConverter.cs
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
using Sui.Utilities;

namespace Sui.Transactions
{
    public class TransactionArgumentConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(TransactionArgument);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject input = JObject.Load(reader);
            switch (input.Properties().Select(p => p.Name).ToList()[0])
            {
                case "Input":
                    return new TransactionArgument
                    (
                        TransactionArgumentKind.Input,
                        new TransactionBlockInput
                        (
                            input["Input"].Value<ushort>()
                        )
                    );
                case "GasCoin":
                    return new TransactionArgument
                    (
                        TransactionArgumentKind.GasCoin,
                        null
                    );
                case "Result":
                    return new TransactionArgument
                    (
                        TransactionArgumentKind.Result,
                        new Result(input["Result"].Value<ushort>())
                    );
                case "NestedResult":
                    return new TransactionArgument
                    (
                        TransactionArgumentKind.NestedResult,
                        new NestedResult
                        (
                            ((JArray)input["NestedResult"])[0].Value<ushort>(),
                            ((JArray)input["NestedResult"])[1].Value<ushort>()
                        )
                    );
                default:
                    return new SuiError(0, "Unable to convert JSON to TransactionArgument", input);
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
                TransactionArgument transaction_argument = (TransactionArgument)value;

                writer.WriteStartObject();

                writer.WritePropertyName(transaction_argument.Kind.ToString());

                switch (transaction_argument.Kind)
                {
                    case TransactionArgumentKind.GasCoin:
                        break;
                    case TransactionArgumentKind.Input:
                        writer.WriteValue(((TransactionBlockInput)transaction_argument.Argument).Index);
                        break;
                    case TransactionArgumentKind.Result:
                        writer.WriteValue(((Result)transaction_argument.Argument).Index);
                        break;
                    case TransactionArgumentKind.NestedResult:
                        writer.WriteStartArray();
                        writer.WriteValue(((NestedResult)transaction_argument.Argument).Index);
                        writer.WriteValue(((NestedResult)transaction_argument.Argument).ResultIndex);
                        writer.WriteEndArray();
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}