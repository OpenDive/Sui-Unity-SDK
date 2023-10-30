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
    }

    public class DynamicFieldNotFoundError : ObjectResponseError
    {
        public AccountAddress ParentObjectId { get; set; }

        public DynamicFieldNotFoundError(AccountAddress parentObjectId)
        {
            this.ParentObjectId = parentObjectId;
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
    }
}