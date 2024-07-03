using System.Numerics;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Types;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class PaginatedObjectsResponse
    {
        [JsonProperty("data")]
        public ObjectDataResponse[] Data { get; set; }

        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; set; }

        [JsonProperty("nextCursor", Required = Required.Default)]
        public string NextCursor { get; set; }
    }

    [JsonObject]
    public class ObjectDataResponse
    {
        [JsonProperty("data", Required = Required.Default)]
        public ObjectData Data { get; set; }

        [JsonProperty("error", Required = Required.Default)]
        public string Error { get; set; }

        public int? GetSharedObjectInitialVersion()
        {
            if (this.Data == null)
                return null;

            Owner owner = this.Data.Owner;

            if (owner == null || owner.Shared == null)
                return null;

            return owner.Shared.InitialSharedVersion;
        }

        public Sui.Types.SuiObjectRef GetObjectReference()
        {
            if (this.Data == null)
                return null;

            return new Sui.Types.SuiObjectRef
            (
                AccountAddress.FromHex(this.Data.ObjectId),
                (int)this.Data.Version,
                this.Data.Digest
            );
        }
    }

    [JsonObject]
    public class ObjectData
    {
        [JsonProperty("bcs", Required = Required.Default), JsonConverter(typeof(RawDataJsonConverter))]
        public RawData Bcs { get; set; }

        [JsonProperty("content", Required = Required.Default), JsonConverter(typeof(DataJsonConverter))]
        public Data Content { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        [JsonProperty("display", Required = Required.Default)]
        public DisplayFieldsResponse Display { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("owner", Required = Required.Default)]
        public Owner Owner { get; set; }

        [JsonProperty("previousTransaction", Required = Required.Default)]
        public string PreviousTransaction { get; set; }

        [JsonProperty("storageRebate", Required = Required.Default)]
        public BigInteger? StorageRebate { get; set; }

        [JsonProperty("type", Required = Required.Default)]
        public string Type { get; set; }

        [JsonProperty("version", Required = Required.Default)]
        public BigInteger Version { get; set; }
    }
}