using NUnit.Framework;
using System;
using Sui.Utilities;
using Sui.Cryptography.Ed25519;
using UnityEngine;

namespace Sui.Tests.Cryptography
{
    public class ED25519Test
    {
        byte[] privateKeyBytes = TestValues.PrivateKeyBytes;
        string expPrivateKeyHex = TestValues.PrivateKeyHex;
        byte[] expSignatureBytes = TestValues.SignatureBytes;

        byte[] privateKeyBytesInvalid = TestValues.PrivateKeyBytesInvalidLength;

        [Test]
        public void PrivateKeyRandom()
        {
            PrivateKey privateKey = (PrivateKey)PrivateKey.Random();
            byte[] keyBytes = privateKey.KeyBytes;
            Assert.AreEqual(32, keyBytes.Length);
        }

        [Test]
        public void PrivateKeyFromBytesSuccess()
        {
            PrivateKey privateKey = new(privateKeyBytes);

            byte[] ActualKeyBytes = privateKey.KeyBytes;            
            Assert.AreEqual(32, ActualKeyBytes.Length);
            Assert.AreEqual(privateKeyBytes, ActualKeyBytes);

            string actual = privateKey.KeyHex;
            Assert.AreEqual(expPrivateKeyHex, actual);
        }

        [Test]
        public void PrivateKeyFromBytesInvalidLength()
        {
            var ex = Assert.Throws<ArgumentException>(() => new PrivateKey(privateKeyBytesInvalid));
            Assert.AreEqual("Invalid key length: \nParameter name: privateKey", ex.Message);
        }

        [Test]
        public void PrivateKeyFromHexStringSuccess()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyFromHexStringInvalid()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyFromBase64tringSuccess()
        {
            string pkHex = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828";
            string pkBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            byte[] pkBytes = { 153, 218, 149, 89, 225, 94, 145, 62, 233, 171, 46, 83, 227, 223, 173, 87, 93, 163, 59, 73, 190, 17, 37, 187, 146, 46, 51, 73, 79, 73, 136, 40 };
            string publicKey = "Gy9JCW4+Xb0Pz6nAwM2S2as7IVRLNNXdSmXZi4eLmSI=";
            PrivateKey pk = new PrivateKey(pkBase64);

            Assert.AreEqual(1, 0, pk.PublicKey().ToBase64());
            //Assert.AreEqual(pkBase64, pk.KeyBase64);
        }

        [Test]
        public void PrivateKeyFromBase64StringInvalid()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyToString()
        {
            PrivateKey pk1 = new PrivateKey("0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828");
            string pkBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            Assert.AreEqual(pkBase64, pk1.ToString());
        }

        [Test]
        public void PrivateKeyToHexString()
        {
            PrivateKey privateKey = new(privateKeyBytes);

            byte[] keyBytes = privateKey.KeyBytes;
            Assert.AreEqual(32, keyBytes.Length);

            string actual = privateKey.ToHex();
            Assert.AreEqual(expPrivateKeyHex, actual);
        }

        [Test]
        public void PrivateKeyToBase64String()
        {
            PrivateKey pk1 = new PrivateKey("0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828");
            string pkBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            Assert.AreEqual(pkBase64, pk1.KeyBase64);
        }

        [Test]
        public void PrivateKeyComparisonTrue()
        {
            PrivateKey pk1 = new PrivateKey("0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828");
            PrivateKey pk2 = new PrivateKey("mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=");
            Assert.AreEqual(pk1, pk2);
        }

        [Test]
        public void PrivateKeyComparisonFalse()
        {
            PrivateKey pk1 = new PrivateKey("mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=");
            PrivateKey pk2 = new PrivateKey("Uz39UFseB/B38iBwjesIU1JZxY6y+TRL9P84JFw41W4=");
            Assert.AreNotEqual(pk1, pk2);
        }

        [Test]
        public void PrivateKeySignature()
        {
            Assert.AreEqual(1, 0);
        }

