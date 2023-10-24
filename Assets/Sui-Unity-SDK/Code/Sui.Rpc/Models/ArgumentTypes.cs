using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Sui.Rpc.Models
{
	public enum ArgumentType
	{
		Object, Pure
	}

    [JsonObject, JsonConverter(typeof(MoveFunctionArgTypesConverter))]
    public class MoveFunctionArgTypes
    {
        public MoveFunctionArgType[] ArgTypes;

        public MoveFunctionArgTypes(MoveFunctionArgType[] ArgTypes)
        {
            this.ArgTypes = ArgTypes;
        }
    }

	[JsonObject]
	public class MoveFunctionArgType
    {
		public ArgumentType ArgumentType;

		public string ArgumentReference;  // Is null when argument type is pure, it isn't when the argument type is object
	}

    public class MoveFunctionArgTypesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MoveFunctionArgTypes);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject typesRaw = JObject.Load(reader);
            JArray types = (JArray)typesRaw["result"];
            List<MoveFunctionArgType> args = new List<MoveFunctionArgType>();
            foreach (JToken type in types)
            {
                MoveFunctionArgType arg = new MoveFunctionArgType();
                // TODO: Look into more efficient implementation of this.
                //if (type["Object"] != null)
                //{
                //    arg.ArgumentType = ArgumentType.Object;
                //    arg.ArgumentReference = (string)type["Object"];
                //    args.Add(arg);
                //    continue;
                //}
                //arg.ArgumentType = ArgumentType.Pure;
                //arg.ArgumentReference = null;
                //args.Add(arg);
                //continue;

                try
                {
                    arg.ArgumentType = ArgumentType.Object;
                    arg.ArgumentReference = (string)(type as JObject)["Object"];
                    args.Add(arg);
                    continue;
                }
                catch
                {
                    arg.ArgumentType = ArgumentType.Pure;
                    arg.ArgumentReference = null;
                    args.Add(arg);
                    continue;
                }
            }
            return new MoveFunctionArgTypes(args.ToArray());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}