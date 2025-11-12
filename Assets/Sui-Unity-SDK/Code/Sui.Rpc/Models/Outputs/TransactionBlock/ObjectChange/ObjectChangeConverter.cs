using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    public class ObjectChangeConverter : JsonConverter
    {
        public readonly string ObjectChangeTypeProperty = "type";

        public readonly string ObjectChangePackageIDProperty = "packageId";
        public readonly string ObjectChangeVersionProperty = "version";
        public readonly string ObjectChangeDigestProperty = "digest";
        public readonly string ObjectChangeModulesProperty = "modules";
        public readonly string ObjectChangeSenderProperty = "sender";
        public readonly string ObjectChangeRecipientProperty = "recipient";
        public readonly string ObjectChangeObjectTypeProperty = "objectType";
        public readonly string ObjectChangeObjectIDProperty = "objectId";
        public readonly string ObjectChangeOwnerProperty = "owner";
        public readonly string ObjectChangePreviousVersionProperty = "previousVersion";

        public override bool CanConvert(Type objectType) => objectType == typeof(ObjectChange);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject object_change = JObject.Load(reader);

                switch (object_change[this.ObjectChangeTypeProperty].Value<string>())
                {
                    case "published":
                        return new ObjectChange
                        (
                            ObjectChangeType.Published,
                            new ObjectChangePublished
                            (
                                (AccountAddress)object_change[this.ObjectChangePackageIDProperty].ToObject(typeof(AccountAddress)),
                                BigInteger.Parse(object_change[this.ObjectChangeVersionProperty].Value<string>()),
                                object_change[this.ObjectChangeDigestProperty].Value<string>(),
                                ((JArray)object_change[this.ObjectChangeModulesProperty]).Select(module => module.Value<string>()).ToList()
                            )
                        );
                    case "transferred":
                        return new ObjectChange
                        (
                            ObjectChangeType.Transferred,
                            new ObjectChangeTransferred
                            (
                                (AccountAddress)object_change[this.ObjectChangeSenderProperty].ToObject(typeof(AccountAddress)),
                                (Owner)object_change[this.ObjectChangeRecipientProperty].ToObject(typeof(Owner)),
                                object_change[this.ObjectChangeObjectTypeProperty].Value<string>(),
                                (AccountAddress)object_change[this.ObjectChangeObjectIDProperty].ToObject(typeof(AccountAddress)),
                                BigInteger.Parse(object_change[this.ObjectChangeVersionProperty].Value<string>()),
                                object_change[this.ObjectChangeDigestProperty].Value<string>()
                            )
                        );
                    case "mutated":
                        return new ObjectChange
                        (
                            ObjectChangeType.Mutated,
                            new ObjectChangeMutated
                            (
                                (AccountAddress)object_change[this.ObjectChangeSenderProperty].ToObject(typeof(AccountAddress)),
                                (Owner)object_change[this.ObjectChangeOwnerProperty].ToObject(typeof(Owner)),
                                object_change[this.ObjectChangeObjectTypeProperty].Value<string>(),
                                BigInteger.Parse(object_change[this.ObjectChangePreviousVersionProperty].Value<string>()),
                                BigInteger.Parse(object_change[this.ObjectChangeVersionProperty].Value<string>()),
                                object_change[this.ObjectChangeDigestProperty].Value<string>()
                            )
                        );
                    case "deleted":
                        return new ObjectChange
                        (
                            ObjectChangeType.Deleted,
                            new ObjectChangeDeleted
                            (
                                (AccountAddress)object_change[this.ObjectChangeSenderProperty].ToObject(typeof(AccountAddress)),
                                object_change[this.ObjectChangeObjectTypeProperty].Value<string>(),
                                (AccountAddress)object_change[this.ObjectChangeObjectIDProperty].ToObject(typeof(AccountAddress)),
                                BigInteger.Parse(object_change[this.ObjectChangeVersionProperty].Value<string>())
                            )
                        );
                    case "wrapped":
                        return new ObjectChange
                        (
                            ObjectChangeType.Wrapped,
                            new ObjectChangeWrapped
                            (
                                (AccountAddress)object_change[this.ObjectChangeSenderProperty].ToObject(typeof(AccountAddress)),
                                object_change[this.ObjectChangeObjectTypeProperty].Value<string>(),
                                (AccountAddress)object_change[this.ObjectChangeObjectIDProperty].ToObject(typeof(AccountAddress)),
                                BigInteger.Parse(object_change[this.ObjectChangeVersionProperty].Value<string>())
                            )
                        );
                    case "created":
                        return new ObjectChange
                        (
                            ObjectChangeType.Created,
                            new ObjectChangeCreated
                            (
                                (AccountAddress)object_change[this.ObjectChangeSenderProperty].ToObject(typeof(AccountAddress)),
                                (Owner)object_change[this.ObjectChangeOwnerProperty].ToObject(typeof(Owner)),
                                object_change[this.ObjectChangeObjectTypeProperty].Value<string>(),
                                (AccountAddress)object_change[this.ObjectChangeObjectIDProperty].ToObject(typeof(AccountAddress)),
                                BigInteger.Parse(object_change[this.ObjectChangeVersionProperty].Value<string>()),
                                object_change[this.ObjectChangeDigestProperty].Value<string>()
                            )
                        );
                    default:
                        return new ObjectChange(new SuiError(0, $"Unable to convert {object_change[this.ObjectChangeTypeProperty].Value<string>()} to ObjectChangeType.", object_change));
                }
            }

            return new ObjectChange(new SuiError(0, "Unable to convert JSON to ObjectChange.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartObject();
                ObjectChange object_change = (ObjectChange)value;

                writer.WritePropertyName(this.ObjectChangeTypeProperty);
                writer.WriteValue(object_change.Type.ToString());

                switch (object_change.Type)
                {
                    case ObjectChangeType.Published:
                        ObjectChangePublished published = (ObjectChangePublished)object_change.Change;

                        writer.WritePropertyName(this.ObjectChangePackageIDProperty);
                        writer.WriteValue(published.PackageID.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeVersionProperty);
                        writer.WriteValue(published.Version.ToString());

                        writer.WritePropertyName(this.ObjectChangeDigestProperty);
                        writer.WriteValue(published.Digest);

                        writer.WritePropertyName(this.ObjectChangeModulesProperty);
                        writer.WriteValue(published.Modules);

                        break;
                    case ObjectChangeType.Transferred:
                        ObjectChangeTransferred transferred = (ObjectChangeTransferred)object_change.Change;

                        writer.WritePropertyName(this.ObjectChangeSenderProperty);
                        writer.WriteValue(transferred.Sender.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeRecipientProperty);
                        writer.WriteValue(transferred.Recipient);

                        writer.WritePropertyName(this.ObjectChangeObjectTypeProperty);
                        writer.WriteValue(transferred.ObjectType);

                        writer.WritePropertyName(this.ObjectChangeObjectIDProperty);
                        writer.WriteValue(transferred.ObjectID.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeVersionProperty);
                        writer.WriteValue(transferred.Version.ToString());

                        writer.WritePropertyName(this.ObjectChangeDigestProperty);
                        writer.WriteValue(transferred.Digest);

                        break;
                    case ObjectChangeType.Mutated:
                        ObjectChangeMutated mutated = (ObjectChangeMutated)object_change.Change;

                        writer.WritePropertyName(this.ObjectChangeSenderProperty);
                        writer.WriteValue(mutated.Sender.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeOwnerProperty);
                        writer.WriteValue(mutated.Owner);

                        writer.WritePropertyName(this.ObjectChangeObjectTypeProperty);
                        writer.WriteValue(mutated.ObjectType);

                        writer.WritePropertyName(this.ObjectChangeVersionProperty);
                        writer.WriteValue(mutated.Version.ToString());

                        writer.WritePropertyName(this.ObjectChangePreviousVersionProperty);
                        writer.WriteValue(mutated.PreviousVersion.ToString());

                        writer.WritePropertyName(this.ObjectChangeDigestProperty);
                        writer.WriteValue(mutated.Digest);

                        break;
                    case ObjectChangeType.Deleted:
                        ObjectChangeDeleted deleted = (ObjectChangeDeleted)object_change.Change;

                        writer.WritePropertyName(this.ObjectChangeSenderProperty);
                        writer.WriteValue(deleted.Sender.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeObjectTypeProperty);
                        writer.WriteValue(deleted.ObjectType);

                        writer.WritePropertyName(this.ObjectChangeObjectIDProperty);
                        writer.WriteValue(deleted.ObjectID.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeVersionProperty);
                        writer.WriteValue(deleted.Version.ToString());

                        break;
                    case ObjectChangeType.Wrapped:
                        ObjectChangeWrapped wrapped = (ObjectChangeWrapped)object_change.Change;

                        writer.WritePropertyName(this.ObjectChangeSenderProperty);
                        writer.WriteValue(wrapped.Sender.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeObjectTypeProperty);
                        writer.WriteValue(wrapped.ObjectType);

                        writer.WritePropertyName(this.ObjectChangeObjectIDProperty);
                        writer.WriteValue(wrapped.ObjectID.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeVersionProperty);
                        writer.WriteValue(wrapped.Version.ToString());

                        break;
                    case ObjectChangeType.Created:
                        ObjectChangeCreated created = (ObjectChangeCreated)object_change.Change;

                        writer.WritePropertyName(this.ObjectChangeSenderProperty);
                        writer.WriteValue(created.Sender.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeOwnerProperty);
                        writer.WriteValue(created.Owner);

                        writer.WritePropertyName(this.ObjectChangeObjectTypeProperty);
                        writer.WriteValue(created.ObjectType);

                        writer.WritePropertyName(this.ObjectChangeObjectIDProperty);
                        writer.WriteValue(created.ObjectID.KeyHex);

                        writer.WritePropertyName(this.ObjectChangeVersionProperty);
                        writer.WriteValue(created.Version.ToString());

                        writer.WritePropertyName(this.ObjectChangeDigestProperty);
                        writer.WriteValue(created.Digest);

                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}