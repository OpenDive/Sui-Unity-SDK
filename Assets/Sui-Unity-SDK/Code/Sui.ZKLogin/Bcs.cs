using Newtonsoft.Json;
using OpenDive.BCS;

namespace Sui.ZKLogin
{
    /// <summary>
    /// TODO: Complete implementation
    /// TODO: See if we can just make the ZKLogin Signature class `ISerializable` to that we can just call `serialize` on it.
    /// TODO: See why this signature looks like this" inputs, maxEpochs, userSignature
    /// </summary>
    [JsonObject]
    public class ZkLoginSignatureBCS: ISerializable
    {
        [JsonProperty("inputs")]
        public Inputs Inputs { get; set; }

        [JsonProperty("maxEpoch")]
        public ulong MaxEpoch { get; set; }

        [JsonProperty("userSignature")]
        public byte[] UserSignature { get; set; } // bcs.vector(bcs.u8()),

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Inputs);
            serializer.SerializeU64(MaxEpoch);
            serializer.SerializeBytes(UserSignature);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            throw new System.NotImplementedException();
        }
    }

    //[JsonObject]
    //public class Inputs: ISerializable
    //{
    //    [JsonProperty("proofPoints")]
    //    public ProofPoints ProofPoints { get; set; }

    //    [JsonProperty("issBase64Details")]
    //    public ZkLoginSignatureInputsClaim IssBase64Details { get; set; }

    //    [JsonProperty("headerBase64")]
    //    public BigInteger HeaderBase64 { get; set; }

    //    [JsonProperty("addressSeed")]
    //    public string AddressSeed { get; set; }

    //    public void Serialize(Serialization serializer)
    //    {
    //        serializer.Serialize(ProofPoints);
    //        serializer.Serialize(IssBase64Details);
    //        serializer.Serialize(HeaderBase64);
    //        serializer.SerializeString(AddressSeed);
    //    }
    //}

    //[JsonObject]
    //public class ProofPoints: ISerializable
    //{
    //    // Sequence input = new Sequence(new string[] { "a", "abc", "def", "ghi" }.ToList().Select(str => new BString(str)).ToArray());
    //    [JsonProperty("a")]
    //    public Sequence A { get; set; } // a: bcs.vector(bcs.string())

    //    [JsonProperty("b")]
    //    public Sequence B { get; set; } // b: bcs.vector(bcs.vector(bcs.string())),

    //    [JsonProperty("c")]
    //    public Sequence C { get; set; } // c: bcs.vector(bcs.string()),

    //    public void Serialize(Serialization serializer)
    //    {
    //        serializer.Serialize(A);
    //        serializer.Serialize(B);
    //        serializer.Serialize(C);
    //    }
    //}

    //[JsonObject]
    //public class ZkLoginSignatureInputsClaim: ISerializable
    //{
    //    [JsonProperty("value")]
    //    public string Value { get; set; }

    //    [JsonProperty("indexMod4")]
    //    public byte IndexMod4 { get; set; }

    //    public void Serialize(Serialization serializer)
    //    {
    //        serializer.SerializeString(Value);
    //        serializer.SerializeU8(IndexMod4);
    //    }
    //}
}
