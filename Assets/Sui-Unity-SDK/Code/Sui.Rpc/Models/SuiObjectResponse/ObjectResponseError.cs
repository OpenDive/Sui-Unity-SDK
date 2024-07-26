using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    public class ObjectResponseErrorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ObjectResponseError);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject object_response_error = (JObject)JToken.ReadFrom(reader);

                switch (object_response_error["code"].Value<string>())
                {
                    case "notExist":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorNotExist
                            (
                                (AccountAddress)object_response_error["objectId"].ToObject(typeof(AccountAddress))
                            ),
                            ObjectResponseErrorType.NotExist
                        );
                    case "dynamicFieldNotFound":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorDynamicFieldNotFound
                            (
                                (AccountAddress)object_response_error["parentObjectId"].ToObject(typeof(AccountAddress))
                            ),
                            ObjectResponseErrorType.DynamicFieldNotFound
                        );
                    case "deleted":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorDeleted
                            (
                                object_response_error["digest"].Value<string>(),
                                (AccountAddress)object_response_error["objectId"].ToObject(typeof(AccountAddress)),
                                BigInteger.Parse(object_response_error["version"].Value<string>())
                            ),
                            ObjectResponseErrorType.Deleted
                        );
                    case "unknown":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorUnknown(),
                            ObjectResponseErrorType.Unknown
                        );
                    case "displayError":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorDisplayError
                            (
                                object_response_error["error"].Value<string>()
                            ),
                            ObjectResponseErrorType.DisplayError
                        );
                    default:
                        return new ObjectResponseError(new SuiError(0, "An unknown error has occured.", object_response_error));
                }
            }

            return new ObjectResponseError(new SuiError(0, "Unable to convert JSON to ObjectResponseError.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartObject();
                ObjectResponseError object_error = (ObjectResponseError)value;

                writer.WritePropertyName("code");
                writer.WriteValue(object_error.Type.ToString());

                switch (object_error.Type)
                {
                    case ObjectResponseErrorType.NotExist:
                        writer.WritePropertyName("objectId");
                        writer.WriteValue(((ObjectResponseErrorNotExist)object_error.ObjectError).ObjectID);
                        break;
                    case ObjectResponseErrorType.DynamicFieldNotFound:
                        writer.WritePropertyName("parentObjectId");
                        writer.WriteValue(((ObjectResponseErrorDynamicFieldNotFound)object_error.ObjectError).ParentObjectID);
                        break;
                    case ObjectResponseErrorType.Deleted:
                        ObjectResponseErrorDeleted object_deleted = (ObjectResponseErrorDeleted)object_error.ObjectError;

                        writer.WritePropertyName("digest");
                        writer.WriteValue(object_deleted.Digest);

                        writer.WritePropertyName("objectId");
                        writer.WriteValue(object_deleted.ObjectID);

                        writer.WritePropertyName("version");
                        writer.WriteValue(object_deleted.Version);
                        break;
                    case ObjectResponseErrorType.DisplayError:
                        writer.WritePropertyName("error");
                        writer.WriteValue(((ObjectResponseErrorDisplayError)object_error.ObjectError).ObjectError);
                        break;
                    default:
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }

    public enum ObjectResponseErrorType
    {
        NotExist,
        DynamicFieldNotFound,
        Deleted,
        Unknown,
        DisplayError
    }

    public interface IObjectResponseError { }

    [JsonConverter(typeof(ObjectResponseErrorConverter))]
    public class ObjectResponseError : ReturnBase
    {
        public IObjectResponseError ObjectError { get; }

        public ObjectResponseErrorType Type { get; }

        public ObjectResponseError
        (
            IObjectResponseError object_error,
            ObjectResponseErrorType type
        )
        {
            this.ObjectError = object_error;
            this.Type = type;
        }

        public ObjectResponseError(SuiError error)
        {
            this.Error = error;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ObjectResponseError)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ObjectResponseError.", obj);

            ObjectResponseError other_object_error = (ObjectResponseError)obj;

            return
                this.Type == other_object_error.Type &&
                this.ObjectError.Equals(other_object_error.ObjectError);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectResponseErrorNotExist : ReturnBase, IObjectResponseError
    {
        public AccountAddress ObjectID { get; }

        public ObjectResponseErrorNotExist(AccountAddress object_id)
        {
            this.ObjectID = object_id;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ObjectResponseErrorNotExist)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ObjectResponseErrorNotExist.", obj);

            ObjectResponseErrorNotExist other_not_exist = (ObjectResponseErrorNotExist)obj;

            return this.ObjectID.Equals(other_not_exist.ObjectID);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectResponseErrorDynamicFieldNotFound : ReturnBase, IObjectResponseError
    {
        public AccountAddress ParentObjectID { get; }

        public ObjectResponseErrorDynamicFieldNotFound(AccountAddress parent_object_id)
        {
            this.ParentObjectID = parent_object_id;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ObjectResponseErrorDynamicFieldNotFound)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ObjectResponseErrorDynamicFieldNotFound.", obj);

            ObjectResponseErrorDynamicFieldNotFound other_dynamic_field_not_found = (ObjectResponseErrorDynamicFieldNotFound)obj;

            return this.ParentObjectID.Equals(other_dynamic_field_not_found.ParentObjectID);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectResponseErrorDeleted : ReturnBase, IObjectResponseError
    {
        public string Digest { get; }

        public AccountAddress ObjectID { get; }

        public BigInteger Version { get; }

        public ObjectResponseErrorDeleted
        (
            string digest,
            AccountAddress object_id,
            BigInteger version
        )
        {
            this.Digest = digest;
            this.ObjectID = object_id;
            this.Version = version;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ObjectResponseErrorDeleted)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ObjectResponseErrorDeleted.", obj);

            ObjectResponseErrorDeleted other_deleted_error = (ObjectResponseErrorDeleted)obj;

            return
                this.Digest == other_deleted_error.Digest &&
                this.ObjectID.Equals(other_deleted_error.ObjectID) &&
                this.Version.Equals(other_deleted_error.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectResponseErrorUnknown : ReturnBase, IObjectResponseError
    {
        public ObjectResponseErrorUnknown() { }

        public ObjectResponseErrorUnknown(SuiError error)
        {
            this.Error = error;
        }
    }

    public class ObjectResponseErrorDisplayError : ReturnBase, IObjectResponseError
    {
        public string ObjectError { get; }

        public ObjectResponseErrorDisplayError(string object_error)
        {
            this.ObjectError = object_error;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ObjectResponseErrorDisplayError)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ObjectResponseErrorDisplayError.", obj);

            ObjectResponseErrorDisplayError other_display_error = (ObjectResponseErrorDisplayError)obj;

            return this.ObjectError == other_display_error.ObjectError;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}