using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// <para>`SuiMoveFunctionArgType` represents the type of an argument in a Move function.</para>
    ///
    /// <para>- `pure`: The argument is a pure value, meaning it doesn't involve any object references.</para>
    /// <para>- `object`: The argument is an object, and its kind (immutable reference, mutable reference, or by value) is specified.</para>
    /// </summary>
	public enum ArgumentType
	{
		Object,
        Pure
	}

    /// <summary>
    /// <para>`ObjectValueKind` represents how an object value is treated or accessed.</para>
    ///
    /// <para>- `byImmutableReference`: The object value is accessed by an immutable reference.</para>
    /// <para>- `byMutableReference`: The object value is accessed by a mutable reference.</para>
    /// <para>- `byValue`: The object value is accessed by value.</para>
    /// </summary>
    public enum ObjectValueType
    {
        ByImmutableReference,
        ByMutableReference,
        ByValue
    }

    /// <summary>
    /// The class that represents the type of Move function argument.
    /// </summary>
    [JsonConverter(typeof(MoveFunctionArgTypesConverter))]
	public class MoveFunctionArgType : ReturnBase
    {
        /// <summary>
        /// The argument's type. Can be pure or an object.
        /// </summary>
		public ArgumentType ArgumentType { get; internal set; }

        /// <summary>
        /// If the argument is an object, it can be immutable, mutable, or by value.
        /// </summary>
		public ObjectValueType? ArgumentReference { get; internal set; }

        public MoveFunctionArgType
        (
            ArgumentType arument_type,
            ObjectValueType? argument_reference = null
        )
        {
            this.ArgumentType = arument_type;
            this.ArgumentReference = argument_reference;
        }

        public MoveFunctionArgType(SuiError error)
        {
            this.Error = error;
        }

        public override bool Equals(object obj)
        {
            if (obj is not MoveFunctionArgType)
                return this.SetError<bool, SuiError>(false, "Object is not MoveFunctionArgType.", obj);

            MoveFunctionArgType other_args = (MoveFunctionArgType)obj;

            return
                this.ArgumentType == other_args.ArgumentType &&
                this.ArgumentReference == other_args.ArgumentReference;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class MoveFunctionArgTypesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(MoveFunctionArgType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject object_arg_type = JObject.Load(reader);

                switch (object_arg_type[ArgumentType.Object.ToString()].Value<string>())
                {
                    case "ByImmutableReference":
                        return new MoveFunctionArgType
                        (
                            ArgumentType.Object,
                            ObjectValueType.ByImmutableReference
                        );
                    case "ByMutableReference":
                        return new MoveFunctionArgType
                        (
                            ArgumentType.Object,
                            ObjectValueType.ByMutableReference
                        );
                    case "ByValue":
                        return new MoveFunctionArgType
                        (
                            ArgumentType.Object,
                            ObjectValueType.ByValue
                        );
                    default:
                        return new MoveFunctionArgType
                        (
                            new SuiError
                            (
                                0,
                                $"Unable to convert {object_arg_type[ArgumentType.Object.ToString()].Value<string>()} to MoveFunctionArgType.",
                                reader
                            )
                        );
                }
            }
            else if (reader.TokenType == JsonToken.String)
                return new MoveFunctionArgType(ArgumentType.Pure);

            return new MoveFunctionArgType(new SuiError(0, "Unable to convert JSON to MoveFunctionArgType.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                MoveFunctionArgType move_function_arg_type = (MoveFunctionArgType)value;

                switch (move_function_arg_type.ArgumentType)
                {
                    case ArgumentType.Pure:
                        writer.WriteValue(ArgumentType.Pure.ToString());
                        break;
                    case ArgumentType.Object:
                        writer.WriteStartObject();

                        writer.WritePropertyName(ArgumentType.Object.ToString());
                        writer.WriteValue(move_function_arg_type.ArgumentReference.ToString());

                        writer.WriteEndObject();
                        break;
                }
            }
        }
    }
}