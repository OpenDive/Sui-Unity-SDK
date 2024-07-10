using System;
using System.Numerics;
using Newtonsoft.Json;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    [JsonConverter(typeof(ObjectResponseErrorJsonConverter))]
    public abstract class ObjectResponseError
    {
        public string Code { get; set; }
    }

    public class NotExistsError : ObjectResponseError
    {
        public AccountAddress ObjectId { get; set; }

        public NotExistsError(AccountAddress objectId)
        {
            this.ObjectId = objectId;
        }

        public override bool Equals(object obj)
        {
            if (obj is not NotExistsError)
                throw new NotImplementedException();

            NotExistsError other_non_exists_error = (NotExistsError)obj;

            return this.ObjectId.Equals(other_non_exists_error.ObjectId);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class DynamicFieldNotFoundError : ObjectResponseError
    {
        public AccountAddress ParentObjectId { get; set; }

        public DynamicFieldNotFoundError(AccountAddress parentObjectId)
        {
            this.ParentObjectId = parentObjectId;
        }

        public override bool Equals(object obj)
        {
            if (obj is not DynamicFieldNotFoundError)
                throw new NotImplementedException();

            DynamicFieldNotFoundError other_dynamic_field_not_found_error = (DynamicFieldNotFoundError)obj;

            return this.ParentObjectId.Equals(other_dynamic_field_not_found_error.ParentObjectId);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class DeletedError : ObjectResponseError
    {
        public string Digest { get; set; }
        public AccountAddress ObjectId { get; set; }
        public BigInteger Version { get; set; }

        public DeletedError(string digest, AccountAddress objectId, string version)
        {
            this.Digest = digest;
            this.ObjectId = objectId;
            this.Version = BigInteger.Parse(version);
        }

        public override bool Equals(object obj)
        {
            if (obj is not DeletedError)
                throw new NotImplementedException();

            DeletedError other_deleted_error = (DeletedError)obj;

            return
                this.Digest == other_deleted_error.Digest &&
                this.ObjectId.Equals(other_deleted_error.ObjectId) &&
                this.Version.Equals(other_deleted_error.Version);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class UnknownError : ObjectResponseError
    {
    }

    public class DisplayError : ObjectResponseError
    {
        public string Error { get; set; }

        public DisplayError(string error)
        {
            this.Error = error;
        }

        public override bool Equals(object obj)
        {
            if (obj is not DisplayError)
                throw new NotImplementedException();

            DisplayError other_display_error = (DisplayError)obj;

            return this.Error == other_display_error.Error;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}