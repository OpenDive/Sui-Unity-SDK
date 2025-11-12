using NBitcoin.JsonConverters;
using NUnit.Framework;
using OpenDive.BCS;
using Sui.Cryptography;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json;

namespace Sui.ZKLogin
{
    //public class Signature : Sui.Cryptography.Signature
    //{
    //    public Signature(string signature) : base(signature)
    //    {
    //    }

    //    public Signature(byte[] signature) : base(signature)
    //    {
    //    }

    //    public byte[] GetZkLoginSignatureBytes()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public string GetZkLoginSignature()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public string ParseZkLoginSignature()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    /// <summary>
    /// TODO: Implement ZkLogin Signature.
    /// TODO: See if it can be extended from the core Signature class.
    /// </summary>
    [JsonObject]
    public class ZkLoginSignature : ISerializable
    {
        //private Inputs SigInputs;
        //private ulong MaxEpoch;
        //private byte[] UserSignature;

        //public ZkLoginSignature(Inputs inputs, ulong MaxEpoch, byte[] UserSignature)
        //{
        //    this.SigInputs = inputs;
        //    this.MaxEpoch = MaxEpoch;
        //}

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

        public static byte[] GetZkLoginSignatureBytes(Inputs inputs, ulong maxEpoch, byte[] userSignature)
        {
            ZkLoginSignature sig = new ZkLoginSignature();
            sig.Inputs = inputs;
            sig.MaxEpoch = maxEpoch;
            sig.UserSignature = userSignature;

            Serialization ser = new Serialization();
            sig.Serialize(ser);
            byte[] sigBytes = ser.GetBytes();
            return sigBytes;
        }

        public static string GetZkLoginSignature(Inputs inputs, ulong maxEpoch, byte[] userSignature)
        {
            byte[] bytes = GetZkLoginSignatureBytes(inputs, maxEpoch, userSignature);
            byte[] signatureBytes = new byte[bytes.Length + 1];
            signatureBytes[0] = SignatureSchemeToFlag.ZkLogin;
            Buffer.BlockCopy(bytes, 0, signatureBytes, 1, bytes.Length);
            return Convert.ToBase64String(signatureBytes);
        }

        public string ParseZkLoginSignature(string strSignature)
        {
            byte[] signatureBytes = Convert.FromBase64String(strSignature);
            return ParseZkLoginSignature(signatureBytes);
        }

        public string ParseZkLoginSignature(byte[] signatureBytes)
        {
            //return ZkLoginSignature.Parse(signatureBytes);
            throw new NotImplementedException();
        }
    }

    [JsonObject]
    public class Inputs : ISerializable
    {
        [JsonProperty("proofPoints")]
        public ProofPoints ProofPoints { get; set; }

        [JsonProperty("issBase64Details")]
        public ZkLoginSignatureInputsClaim IssBase64Details { get; set; }

        [JsonProperty("headerBase64")]
        public string HeaderBase64 { get; set; }

        [JsonProperty("addressSeed")]
        public string AddressSeed { get; set; }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(ProofPoints);
            serializer.Serialize(IssBase64Details);
            serializer.SerializeString(HeaderBase64);
            serializer.SerializeString(AddressSeed);
        }
    }

    [JsonObject]
    public class ProofPoints : ISerializable
    {
        [JsonProperty("a")]
        public List<string> a { get; set; }

        [JsonProperty("b")]
        public List<List<string>> b { get; set; } // b: bcs.vector(bcs.vector(bcs.string())),

        [JsonProperty("c")]
        public List<string> c { get; set; } // c: bcs.vector(bcs.string()),

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)a.Count);
            foreach (var item in a)
            {
                serializer.SerializeString(item);
            }


            // --- B ---
            serializer.SerializeU32AsUleb128((uint)b.Count);
            foreach (var innerList in b)
            {
                serializer.SerializeU32AsUleb128((uint)innerList.Count);
                foreach (var item in innerList)
                {
                    serializer.SerializeString(item);
                }

            }

            // --- C ---
            serializer.SerializeU32AsUleb128((uint)c.Count);
            foreach (var item in c)
            {
                serializer.SerializeString(item);
            }
        }
    }

    [JsonObject]
    public class ZkLoginSignatureInputsClaim : ISerializable
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("indexMod4")]
        public byte IndexMod4 { get; set; }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeString(Value);
            serializer.SerializeU8(IndexMod4);
        }
    }
}