        /// <summary>
        /// Public key
        /// </summary>
        [Test]
        public void PublicKeyFromBytesSuccess()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBytes);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToBase64());
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToString());
            Assert.AreEqual(TestValues.ValidKeyHex, publicKey.KeyHex);
            Assert.AreEqual(TestValues.ValidKeyBytes, publicKey.KeyBytes);
        }

        [Test]
        public void PublicKeyFromBytesInvalidLength()
        {
            byte[] invalidPublicKey = { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var ex = Assert.Throws<ArgumentException>(() => new PublicKey(invalidPublicKey));
            Assert.AreEqual("Invalid key length: \nParameter name: publicKey", ex.Message);
        }

        [Test]
        public void PublicKeyFromHexStringSuccess()
        {
            PublicKey publicKey = new(TestValues.ValidKeyHex);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToBase64());
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToString());
            Assert.AreEqual(TestValues.ValidKeyHex, publicKey.KeyHex);
            Assert.AreEqual(TestValues.ValidKeyBytes, publicKey.KeyBytes);
        }

        [Test]
        public void PublicKeyFromHexStringInvalid()
        {
            string invalidPublicKeyHex = "0x30000000";
            var ex = Assert.Throws<ArgumentException>(() => new PublicKey(invalidPublicKeyHex));
            Assert.AreEqual("Invalid key: \nParameter name: publicKey", ex.Message);
        }

        [Test]
        public void PublicKeyFromBase64StringSuccess()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToBase64());
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToString());
            Assert.AreEqual(TestValues.ValidKeyHex, publicKey.KeyHex);
            Assert.AreEqual(TestValues.ValidKeyBytes, publicKey.KeyBytes);
        }

        [Test]
        public void PublicKeyFromBase64StringInvalid()
        {
            string invalidPublicKeyHex = "Uz39UFseB/B38iBwjesIU1JZxY6y+TRL9P84JFw414=";
            var ex = Assert.Throws<ArgumentException>(() => new PublicKey(invalidPublicKeyHex));
            Assert.AreEqual("Invalid key: \nParameter name: publicKey", ex.Message);
        }

        [Test]
        public void PublicKeyToString()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToString());
        }

        [Test]
        public void PublicKeyToHexString()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(TestValues.ValidKeyHex, publicKey.KeyHex);
        }

        [Test]
        public void PublicKeyToBase64String()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToBase64());
        }

        [Test]
        public void PublicKeyComparisonTrue()
        {
            PublicKey publicKeyOne = new(TestValues.ValidKeyHex);
            PublicKey publicKeyTwo = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(publicKeyOne, publicKeyTwo, publicKeyOne.KeyHex + "!\n" + publicKeyTwo.KeyHex + "!");
        }

        [Test]
        public void PublicKeyComparisonFalse()
        {
            PublicKey publicKeyOne = new("0xd77a6cd55073e98d4029b1b0b8bd8d88f45f343dad2732fc9a7965094e635c55");
            PublicKey publicKeyTwo = new(TestValues.ValidKeyBase64);
            Assert.AreNotEqual(publicKeyOne, publicKeyTwo, publicKeyOne.KeyHex + "!\n" + publicKeyTwo.KeyHex + "!");
        }

        [Test]
        public void PublicKeySuiPublicKey()
        {
            for(int i = 0; i < TestValues.TestCases.Length; i++)
            {
                string rawPublicKey = TestValues.TestCases[i].Item1;
                string suiPublicKey = TestValues.TestCases[i].Item2;
                PublicKey publicKey = new PublicKey(rawPublicKey);
                Assert.AreEqual(suiPublicKey, publicKey.ToSuiPublicKey());
            }
        }

        [Test]
        public void PublicKeySuiAddress()
        {
            for (int i = 0; i < TestValues.TestCases.Length; i++)
            {
                string rawPublicKey = TestValues.TestCases[i].Item1;
                string suiAddress = TestValues.TestCases[i].Item3;

                PublicKey publicKey = new PublicKey(rawPublicKey);
                Assert.AreEqual(suiAddress, publicKey.ToSuiAddress(), publicKey.ToSuiAddress().Length + "---- \n" + publicKey.ToSuiBytes().ToReadableString() + "\n" + publicKey.ToSuiAddress());
            }
        }
    }
}