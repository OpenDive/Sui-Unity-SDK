//
//  VotingRightsConverter.cs
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
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    public class VotingRightsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(VotingRights);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray voting_rights = JArray.Load(reader);

                // NOTE: We expect it to follow the response example provided in the RPC docs
                // "validators": [
                //      [
                //          "jc/20VUECmVvSBmxMRG1LFdGqGunLzlfuv4uw4R9HoFA5iSnUf32tfIFC8cgXPnTAATJCwx0Cv/TJs5nPMKyOi0k1T4q/rKG38Zo/UBgCJ1tKxe3md02+Q0zLlSnozjU",
                //          "2500"
                //      ],
                //      ...
                //      [
                //          "rd7vlNiYyI5A297/kcXxBfnPLHR/tvK8N+wD1ske2y4aV4z1RL6LCTHiXyQ9WbDDDZihbOO6HWzx1/UEJpkusK2zE0sFW+gUDS218l+wDYP45CIr8B/WrJOh/0152ljy",
                //          "2500"
                //      ]
                // ]
                return new VotingRights
                (
                    (string)voting_rights[0],
                    BigInteger.Parse((string)voting_rights[1])
                );
            }

            return new VotingRights(new SuiError(0, "Unable to convert JSON to VotingRights.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                VotingRights voting_rights = (VotingRights)value;

                writer.WriteStartArray();

                writer.WriteValue(voting_rights.AuthorityName);
                writer.WriteValue(voting_rights.StakeUnit.ToString());

                writer.WriteEndArray();
            }
        }
    }
}