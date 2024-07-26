using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sui.Accounts;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    [JsonConverter(typeof(SuiOwnerConverter))]
    public class Owner : ReturnBase
    {
        public SuiOwnerType Type { get; }

        [JsonProperty("AddressOwner", NullValueHandling = NullValueHandling.Include)]
        public AccountAddress Address { get; }

        [JsonProperty("Shared", NullValueHandling = NullValueHandling.Include)]
        public SharedOwner Shared { get; }

        public Owner()
        {
            Type = SuiOwnerType.Immutable;
        }

        public Owner(SuiOwnerType type, AccountAddress address)
        {
            Type = type;
            Address = address;
        }

        public Owner(int initial_shared_version)
        {
            Type = SuiOwnerType.Shared;
            Shared = new SharedOwner(initial_shared_version);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Owner)
                this.SetError<bool, SuiError>(false, "Compared object is not an Owner.", obj);

            Owner other_owner = (Owner)obj;

            if (this.Type == SuiOwnerType.Immutable && other_owner.Type == SuiOwnerType.Immutable)
                return true;

            if (this.Address != null && other_owner.Address != null)
                return
                    this.Type == other_owner.Type &&
                    this.Address.Equals(other_owner.Address);

            if (this.Shared != null && other_owner.Shared != null)
                return
                    this.Type == other_owner.Type &&
                    this.Shared.Equals(other_owner.Shared);

            return false;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    [JsonObject]
    public class SharedOwner : ReturnBase
    {
        [JsonProperty("initial_shared_version")]
        public int? InitialSharedVersion { get; }

        public SharedOwner(int initial_shared_version)
        {
            this.InitialSharedVersion = initial_shared_version;
        }

        public override bool Equals(object obj)
        {
            if (obj is not SharedOwner)
                this.SetError<bool, SuiError>(false, "Compared object is not a SharedOwner.", obj);

            SharedOwner other_shared_owner = (SharedOwner)obj;

            return
                this.InitialSharedVersion == other_shared_owner.InitialSharedVersion;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}