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
        /// Create a PrivateKey object from a raw secret key byte array, also known as seed.
        /// This is NOT the private scalar which is result of hashing and bit clamping of
        /// the raw secret key.
        ///
        /// The sui.keystore key is a list of Base64 encoded `flag || privkey`. To import
        /// a key from sui.keystore to typescript, decode from base64 and remove the first
        /// flag byte after checking it is indeed the Ed25519 scheme flag 0x00 (See more
        /// on flag for signature scheme: https://github.com/MystenLabs/sui/blob/818406c5abdf7de1b80915a0519071eec3a5b1c7/crates/sui-types/src/crypto.rs#L1650):
        /// ```
        /// import { Ed25519Keypair, fromB64 }
        /// from '@mysten/sui.js';
        /// const raw = fromB64(t[1]);
        /// if (raw[0] !== 0 || raw.length !== PRIVATE_KEY_SIZE + 1) {
        ///     throw new Error('invalid key');
        /// }
        /// const imported = Ed25519Keypair.fromSecretKey(raw.slice(1))
        /// </summary>
        /// <param name="secretKet"></param>
        /// <returns></returns>
        public static PrivateKeyBase FromSecretKey(int[] secretKey)
        {
            throw new NotImplementedException();
        }

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