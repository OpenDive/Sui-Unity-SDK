using System;
using Sui.Cryptography;

namespace Sui.Cryptography
{
    public interface IPrivateKey
    {
        public static PrivateKeyBase FromHex(string hexStr) => throw new NotImplementedException();
        public static PrivateKeyBase FromBase64(string base64Str) => throw new NotImplementedException();
        public static PrivateKeyBase Random() => throw new NotImplementedException();
        public static SignatureBase Sign(byte[] data) => throw new NotImplementedException();
    }
}