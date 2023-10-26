using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class ObjectDataResponse
    {
        [JsonProperty("data", Required = Required.Default)]
        public ObjectData Data { get; set; }

        [JsonProperty("error", Required = Required.Default)]
        public string Error { get; set; }
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

        [JsonProperty("owner")]
        public Owner Owner { get; set; }

        [JsonProperty("previousTransaction", Required = Required.Default)]
        public string PreviousTransaction { get; set; }

        [JsonProperty("storageRebate", Required = Required.Default)]
        public BigInteger? StorageRebate { get; set; }

        [JsonProperty("type", Required = Required.Default)]
        public string Type { get; set; }

        [JsonProperty("version")]
        public BigInteger Version { get; set; }
    }
}