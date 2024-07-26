using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    public enum ObjectChangeType
    {
        Published,
        Transferred,
        Mutated,
        Deleted,
        Wrapped,
        Created
    }

    public abstract class IObjectChange : ReturnBase
    {
        public BigInteger Version { get; internal set; }

        public IObjectChange(BigInteger version)
        {
            this.Version = version;
        }
    }

    [JsonConverter(typeof(ObjectChangeConverter))]
    public class ObjectChange : ReturnBase
    {
        public ObjectChangeType Type { get; internal set; }

        public IObjectChange Change { get; internal set; }

        public ObjectChange
        (
            ObjectChangeType type,
            IObjectChange change
        )
        {
            this.Type = type;
            this.Change = change;
        }

        public ObjectChange(SuiError error)
        {
            this.Error = error;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChange)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChange.", other);

            ObjectChange other_object_change = (ObjectChange)other;

            return
                this.Type == other_object_change.Type &&
                this.Change.Equals(other_object_change.Change);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectChangePublished : IObjectChange
    {
        public AccountAddress PackageID { get; internal set; }

        public string Digest { get; internal set; }

        public List<string> Modules { get; internal set; }

        public ObjectChangePublished
        (
            AccountAddress package_id,
            BigInteger version,
            string digest,
            List<string> modules
        ) : base(version)
        {
            this.PackageID = package_id;
            this.Digest = digest;
            this.Modules = modules;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangePublished)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangePublished.", other);

            ObjectChangePublished other_object_change_published = (ObjectChangePublished)other;

            return
                this.PackageID.Equals(other_object_change_published.PackageID) &&
                this.Version.Equals(other_object_change_published.Version) &&
                this.Digest == other_object_change_published.Digest &&
                this.Modules.SequenceEqual(other_object_change_published.Modules);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectChangeTransferred : IObjectChange
    {
        public AccountAddress Sender { get; internal set; }

        public Owner Recipient { get; internal set; }

        public string ObjectType { get; internal set; }

        public AccountAddress ObjectID { get; internal set; }

        public string Digest { get; internal set; }

        public ObjectChangeTransferred
        (
            AccountAddress sender,
            Owner recipient,
            string object_type,
            AccountAddress object_id,
            BigInteger version,
            string digest
        ) : base(version)
        {
            this.Sender = sender;
            this.Recipient = recipient;
            this.ObjectType = object_type;
            this.ObjectID = object_id;
            this.Digest = digest;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangeTransferred)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangeTransferred.", other);

            ObjectChangeTransferred other_object_change_transferred = (ObjectChangeTransferred)other;

            return
                this.Sender.Equals(other_object_change_transferred.Sender) &&
                this.Recipient.Equals(other_object_change_transferred.Recipient) &&
                this.ObjectType == other_object_change_transferred.ObjectType &&
                this.ObjectID.Equals(other_object_change_transferred.ObjectID) &&
                this.Digest == other_object_change_transferred.Digest &&
                this.Version.Equals(other_object_change_transferred.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectChangeMutated : IObjectChange
    {
        public AccountAddress Sender { get; internal set; }

        public Owner Owner { get; internal set; }

        public string ObjectType { get; internal set; }

        public BigInteger PreviousVersion { get; internal set; }

        public string Digest { get; internal set; }

        public ObjectChangeMutated
        (
            AccountAddress sender,
            Owner owner,
            string object_type,
            BigInteger previous_version,
            BigInteger version,
            string digest
        ) : base(version)
        {
            this.Sender = sender;
            this.Owner = owner;
            this.ObjectType = object_type;
            this.PreviousVersion = previous_version;
            this.Digest = digest;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangeMutated)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangeMutated.", other);

            ObjectChangeMutated other_object_change_mutated = (ObjectChangeMutated)other;

            return
                this.Sender.Equals(other_object_change_mutated.Sender) &&
                this.Owner.Equals(other_object_change_mutated.Owner) &&
                this.ObjectType == other_object_change_mutated.ObjectType &&
                this.PreviousVersion.Equals(other_object_change_mutated.PreviousVersion) &&
                this.Digest == other_object_change_mutated.Digest &&
                this.Version.Equals(other_object_change_mutated.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectChangeDeleted : IObjectChange
    {
        public AccountAddress Sender { get; internal set; }

        public string ObjectType { get; internal set; }

        public AccountAddress ObjectID { get; internal set; }

        public ObjectChangeDeleted
        (
            AccountAddress sender,
            string object_type,
            AccountAddress object_id,
            BigInteger version
        ) : base(version)
        {
            this.Sender = sender;
            this.ObjectType = object_type;
            this.ObjectID = object_id;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangeDeleted)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangeDeleted.", other);

            ObjectChangeDeleted other_object_change_deleted = (ObjectChangeDeleted)other;

            return
                this.Sender.Equals(other_object_change_deleted.Sender) &&
                this.ObjectType == other_object_change_deleted.ObjectType &&
                this.ObjectID.Equals(other_object_change_deleted.ObjectID) &&
                this.Version.Equals(other_object_change_deleted.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectChangeWrapped : IObjectChange
    {
        public AccountAddress Sender { get; internal set; }

        public string ObjectType { get; internal set; }

        public AccountAddress ObjectID { get; internal set; }

        public ObjectChangeWrapped
        (
            AccountAddress sender,
            string object_type,
            AccountAddress object_id,
            BigInteger version
        ) : base(version)
        {
            this.Sender = sender;
            this.ObjectType = object_type;
            this.ObjectID = object_id;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangeWrapped)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangeWrapped.", other);

            ObjectChangeWrapped other_object_change_wrapped = (ObjectChangeWrapped)other;

            return
                this.Sender.Equals(other_object_change_wrapped.Sender) &&
                this.ObjectType == other_object_change_wrapped.ObjectType &&
                this.ObjectID.Equals(other_object_change_wrapped.ObjectID) &&
                this.Version.Equals(other_object_change_wrapped.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class ObjectChangeCreated : IObjectChange
    {
        public AccountAddress Sender { get; internal set; }

        public Owner Owner { get; internal set; }

        public string ObjectType { get; internal set; }

        public AccountAddress ObjectID { get; internal set; }

        public string Digest { get; internal set; }

        public ObjectChangeCreated
        (
            AccountAddress sender,
            Owner owner,
            string object_type,
            AccountAddress object_id,
            BigInteger version,
            string digest
        ) : base(version)
        {
            this.Sender = sender;
            this.Owner = owner;
            this.ObjectType = object_type;
            this.ObjectID = object_id;
            this.Digest = digest;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangeCreated)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangeCreated.", other);

            ObjectChangeCreated other_object_change_created = (ObjectChangeCreated)other;

            return
                this.Sender.Equals(other_object_change_created.Sender) &&
                this.Owner.Equals(other_object_change_created.Owner) &&
                this.ObjectType == other_object_change_created.ObjectType &&
                this.ObjectID.Equals(other_object_change_created.ObjectID) &&
                this.Digest == other_object_change_created.Digest &&
                this.Version.Equals(other_object_change_created.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}