using System;

namespace Sui.Cryptography
{
    public interface IPrivateKey
    {
        /// <summary>
        /// Creates a PrivateKey object from a private key represented as a hex string
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static PrivateKeyBase FromHex(string hexStr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a PrivateKey object from a private key represented as a base64 string
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        public static PrivateKeyBase FromBase64(string base64Str)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a PrivateKey object from a random seed
        /// </summary>
        /// <returns></returns>
        public static PrivateKeyBase Random() => throw new NotImplementedException();

        /// <summary>
        /// Use private key to sign abitrary data in byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SignatureBase Sign(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sign an arbitrary b64 string.
        /// </summary>
        /// <param name="b64Message"></param>
        /// <returns></returns>
        public abstract string Sign(string b64Message);
    }
}