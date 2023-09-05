using System;
using Org.BouncyCastle.Crypto.Digests;
using Sui.Cryptography;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Accounts
{
    /// <summary>
    /// Represents a Sui address associated with a give public key.
    /// 
    /// For deriving a 32-byte Sui address,
    /// Sui hashes the signature scheme flag 1-byte concatenated with public key bytes
    /// using the BLAKE2b (256 bits output) hashing function.
    ///
    /// Sui address currently supports pure Ed25519, Secp256k1, Secp256r1,
    /// and MultiSig with corresponding flag bytes of 0x00, 0x01, 0x02, and 0x03, respectively.
    /// https://docs.sui.io/learn/cryptography/sui-wallet-specs#address-format
    /// </summary>
    public class AccountAddress
    {
        private static readonly int Length = 32;
        private readonly byte[] AddressBytes;

        public AccountAddress(PublicKeyBase publicKey)
        {
            new AccountAddress(publicKey.KeyBytes, publicKey.SignatureScheme);
        }

        /// <summary>
        /// Cerates an AccountAddress object using a public key and a signature scheme.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="signatureScheme"></param>
        public AccountAddress(byte[] publicKey, SignatureScheme signatureScheme)
        {
            byte[] suiBytes = PublicKeyBase.ToSuiBytes(signatureScheme, publicKey);

            // BLAKE2b hash
            byte[] result = new byte[64];
            Blake2bDigest blake2b = new(256);
            blake2b.BlockUpdate(suiBytes, 0, suiBytes.Length);
            blake2b.DoFinal(result, 0);

            new AccountAddress(result);
        }

        /// <summary>
        /// Create an AccountAddress object from a byte array representation.
        /// Sui hashes the signature scheme flag 1-byte concatenated with public key bytes.
        /// </summary>
        /// <param name="address"></param>
        public AccountAddress(byte[] suiAddress)
        {
            if (suiAddress.Length != Length)
                throw new ArgumentException("Invalid address length. It must be " + Length + " bytes");
            AddressBytes = suiAddress;
        }

        public static AccountAddress FromKey(byte[] publicKey)
        {
            throw new NotImplementedException();
        }

        public static AccountAddress FromHex(string publicKey)
        {
            throw new NotImplementedException();
        }

        public static AccountAddress FromBase64(string publicKey)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            // Convert to hex string
            string addressHex = BitConverter.ToString(AddressBytes); // Turn into hexadecimal string
            addressHex = addressHex.Replace("-", "").ToLowerInvariant(); // Remove '-' characters from hex string hash
            addressHex = "0x" + addressHex.Substring(0, 64);

            return addressHex;
        }
    }
}