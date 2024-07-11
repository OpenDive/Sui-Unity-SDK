using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sui.Rpc.Models
{
    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public interface IEventFilter { }

    public class AnyEventFilter : IEventFilter
    {
        [JsonProperty("Any")]
        public IEventFilter[] Any { get; set; }

        public AnyEventFilter(IEventFilter[] any)
        {
            this.Any = any;
        }
    }

    public class AllEventFilter : IEventFilter
    {
        [JsonProperty("All")]
        public IEventFilter[] All { get; set; }

        public AllEventFilter(IEventFilter[] all)
        {
            this.All = all;
        }
    }

    public class AndEventFilter : IEventFilter
    {
        [JsonProperty("And")]
        public IEventFilter[] And { get; set; }

        public AndEventFilter(IEventFilter[] and)
        {
            this.And = and;
        }
    }

    public class OrEventFilter : IEventFilter
    {
        [JsonProperty("Or")]
        public IEventFilter[] Or { get; set; }

        public OrEventFilter(IEventFilter[] or)
        {
            this.Or = or;
        }
    }

    public class SenderEventFilter: IEventFilter
    {
        [JsonProperty("Sender")]
        public string Sender { get; set; }

        public SenderEventFilter(string sender)
        {
            this.Sender = sender;
        }
    }

    public class TransactionEventFilter : IEventFilter
    {
        [JsonProperty("Transaction")]
        public string Transaction { get; set; }

        public TransactionEventFilter(string transaction)
        {
            this.Transaction = transaction;
        }
    }

    public class PackageEventFilter : IEventFilter
    {
        [JsonProperty("Package")]
        public string Package { get; set; }

        public PackageEventFilter(string package)
        {
            this.Package = package;
        }
    }

    public class MoveModuleEventFilter: IEventFilter
    {
        [JsonProperty("MoveModule")]
        public FilterMoveModule MoveModule { get; set; }

        public MoveModuleEventFilter(FilterMoveModule move_module)
        {
            this.MoveModule = move_module;
        }
    }

    public class MoveEventTypeEventFilter : IEventFilter
    {
        [JsonProperty("MoveEventType")]
        public string MoveEventType { get; set; }

        public MoveEventTypeEventFilter(string move_event_type)
        {
            this.MoveEventType = move_event_type;
        }
    }

    public class MoveEventModuleEventFilter : IEventFilter
    {
        [JsonProperty("MoveEventModule")]
        public FilterMoveModule MoveEventModule { get; set; }

        public MoveEventModuleEventFilter(FilterMoveModule move_event_module)
        {
            this.MoveEventModule = move_event_module;
        }
    }

    public class MoveEventFieldEventFilter: IEventFilter
    {
        [JsonProperty("MoveEventField")]
        public MoveEventField MoveEventField { get; set; }

        public MoveEventFieldEventFilter(MoveEventField move_event_field)
        {
            this.MoveEventField = move_event_field;
        }
    }

    public class TimeRangeEventFilter: IEventFilter
    {
        [JsonProperty("TimeRange")]
        public TimeRange TimeRange { get; set; }

        public TimeRangeEventFilter(TimeRange time_range)
        {
            this.TimeRange = time_range;
        }
    }

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

    public class MoveEventField
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("value")]
        public JObject Value { get; set; }

        public MoveEventField(string path, JObject value)
        {
            this.Path = path;
            this.Value = value;
        }
    }

    public class TimeRange
    {
        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        public TimeRange(string endTime, string startTime)
        {
            this.EndTime = endTime;
            this.StartTime = startTime;
        }
    }

    public class FilterMoveModule
    {
        [JsonProperty("package")]
        public string Package { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }

        public FilterMoveModule(string package, string module)
        {
            this.Module = module;
            this.Package = package;
        }
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