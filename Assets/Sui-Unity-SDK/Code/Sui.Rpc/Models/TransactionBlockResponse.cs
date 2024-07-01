
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Response for `DryTransactionBlock` and other transaction related queries.
    /// <code>
    /// {
    ///     "jsonrpc": "2.0",
    ///     "result": {
    ///         "digest": "DNtx7EmGqSywGbnSC1CKoqmBFEXGvApXpRVt6bU855xP",
    ///         "transaction": {
    ///             "data": {
    ///                 "messageVersion": "v1",
    ///                 "transaction": {
    ///                     "kind": "ProgrammableTransaction",
    ///                     "inputs": [
    ///                         {
    ///                             "type": "pure",
    ///                             "valueType": "address",
    ///                             "value": "0x7ba91ddc7e717cf708c937060f04048736ec33fb1746d999a5e58cd5c677ed80"
    ///                         },
    ///                         {
    ///                             "type": "object",
    ///                             "objectType": "immOrOwnedObject",
    ///                             "objectId": "0x4f82f1c8587b98d64c00bfb46c3843bd8bf6ccfa7c65a86138698cd1fdcac3dc",
    ///                             "version": "2",
    ///                             "digest": "Cv7n2YaM7Am1ssZGu4khsFkcKHnpgVhwFCSs4kLjrtLW"
    ///                         }
    ///                     ],
    ///                     "transactions": [
    ///                         {
    ///                             "TransferObjects": [
    ///                                 [
    ///                                     {
    ///                                         "Input": 1
    ///                                     }
    ///                                 ],
    ///                                 {
    ///                                     "Input": 0
    ///                                 }
    ///                             ]
    ///                         }
    ///                     ]
    ///                 },
    ///                 "sender": "0x61fbb5b4f342a40bdbf87fe4a946b9e38d18cf8ffc7b0000b975175c7b6a9576",
    ///                 "gasData": {
    ///                     "payment": [
    ///                         {
    ///                             "objectId": "0xe8d8c7ce863f313da3dbd92a83ef26d128b88fe66bf26e0e0d09cdaf727d1d84",
    ///                             "version": 2,
    ///                             "digest": "EnRQXe1hDGAJCFyF2ds2GmPHdvf9V6yxf24LisEsDkYt"
    ///                         }
    ///                     ],
    ///                     "owner": "0x61fbb5b4f342a40bdbf87fe4a946b9e38d18cf8ffc7b0000b975175c7b6a9576",
    ///                     "price": "10",
    ///                     "budget": "100000"
    ///                 }
    ///             },
    ///             "txSignatures": [
    ///                 "AG+AHZMT7BZWQVagaGfENXyiFQ2nYRkG4XdnwqwToeJEmZ4J1IxKw0xKzTATGiUzFedY/nxKVuHikFibNlZ3wg9Dij1TvBYKLcfLNo8fq6GASb9yfo6uvuwNUBGkTf54wQ=="
    ///             ]
    ///         },
    ///         "rawTransaction": "AQAAAAAAAgAge6kd3H5xfPcIyTcGDwQEhzbsM/sXRtmZpeWM1cZ37YABAE+C8chYe5jWTAC/tGw4Q72L9sz6fGWoYThpjNH9ysPcAgAAAAAAAAAgsQwARuKTwIzsD0B4PZrEv0q7TX+CBkf9hdGRg97nx/8BAQEBAQABAABh+7W080KkC9v4f+SpRrnjjRjPj/x7AAC5dRdce2qVdgHo2MfOhj8xPaPb2SqD7ybRKLiP5mvybg4NCc2vcn0dhAIAAAAAAAAAIMzKqXoCzDqYBBZw4WCdtxTphYyW6eRphV8c87/fl+hlYfu1tPNCpAvb+H/kqUa5440Yz4/8ewAAuXUXXHtqlXYKAAAAAAAAAKCGAQAAAAAAAAFhAG+AHZMT7BZWQVagaGfENXyiFQ2nYRkG4XdnwqwToeJEmZ4J1IxKw0xKzTATGiUzFedY/nxKVuHikFibNlZ3wg9Dij1TvBYKLcfLNo8fq6GASb9yfo6uvuwNUBGkTf54wQ==",
    ///         "effects": {
    ///             "messageVersion": "v1",
    ///             "status": {
    ///                 "status": "success"
    ///             },
    ///             "executedEpoch": "0",
    ///             "gasUsed": {
    ///                 "computationCost": "100",
    ///                 "storageCost": "100",
    ///                 "storageRebate": "10",
    ///                 "nonRefundableStorageFee": "0"
    ///             },
    ///             "transactionDigest": "8UExPV121BEfWkbymSPDYhh23rVNh3MSWtC5juJ9JGMJ",
    ///             "mutated": [
    ///                 {
    ///                     "owner": {
    ///                         "AddressOwner": "0x61fbb5b4f342a40bdbf87fe4a946b9e38d18cf8ffc7b0000b975175c7b6a9576"
    ///                     },
    ///                     "reference": {
    ///                         "objectId": "0xe8d8c7ce863f313da3dbd92a83ef26d128b88fe66bf26e0e0d09cdaf727d1d84",
    ///                         "version": 2,
    ///                         "digest": "EnRQXe1hDGAJCFyF2ds2GmPHdvf9V6yxf24LisEsDkYt"
    ///                     }
    ///                 },
    ///                 {
    ///                     "owner": {
    ///                         "AddressOwner": "0x7ba91ddc7e717cf708c937060f04048736ec33fb1746d999a5e58cd5c677ed80"
    ///                     },
    ///                     "reference": {
    ///                         "objectId": "0x4f82f1c8587b98d64c00bfb46c3843bd8bf6ccfa7c65a86138698cd1fdcac3dc",
    ///                         "version": 2,
    ///                         "digest": "Cv7n2YaM7Am1ssZGu4khsFkcKHnpgVhwFCSs4kLjrtLW"
    ///                     }
    ///                 }
    ///             ],
    ///             "gasObject": {
    ///                 "owner": {
    ///                     "ObjectOwner": "0x61fbb5b4f342a40bdbf87fe4a946b9e38d18cf8ffc7b0000b975175c7b6a9576"
    ///                 },
    ///                 "reference": {
    ///                     "objectId": "0xe8d8c7ce863f313da3dbd92a83ef26d128b88fe66bf26e0e0d09cdaf727d1d84",
    ///                     "version": 2,
    ///                     "digest": "EnRQXe1hDGAJCFyF2ds2GmPHdvf9V6yxf24LisEsDkYt"
    ///                 }
    ///             },
    ///             "eventsDigest": "55TNn3v5vpuXjQfjqamw76P9GZD522pumo4NuT7RYeFB"
    ///         },
    ///         "objectChanges": [
    ///             {
    ///                 "type": "transferred",
    ///                 "sender": "0x61fbb5b4f342a40bdbf87fe4a946b9e38d18cf8ffc7b0000b975175c7b6a9576",
    ///                 "recipient": {
    ///                     "AddressOwner": "0x7ba91ddc7e717cf708c937060f04048736ec33fb1746d999a5e58cd5c677ed80"
    ///                 },
    ///                 "objectType": "0x2::example::Object",
    ///                 "objectId": "0x4f82f1c8587b98d64c00bfb46c3843bd8bf6ccfa7c65a86138698cd1fdcac3dc",
    ///                 "version": "2",
    ///                 "digest": "B3xLC8EbyvTxy5pgiwTNUzHLa6kS7uwD6sZdErKB8F8f"
    ///             }
    ///         ]
    ///     }
    /// }
    ///
    /// </code>
    /// </summary>
    [JsonObject]
    public class TransactionBlockResponse
    {
        [JsonProperty("effects")]
        public TransactionBlockEffects Effects { get; set; }

        [JsonProperty("events")]
        public List<SuiEvent> Events { get; set; }

        [JsonProperty("objectChanges")]
        public List<ObjectChange> ObjectChanges { get; set; }

        [JsonProperty("balanceChange")]
        public List<BalanceChange> BalanceChanges { get; set; }

        [JsonProperty("timestampMs", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? TimestampMs { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        // TODO: Complete deserialization

        //[JsonProperty("transaction")]
        //public TransactionBlock Transaction { get; set; }

        //[JsonProperty("rawTransaction")]
        //public string RawTransaction { get; set; }

        // effects : {}

        // objectChanges: []
    }

    [JsonObject]
    public class DevInspectResponse
    {
        [JsonProperty("effects")]
        public TransactionBlockEffects Effects { get; set; }

        [JsonProperty("events")]
        public List<SuiEvent> Events { get; set; }

        [JsonProperty("error", Required = Required.Default)]
        public string Error { get; set; }

        // TODO: Implement ExecutionResultType value
    }
}