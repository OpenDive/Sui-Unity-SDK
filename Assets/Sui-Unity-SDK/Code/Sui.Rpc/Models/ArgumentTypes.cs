using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Sui.Accounts;
using UnityEngine;

namespace Sui.Rpc.Models
{
	public enum ArgumentType
	{
		Object,
        Pure
	}

    public enum ObjectValueType
    {
        ByImmutableReference,
        ByMutableReference,
        ByValue
    }

    [JsonObject, JsonConverter(typeof(MoveFunctionArgTypesConverter))]
    public class MoveFunctionArgTypes
    {
        public MoveFunctionArgType[] ArgTypes;

        public MoveFunctionArgTypes(MoveFunctionArgType[] ArgTypes)
        {
            this.ArgTypes = ArgTypes;
        }

        public override bool Equals(object obj)
        {
            if (obj is not MoveFunctionArgTypes)
                throw new NotImplementedException();

            MoveFunctionArgTypes other_args = (MoveFunctionArgTypes)obj;

            return this.ArgTypes.SequenceEqual(other_args.ArgTypes);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

	[JsonObject]
	public class MoveFunctionArgType
    {
		public ArgumentType ArgumentType;

		public ObjectValueType ArgumentReference;

        public MoveFunctionArgType(ArgumentType arument_type, ObjectValueType argument_reference)
        {
            this.ArgumentType = arument_type;
            this.ArgumentReference = argument_reference;
        }

        public MoveFunctionArgType() { }

        public override bool Equals(object obj)
        {
            if (obj is not MoveFunctionArgType)
                throw new NotImplementedException();

            MoveFunctionArgType other_args = (MoveFunctionArgType)obj;

            return
                (int)this.ArgumentType == (int)other_args.ArgumentType &&
                (int)this.ArgumentReference == (int)other_args.ArgumentReference;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MoveFunctionArgTypesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MoveFunctionArgTypes);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray types = JArray.Load(reader);
            List<MoveFunctionArgType> args = new List<MoveFunctionArgType>();
            foreach (JToken type in types)
            {
                MoveFunctionArgType arg = new MoveFunctionArgType();
                if (type.Type == JTokenType.Object)
                {
                    arg.ArgumentType = ArgumentType.Object;
                    switch((string)(type as JObject)["Object"])
                    {
                        case "ByImmutableReference":
                            arg.ArgumentReference = ObjectValueType.ByImmutableReference;
                            break;
                        case "ByMutableReference":
                            arg.ArgumentReference = ObjectValueType.ByMutableReference;
                            break;
                        case "ByValue":
                            arg.ArgumentReference = ObjectValueType.ByValue;
                            break;
                    }

                }
                else
                    arg.ArgumentType = ArgumentType.Pure;

                args.Add(arg);
            }
            return new MoveFunctionArgTypes(args.ToArray());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}