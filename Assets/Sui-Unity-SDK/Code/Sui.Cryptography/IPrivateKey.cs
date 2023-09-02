using System;

namespace Sui.Cryptography
{
    public interface IPrivateKey
    {
        public byte[] KeyBytes { get; }
        public string Hex();
        public string Base64();
        public static IPrivateKey FromHex(string hexStr) => throw new NotImplementedException();
        public static IPrivateKey FromBase64(string base64Str) => throw new NotImplementedException();
        public static IPrivateKey Random() => throw new NotImplementedException();
        public SignatureBase Sign(byte[] data);
        //public void Serialize(Serializer serializer);
    }
}