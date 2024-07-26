namespace Sui.Rpc.Models
{
    /// <summary>
    /// Used for filtering results with the RPC client.
    /// </summary>
    public class SuiRpcFilter
    {
        /// <summary>
        /// An optional paging cursor. If provided, the query will start from
        /// the next item after the specified cursor. Default to start from the
        /// first item if not specified.
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Maximum item returned per page, default to
        /// [QUERY_MAX_RESULT_LIMIT] if not specified.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Query result ordering, default to false
        /// (ascending order, SortOrder.Ascending), oldest record first.
        /// </summary>
        public SortOrder? Order { get; set; }

        public SuiRpcFilter
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null
        )
        {
            this.Cursor = cursor;
            this.Limit = limit;
            this.Order = order;
        }
    }

    /// <summary>
    /// The query object used for object endpoints.
    /// </summary>
    public class ObjectQuery : SuiRpcFilter
    {
        /// <summary>
        /// The options for what the return object will send.
        /// </summary>
        public ObjectDataOptions ObjectDataOptions { get; set; }

        /// <summary>
        /// The filter options for querying object.
        /// </summary>
        public IObjectDataFilter ObjectDataFilter { get; set; }

        public ObjectQuery
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null,
            ObjectDataOptions object_data_options = null,
            IObjectDataFilter object_data_filter = null
        ) : base(cursor, limit, order)
        {
            this.ObjectDataOptions = object_data_options;
            this.ObjectDataFilter = object_data_filter;
        }
    }

    /// <summary>
    /// The query object used for event endpoints.
    /// </summary>
    public class EventQuery : SuiRpcFilter
    {
        /// <summary>
        /// The event query criteria. See Event filter documentation for examples.
        /// https://docs.sui.io/build/event_api#event-filters
        /// </summary>
        public IEventFilter EventFilter { get; set; }

        public EventQuery
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null,
            IEventFilter event_filter = null
        ) : base(cursor, limit, order)
        {
            this.EventFilter = event_filter;
        }
    }

    /// <summary>
    /// The query object used for any transaction block endpoints.
    /// </summary>
    public abstract class ITransactionBlockQuery : SuiRpcFilter
    {
        /// <summary>
        /// The options for what the return transaction block responses will send. 
        /// </summary>
        public TransactionBlockResponseOptions TransactionBlockResponseOptions { get; set; }

        public ITransactionBlockQuery
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null,
            TransactionBlockResponseOptions transaction_block_response_options = null
        ) : base(cursor, limit, order)
        {
            this.TransactionBlockResponseOptions = transaction_block_response_options;
        }
    }

    /// <summary>
    /// The query object used for transaction block execution endpoints.
    /// </summary>
    public class TransactionBlockExecutionQuery : ITransactionBlockQuery
    {
        /// <summary>
        /// The request type of the executed transaction block.
        /// </summary>
        public RequestType? RequestType { get; set; }

        public TransactionBlockExecutionQuery
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null,
            TransactionBlockResponseOptions transaction_block_response_options = null,
            RequestType? request_type = null
        ) : base(cursor, limit, order, transaction_block_response_options)
        {
            this.RequestType = request_type;
        }
    }

    /// <summary>
    /// The query object used for transaction block response endpoints.
    /// </summary>
    public class TransactionBlockResponseQueryInput : ITransactionBlockQuery
    {
        /// <summary>
        /// The filter options for querying transaction responses.
        /// </summary>
        public ITransactionFilter TransactionFilter { get; set; }

        public TransactionBlockResponseQueryInput
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null,
            TransactionBlockResponseOptions transaction_block_response_options = null,
            ITransactionFilter transaction_filter = null
        ) : base(cursor, limit, order, transaction_block_response_options)
        {
            this.TransactionFilter = transaction_filter;
        }
    }
}

