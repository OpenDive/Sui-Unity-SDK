//
//  ValidatorReportRecordConverter.cs
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
using Sui.Accounts;
using Sui.Utilities;
using System;
using System.Linq;

namespace Sui.Rpc.Models
{
    public class ValidatorReportRecordConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ValidatorReportRecord);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray validator_report_record = JArray.Load(reader);

                return new ValidatorReportRecord
                (
                    (AccountAddress)validator_report_record[0].ToObject(typeof(AccountAddress)),
                    ((JArray)validator_report_record[1]).Select(record => (AccountAddress)record.ToObject(typeof(AccountAddress))).ToArray()
                );
            }

            return new ValidatorReportRecord(new SuiError(0, "Unable to convert JSON to ValidatorReportRecord.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                ValidatorReportRecord validator_report_record = (ValidatorReportRecord)value;

                writer.WriteStartArray();

                writer.WriteValue(validator_report_record.Validator.KeyHex);
                writer.WriteValue(validator_report_record.ReportedValidators.Select(validator => validator.KeyHex).ToArray());

                writer.WriteEndArray();
            }
        }
    }
}