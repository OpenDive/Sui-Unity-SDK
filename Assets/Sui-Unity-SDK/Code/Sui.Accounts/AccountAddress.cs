using System;
using OpenDive.BCS;
using Chaos.NaCl;
using Org.BouncyCastle.Crypto.Digests;
using Sui.Cryptography;
using Sui.Utilities;
using static Sui.Cryptography.SignatureUtils;
using Newtonsoft.Json;
using Sui.Rpc.Models;
using UnityEngine;
using System.Text;
using static Sui.Rpc.Models.Stakes;
using System.Linq;

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
    [JsonConverter(typeof(SuiAddressJsonConverter))]
    public class AccountAddress : ISerializableTag
    {
        public static readonly int OldLength = 20;

        /// <summary>
        /// Length of a Sui account address.
        /// </summary>
        public static readonly int Length = 32;


        private byte[] _addressBytes;
        /// <summary>
        /// Sui address in byte array format.
        /// </summary>
        public byte[] AddressBytes
        {
            get => _addressBytes;
            set => _addressBytes = value;
        }

        private SignatureScheme _signatureScheme;
        /// <summary>
        /// Return the signature scheme.
        /// If the AccountAddress object was initialized with a sui address byte representation,
        /// then we get the first byte from that byte array -- this represents the
        /// signature scheme flat.
        /// </summary>
        public SignatureScheme SignatureScheme
        {
            get
            {
                // TODO: Look into how to check if an enum property is set
                if (_signatureScheme <= 0)
                {
                    byte flag = AddressBytes[0];
                    _signatureScheme = SignatureFlagToScheme.GetScheme(flag);
                }
                return _signatureScheme;
            }

            set => _signatureScheme = value;
        }

        /// <summary>
        /// Create an AccountAddress object using a PublicKey object.
        /// </summary>
        /// <param name="publicKey"></param>
        public AccountAddress(PublicKeyBase publicKey)
        {
            new AccountAddress(publicKey.KeyBytes, publicKey.SignatureScheme);
        }

        /// <summary>
        /// Cerate an AccountAddress object using a public key and a signature scheme.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="signatureScheme"></param>
        public AccountAddress(byte[] publicKey, SignatureScheme signatureScheme)
        {
            SignatureScheme = signatureScheme;
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
            Debug.Log("IRVIN::: bytes length: " + suiAddress.Length);
            if (suiAddress.Length != OldLength && suiAddress.Length != Length)
                throw new ArgumentException("Invalid address length. It must be " + Length + " bytes");
            Debug.Log("IRVIN::: bytes assigning ...");
            AddressBytes = suiAddress;
            Debug.Log("IRVIN::: bytes assigning ...");
        }

        /// <summary>
        /// Create an AccountAddress object from a given public key and signature scheme.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="signatureScheme"></param>
        /// <returns></returns>
        public static AccountAddress FromKey(byte[] publicKey, SignatureScheme signatureScheme)
        {
            return new AccountAddress(publicKey, signatureScheme);
        }

        /// <summary>
        /// Create an AccountAddress object from a hex string representation of a Sui address
        /// </summary>
        /// <param name="suiAddress"></param>
        /// <returns></returns>
        public static AccountAddress FromHex(string suiAddress)
        {
            //byte[] suiAddressBytes = suiAddress.HexStringToByteArray();
            //return new AccountAddress(suiAddressBytes);

            if (string.IsNullOrWhiteSpace(suiAddress))
                throw new ArgumentException("Address string is empty.");

            //if (suiAddress.Contains("0x") && suiAddress.Length == 3)
            //{
            //    suiAddress = "0x" + suiAddress;
            //}

            string addr = suiAddress;

            if (suiAddress[0..2].Equals("0x")) {
                addr = suiAddress[2..];
            }

            // TODO: Document that Sui changed their address length from 20 to 32, hence some old addresses are shorter
            if (addr.Length < AccountAddress.Length * 2)
            {
                string pad = new string('0', AccountAddress.Length * 2 - addr.Length);
                addr = pad + addr;
            }

            Debug.Log("MARCUS:::HEX STRING - " + addr.HexStringToByteArray().ByteArrayToString());

            return new AccountAddress(addr.HexStringToByteArray());
        }

        public override bool Equals(object obj)
        {
            if (obj is not AccountAddress)
                throw new NotImplementedException();

            AccountAddress other_addr = (AccountAddress)obj;

            return this.AddressBytes.SequenceEqual(other_addr.AddressBytes);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Create an AccountAddress object from a base64 string representation of a Sui address
        /// </summary>
        /// <param name="suiAddress"></param>
        /// <returns></returns>
        public static AccountAddress FromBase64(string suiAddress)
        {
            byte[] suiAddressBytes = Convert.FromBase64String(suiAddress);
            return new AccountAddress(suiAddressBytes);
        }

        public static AccountAddress FromBase58(string suiAddress)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(suiAddress);
            Base58Encoder base58Encoder = new Base58Encoder();
            if (Base58Encoder.IsValidEncoding(suiAddress) == false)
                throw new ArgumentException("Not a valid Base58 string - " + suiAddress);

            Debug.Log(suiAddress);
            byte[] suiAddressBytes = base58Encoder.DecodeData(suiAddress);
            return new AccountAddress(suiAddressBytes);
        }

        /// <summary>
        /// Returns a hex string representation of a Sui address.
        /// </summary>
        /// <returns></returns>
        public string ToHex()
        {
            string addressHex = BitConverter.ToString(AddressBytes); // Turn into hexadecimal string
            addressHex = addressHex.Replace("-", "").ToLowerInvariant(); // Remove '-' characters from hex string hash
            //addressHex = "0x" + addressHex.Substring(0, 64); // TODO: Address this
            addressHex = "0x" + addressHex.Substring(0, AddressBytes.Length * 2); // It is assumed that at this stage the bytes are valid, so it's either 20 or 32 bytes
            return addressHex;
        }

        /// <summary>
        /// Returns a base64 string representation of a Sui address.
        /// </summary>
        /// <returns></returns>
        public string ToBase64()
        {
            return CryptoBytes.ToBase64String(AddressBytes);
        }

        /// <summary>
        /// Returns a base64 string representation of a Sui address.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToHex();
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeFixedBytes(this.AddressBytes);
        }

        public static AccountAddress Deserialize(Deserialization deserializer)
        {
            throw new NotImplementedException();
        }

        public TypeTag Variant()
        {
            throw new NotImplementedException();
        }

        public object GetValue()
        {
            throw new NotImplementedException();
        }
    }
}