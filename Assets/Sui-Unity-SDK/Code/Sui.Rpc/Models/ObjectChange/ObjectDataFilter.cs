using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    public class ObjectResponseQuery
    {
        [JsonProperty("filter", Required = Required.Default)]
        public IObjectDataFilter Filter { get; set; }

        [JsonProperty("options", Required = Required.Default)]
        public ObjectDataOptions Options { get; set; }

        public ObjectResponseQuery(IObjectDataFilter filter, ObjectDataOptions options)
        {
            this.Filter = filter;
            this.Options = options;
        }
    }

	public interface IObjectDataFilter { }

    public class FilterMoveModule
    {
        [JsonProperty("package")]
        public string Package { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }
    }

	public class ObjectDataFilterMatchAll: IObjectDataFilter
    {
        [JsonProperty("MatchAll")]
        public IObjectDataFilter[] MatchAll { get; set; }

        public ObjectDataFilterMatchAll(IObjectDataFilter[] match_all)
        {
            this.MatchAll = match_all;
        }
    }

    public class ObjectDataFilterMatchAny : IObjectDataFilter
    {
        [JsonProperty("MatchAny")]
        public IObjectDataFilter[] MatchAny { get; set; }

        public ObjectDataFilterMatchAny(IObjectDataFilter[] match_any)
        {
            this.MatchAny = match_any;
        }
    }

    public class ObjectDataFilterMatchNone : IObjectDataFilter
    {
        [JsonProperty("MatchNone")]
        public IObjectDataFilter[] MatchNone { get; set; }

        public ObjectDataFilterMatchNone(IObjectDataFilter[] match_none)
        {
            this.MatchNone = match_none;
        }
    }

    public class ObjectDataFilterPackage : IObjectDataFilter
    {
        [JsonProperty("Package")]
        public string Package { get; set; }

        public ObjectDataFilterPackage(string package)
        {
            this.Package = package;
        }
    }

    public class ObjectDataFilterMoveModule: IObjectDataFilter
    {
        [JsonProperty("MoveModule")]
        public FilterMoveModule MoveModule { get; set; }

        public ObjectDataFilterMoveModule(FilterMoveModule move_module)
        {
            this.MoveModule = move_module;
        }
    }

    public class ObjectDataFilterStructType : IObjectDataFilter
    {
        [JsonProperty("StructType")]
        public string StructType { get; set; }

        public ObjectDataFilterStructType(string struct_type)
        {
            this.StructType = struct_type;
        }
    }

    public class ObjectDataFilterAddressOwner : IObjectDataFilter
    {
        [JsonProperty("AddressOwner")]
        public string AddressOwner { get; set; }

        public ObjectDataFilterAddressOwner(string address_owner)
        {
            this.AddressOwner = address_owner;
        }
    }

    public class ObjectDataFilterObjectOwner : IObjectDataFilter
    {
        [JsonProperty("ObjectOwner")]
        public string ObjectOwner { get; set; }

        public ObjectDataFilterObjectOwner(string object_owner)
        {
            this.ObjectOwner = object_owner;
        }
    }

    public class ObjectDataFilterObjectId : IObjectDataFilter
    {
        [JsonProperty("ObjectId")]
        public string ObjectId { get; set; }

        public ObjectDataFilterObjectId(string object_id)
        {
            this.ObjectId = object_id;
        }
    }

    public class ObjectDataFilterObjectIds : IObjectDataFilter
    {
        [JsonProperty("ObjectIds")]
        public string[] ObjectIds { get; set; }

        public ObjectDataFilterObjectIds(string[] object_ids)
        {
            this.ObjectIds = object_ids;
        }
    }

    public class ObjectDataFilterVersion : IObjectDataFilter
    {
        [JsonProperty("Version")]
        public string Version { get; set; }

        public ObjectDataFilterVersion(string version)
        {
            this.Version = version;
        }
    }
}