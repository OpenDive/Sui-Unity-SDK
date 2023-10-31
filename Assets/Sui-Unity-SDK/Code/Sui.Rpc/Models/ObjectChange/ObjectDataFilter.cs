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

	public interface IObjectDataFilter
	{
	}

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
	}

    public class ObjectDataFilterMatchAny : IObjectDataFilter
    {
        [JsonProperty("MatchAny")]
        public IObjectDataFilter[] MatchAny { get; set; }
    }

    public class ObjectDataFilterMatchNone : IObjectDataFilter
    {
        [JsonProperty("MatchNone")]
        public IObjectDataFilter[] MatchNone { get; set; }
    }

    public class ObjectDataFilterPackage : IObjectDataFilter
    {
        [JsonProperty("Package")]
        public string Package { get; set; }
    }

    public class ObjectDataFilterMoveModule: IObjectDataFilter
    {
        [JsonProperty("MoveModule")]
        public FilterMoveModule MoveModule { get; set; }
    }

    public class ObjectDataFilterStructType : IObjectDataFilter
    {
        [JsonProperty("StructType")]
        public string StructType { get; set; }
    }

    public class ObjectDataFilterAddressOwner : IObjectDataFilter
    {
        [JsonProperty("AddressOwner")]
        public string AddressOwner { get; set; }
    }

    public class ObjectDataFilterObjectOwner : IObjectDataFilter
    {
        [JsonProperty("ObjectOwner")]
        public string ObjectOwner { get; set; }
    }

    public class ObjectDataFilterObjectId : IObjectDataFilter
    {
        [JsonProperty("ObjectId")]
        public string ObjectId { get; set; }
    }

    public class ObjectDataFilterObjectIds : IObjectDataFilter
    {
        [JsonProperty("ObjectIds")]
        public string[] ObjectIds { get; set; }
    }

    public class ObjectDataFilterVersion : IObjectDataFilter
    {
        [JsonProperty("Version")]
        public string Version { get; set; }
    }
}